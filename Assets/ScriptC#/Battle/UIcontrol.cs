// UIcontrol.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIcontrol : MonoBehaviour
{
    [Header("UI ���")]
    public GameObject battleStartPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject battleStatsPanel;

    public GameObject gameWonPanel; // *** ����: ��Ϸͨ����� ***


    [Header("ս��״̬UI����")]
    public Character playerBattleUI;
    public Character enemyBattleUI;

    [Header("ʤ�����/ս��Ʒ��")]
    public Transform rewardContainer;
    public GameObject rewardUIPrefab;
    public Button victoryReturnButton;
    public TextMeshProUGUI victoryReturnButtonText;

    [Header("������ť")]
    public Button startBattleButton;
    public Button defeatReturnButton;

    [Header("��������")]
    [SerializeField] private PlayerData runtimePlayerData; // *** �����޸� 1: ֱ������ȫ��������� ***
    [SerializeField] private RewardDatabase rewardDatabase;
    [SerializeField] private GameSessionData runtimeSessionData; // *** ����: ���ûỰ���� ***

    public Battle battleManager;
    public SceneSwitcher sceneSwitcher;

    private Dictionary<RewardUI, LootGroup> rewardUIGroupMap = new Dictionary<RewardUI, LootGroup>();
    private int rewardsLeft;

    void Awake()
    {
        if (battleManager == null) battleManager = FindObjectOfType<Battle>();
        if (battleManager == null)
        {
            Debug.LogError("UIcontrol: δ�ҵ� Battle ��������", this.gameObject);
            this.enabled = false;
            return;
        }

        if (battleManager.OnBattleEnd == null)
        {
            battleManager.OnBattleEnd = new UnityEngine.Events.UnityEvent<bool>();
        }
        battleManager.OnBattleEnd.AddListener(HandleBattleEnd);

        startBattleButton?.onClick.AddListener(OnStartBattleClicked);
        victoryReturnButton?.onClick.AddListener(ReturnToMap);
        defeatReturnButton?.onClick.AddListener(ReturnToMap);
    }

    void Start()
    {
        SetUIState_PreBattle();
        gameWonPanel?.SetActive(false);

    }

    private void SetUIState_PreBattle()
    {
        battleStartPanel?.SetActive(true);
        battleStatsPanel?.SetActive(true);
        victoryPanel?.SetActive(false);
        defeatPanel?.SetActive(false);

        playerBattleUI?.UpdateAllVisuals();
        enemyBattleUI?.UpdateAllVisuals();
    }

    private void SetUIState_InBattle()
    {
        battleStartPanel?.SetActive(false);
        battleStatsPanel?.SetActive(true);
        victoryPanel?.SetActive(false);
        defeatPanel?.SetActive(false);
    }

    private void SetUIState_PostBattle(bool playerWon)
    {
        battleStatsPanel?.SetActive(true);

        if (playerWon)
        {
            // *** ���ĸĶ�: ����Ƿ���Bossսʤ�� ***
            if (runtimeSessionData != null && runtimeSessionData.isFinalBossBattle)
            {
                // ����ǣ���ʾ��Ϸͨ�����
                Debug.Log("����Boss�ѱ����ܣ���Ϸʤ����");
                gameWonPanel?.SetActive(true);
            }
            else
            {
                // �������ͨʤ������ʾս��Ʒ���
                victoryPanel?.SetActive(true);
                GenerateAndDisplayRewards();
            }
        }
        else
        {
            defeatPanel?.SetActive(true);
        }
    }
    public void OnStartBattleClicked()
    {
        SetUIState_InBattle();
        battleManager?.StartBattle();
    }

    private void HandleBattleEnd(bool isPlayerWin)
    {
        SetUIState_PostBattle(isPlayerWin);
    }

    public void ReturnToMap()
    {
        // ʧ�� �� Boss սʤ�� �� MainMenu������ �� Map
        bool toMainMenu =
            defeatPanel?.activeSelf == true ||            // ʧ��
            gameWonPanel?.activeSelf == true;             // Boss սʤ��

        sceneSwitcher?.SwitchScene(toMainMenu ? "MainMenu" : "Map");
    }

    // *** �����޸�: �ع�����ս��Ʒ���ɷ�������ȷ���߼��ľ�����ȷ�� ***
    private void GenerateAndDisplayRewards()
    {
        // 1. ������
        foreach (Transform child in rewardContainer) Destroy(child.gameObject);
        rewardUIGroupMap.Clear();

        victoryReturnButton?.gameObject.SetActive(true);
        if (victoryReturnButtonText != null) victoryReturnButtonText.text = "��������";

        if (battleManager?.defender?.characterData == null)
        {
            OnAllRewardsTaken();
            return;
        }

        List<LootGroup> allLootGroups = battleManager.defender.characterData.lootGroups;
        if (allLootGroups == null || allLootGroups.Count == 0)
        {
            OnAllRewardsTaken();
            return;
        }

        List<LootGroup> groupsToProcess = new List<LootGroup>();
        switch (battleManager.defender.characterData.globalRule)
        {
            case GlobalLootRule.ProcessAll:
                groupsToProcess.AddRange(allLootGroups);
                break;
            case GlobalLootRule.RollN_Groups:
                List<LootGroup> tempGroups = new List<LootGroup>(allLootGroups);
                for (int i = 0; i < battleManager.defender.characterData.globalRuleAmount && tempGroups.Count > 0; i++)
                {
                    int randomIndex = Random.Range(0, tempGroups.Count);
                    groupsToProcess.Add(tempGroups[randomIndex]);
                    tempGroups.RemoveAt(randomIndex);
                }
                break;
        }

        // --- 3. ���������飬��������ʱ Reward �����б� ---
        var rewardsToProcess = new List<(Reward reward, LootGroup group, LootDrop originalLootDrop)>();

        foreach (var group in groupsToProcess)
        {
            List<(Reward reward, LootDrop originalLootDrop)> possibleDropsInGroup = new List<(Reward, LootDrop)>();
            foreach (var lootDrop in group.drops)
            {
                if (Random.Range(0f, 100f) < lootDrop.dropChance)
                {
                    Reward generatedReward = new Reward();
                    generatedReward.type = lootDrop.type;

                    if (lootDrop.type == RewardType.Item)
                    {
                        // *** �ؼ��޸� 1: ȷ�� itemData �� quantity ����ȷ��ֵ ***
                        generatedReward.itemData = lootDrop.specificItem;
                        generatedReward.itemQuantity = lootDrop.itemQuantity;
                    }
                    else
                    {
                        generatedReward.amount = Random.Range(lootDrop.minAmount, lootDrop.maxAmount + 1);
                    }
                    possibleDropsInGroup.Add((generatedReward, lootDrop));
                }
            }

            // --- ����������� (RollN, PickN, TakeAll) ---
            switch (group.rule)
            {
                case LootGroupRule.TakeAll:
                case LootGroupRule.PickN:
                    foreach (var pair in possibleDropsInGroup) rewardsToProcess.Add((pair.reward, group, pair.originalLootDrop));
                    break;
                case LootGroupRule.RollN:
                    for (int i = 0; i < group.ruleAmount && possibleDropsInGroup.Count > 0; i++)
                    {
                        int randomIndex = Random.Range(0, possibleDropsInGroup.Count);
                        var selectedPair = possibleDropsInGroup[randomIndex];
                        rewardsToProcess.Add((selectedPair.reward, group, selectedPair.originalLootDrop));
                        possibleDropsInGroup.RemoveAt(randomIndex);
                    }
                    break;
            }
        }

        rewardsLeft = rewardsToProcess.Count;
        if (rewardsLeft == 0)
        {
            OnAllRewardsTaken();
            return;
        }

        // --- 4. ʵ����UI ---
        foreach (var tuple in rewardsToProcess)
        {
            var rewardData = tuple.reward;
            var sourceGroup = tuple.group;
            var originalLootDrop = tuple.originalLootDrop;

            RewardInfo rewardInfo = rewardDatabase.GetRewardInfo(rewardData.type);

            Sprite iconToShow;
            string descriptionToShow;

            // --- �ؼ��޸� 2: ��ȷ����������ͼ�� ---
            string descriptionTemplate;
            if (originalLootDrop != null && !string.IsNullOrEmpty(originalLootDrop.overrideDescription))
            {
                descriptionTemplate = originalLootDrop.overrideDescription;
            }
            else
            {
                // �������Ʒ������Ӧ��������Ʒ������������ԣ��������ݿ�
                descriptionTemplate = (rewardData.type == RewardType.Item && rewardData.itemData != null)
                    ? rewardData.itemData.description
                    : rewardInfo?.defaultDescription ?? ""; // ��ȫ����
            }

            if (rewardData.type == RewardType.Item && rewardData.itemData != null)
            {
                iconToShow = rewardData.itemData.icon;
                descriptionToShow = descriptionTemplate.Replace("{itemName}", rewardData.itemData.itemName);
            }
            else
            {
                iconToShow = rewardInfo?.icon;
                descriptionToShow = descriptionTemplate.Replace("{amount}", rewardData.amount.ToString());
            }

            GameObject rewardGO = Instantiate(rewardUIPrefab, rewardContainer);
            RewardUI rewardUI = rewardGO.GetComponent<RewardUI>();
            if (rewardUI != null)
            {
                rewardUI.Setup(rewardData, iconToShow, descriptionToShow, runtimePlayerData, sourceGroup, OnSingleRewardTaken);
                rewardUIGroupMap[rewardUI] = sourceGroup;
            }
        }
    }
    private void OnSingleRewardTaken(RewardUI takenRewardUI, LootGroup sourceGroup)
    {
        if (sourceGroup.rule == LootGroupRule.PickN)
        {
            List<GameObject> siblingsToDestroy = new List<GameObject>();
            foreach (var pair in rewardUIGroupMap)
            {
                if (pair.Value == sourceGroup && pair.Key != takenRewardUI)
                {
                    siblingsToDestroy.Add(pair.Key.gameObject);
                }
            }
            foreach (var sibling in siblingsToDestroy)
            {
                if (sibling != null) Destroy(sibling);
            }
            rewardsLeft = 1;
        }

        rewardUIGroupMap.Remove(takenRewardUI);
        rewardsLeft--;
        if (rewardsLeft <= 0)
        {
            OnAllRewardsTaken();
        }
    }

    private void OnAllRewardsTaken()
    {
        if (victoryReturnButtonText != null)
        {
            victoryReturnButtonText.text = "����";
        }
        rewardUIGroupMap.Clear();
    }
}