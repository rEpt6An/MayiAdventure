// UIcontrol.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIcontrol : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject battleStartPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject battleStatsPanel;

    public GameObject gameWonPanel; // *** 新增: 游戏通关面板 ***


    [Header("战斗状态UI引用")]
    public Character playerBattleUI;
    public Character enemyBattleUI;

    [Header("胜利面板/战利品栏")]
    public Transform rewardContainer;
    public GameObject rewardUIPrefab;
    public Button victoryReturnButton;
    public TextMeshProUGUI victoryReturnButtonText;

    [Header("其他按钮")]
    public Button startBattleButton;
    public Button defeatReturnButton;

    [Header("核心引用")]
    [SerializeField] private PlayerData runtimePlayerData; // *** 核心修复 1: 直接引用全局玩家数据 ***
    [SerializeField] private RewardDatabase rewardDatabase;
    [SerializeField] private GameSessionData runtimeSessionData; // *** 新增: 引用会话数据 ***

    public Battle battleManager;
    public SceneSwitcher sceneSwitcher;

    private Dictionary<RewardUI, LootGroup> rewardUIGroupMap = new Dictionary<RewardUI, LootGroup>();
    private int rewardsLeft;

    void Awake()
    {
        if (battleManager == null) battleManager = FindObjectOfType<Battle>();
        if (battleManager == null)
        {
            Debug.LogError("UIcontrol: 未找到 Battle 管理器！", this.gameObject);
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
            // *** 核心改动: 检查是否是Boss战胜利 ***
            if (runtimeSessionData != null && runtimeSessionData.isFinalBossBattle)
            {
                // 如果是，显示游戏通关面板
                Debug.Log("最终Boss已被击败！游戏胜利！");
                gameWonPanel?.SetActive(true);
            }
            else
            {
                // 如果是普通胜利，显示战利品面板
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
        // 失败 或 Boss 战胜利 → MainMenu；其余 → Map
        bool toMainMenu =
            defeatPanel?.activeSelf == true ||            // 失败
            gameWonPanel?.activeSelf == true;             // Boss 战胜利

        sceneSwitcher?.SwitchScene(toMainMenu ? "MainMenu" : "Map");
    }

    // *** 核心修复: 重构整个战利品生成方法，以确保逻辑的绝对正确性 ***
    private void GenerateAndDisplayRewards()
    {
        // 1. 清理工作
        foreach (Transform child in rewardContainer) Destroy(child.gameObject);
        rewardUIGroupMap.Clear();

        victoryReturnButton?.gameObject.SetActive(true);
        if (victoryReturnButtonText != null) victoryReturnButtonText.text = "跳过奖励";

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

        // --- 3. 遍历掉落组，生成运行时 Reward 对象列表 ---
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
                        // *** 关键修复 1: 确保 itemData 和 quantity 被正确赋值 ***
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

            // --- 根据组规则处理 (RollN, PickN, TakeAll) ---
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

        // --- 4. 实例化UI ---
        foreach (var tuple in rewardsToProcess)
        {
            var rewardData = tuple.reward;
            var sourceGroup = tuple.group;
            var originalLootDrop = tuple.originalLootDrop;

            RewardInfo rewardInfo = rewardDatabase.GetRewardInfo(rewardData.type);

            Sprite iconToShow;
            string descriptionToShow;

            // --- 关键修复 2: 正确处理描述和图标 ---
            string descriptionTemplate;
            if (originalLootDrop != null && !string.IsNullOrEmpty(originalLootDrop.overrideDescription))
            {
                descriptionTemplate = originalLootDrop.overrideDescription;
            }
            else
            {
                // 如果是物品，描述应该来自物品本身；如果是属性，来自数据库
                descriptionTemplate = (rewardData.type == RewardType.Item && rewardData.itemData != null)
                    ? rewardData.itemData.description
                    : rewardInfo?.defaultDescription ?? ""; // 安全回退
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
            victoryReturnButtonText.text = "返回";
        }
        rewardUIGroupMap.Clear();
    }
}