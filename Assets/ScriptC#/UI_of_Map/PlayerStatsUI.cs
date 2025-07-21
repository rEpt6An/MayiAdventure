// PlayerStatsUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("����Դ")]
    [Tooltip("��Project�����е��Ǹ�����ʱ��������ļ� Player.asset")]
    [SerializeField] private PlayerData runtimePlayerData;

    [Header("UI Ԫ������")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI foodText;

    // һ��˽�б����������洢��һ֡�����ݣ��Ա��ⲻ��Ҫ��UI����
    private float lastHp, lastMaxHp, lastGold, lastFood, lastMaxFood;

    void OnEnable()
    {
        // ����ResetPlayer�����ġ����������á��¼�
        ResetPlayer.OnPlayerDataReset += HandleDataReset;
    }

    void OnDisable()
    {
        // ȡ�����ģ���ֹ�ڴ�й©
        ResetPlayer.OnPlayerDataReset -= HandleDataReset;
    }

    void Start()
    {
        if (runtimePlayerData == null)
        {
            Debug.LogError("PlayerStatsUI û����������ʱ������� (runtimePlayerData)��");
            gameObject.SetActive(false);
            return;
        }

        UpdateAllStats();
    }

    // Update����ÿһִ֡�У���������Ƿ��б仯
    void LateUpdate() // ʹ��LateUpdate����ȷ����������Ϸ�߼�������Ϻ��ٸ���UI
    {
        // *** �����޸� 2: ֻ�е��������������仯ʱ����ȥ����UI ***
        // ����һ�������Ż���Ҳ��һ��������Ǳ�ڵ�ѭ������
        if (runtimePlayerData.currentHP != lastHp || runtimePlayerData.maxHP != lastMaxHp ||
            runtimePlayerData.Gold != lastGold || runtimePlayerData.Food != lastFood ||
            runtimePlayerData.MaxFood != lastMaxFood)
        {
            UpdateAllStats();
        }
    }

    private void HandleDataReset()
    {
        Debug.Log("PlayerStatsUI: �յ������������źţ�����ˢ��UI...");
        UpdateAllStats();
    }

    /// <summary>
    /// һ��ͳһ�ġ���������״̬��ʾ�ķ���
    /// </summary>
    public void UpdateAllStats()
    {
        if (runtimePlayerData == null) return;

        // ����Ѫ��
        if (hpSlider != null)
        {
            if (runtimePlayerData.maxHP > 0)
                hpSlider.value = (float)runtimePlayerData.currentHP / runtimePlayerData.maxHP;
        }
        if (hpText != null)
        {
            hpText.text = $"{runtimePlayerData.currentHP} / {runtimePlayerData.maxHP}";
        }

        // ���½�Һ�ʳ��
        if (goldText != null)
        {
            goldText.text = runtimePlayerData.Gold.ToString();
        }
        if (foodText != null)
        {
            foodText.text = $"{runtimePlayerData.Food} / {runtimePlayerData.MaxFood}";
        }

        // *** �����޸� 3: ������UI�󣬼�¼��ǰ���� ***
        lastHp = runtimePlayerData.currentHP;
        lastMaxHp = runtimePlayerData.maxHP;
        lastGold = runtimePlayerData.Gold;
        lastFood = runtimePlayerData.Food;
        lastMaxFood = runtimePlayerData.MaxFood;
    }
}