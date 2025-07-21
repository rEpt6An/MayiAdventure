// PlayerStatsUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("数据源")]
    [Tooltip("在Project窗口中的那个运行时玩家数据文件 Player.asset")]
    [SerializeField] private PlayerData runtimePlayerData;

    [Header("UI 元素引用")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI foodText;

    // 一个私有变量，用来存储上一帧的数据，以避免不必要的UI更新
    private float lastHp, lastMaxHp, lastGold, lastFood, lastMaxFood;

    void OnEnable()
    {
        // 订阅ResetPlayer发出的“数据已重置”事件
        ResetPlayer.OnPlayerDataReset += HandleDataReset;
    }

    void OnDisable()
    {
        // 取消订阅，防止内存泄漏
        ResetPlayer.OnPlayerDataReset -= HandleDataReset;
    }

    void Start()
    {
        if (runtimePlayerData == null)
        {
            Debug.LogError("PlayerStatsUI 没有设置运行时玩家数据 (runtimePlayerData)！");
            gameObject.SetActive(false);
            return;
        }

        UpdateAllStats();
    }

    // Update会在每一帧执行，检查数据是否有变化
    void LateUpdate() // 使用LateUpdate可以确保在所有游戏逻辑结算完毕后再更新UI
    {
        // *** 核心修复 2: 只有当数据真正发生变化时，才去更新UI ***
        // 这是一种性能优化，也进一步避免了潜在的循环问题
        if (runtimePlayerData.currentHP != lastHp || runtimePlayerData.maxHP != lastMaxHp ||
            runtimePlayerData.Gold != lastGold || runtimePlayerData.Food != lastFood ||
            runtimePlayerData.MaxFood != lastMaxFood)
        {
            UpdateAllStats();
        }
    }

    private void HandleDataReset()
    {
        Debug.Log("PlayerStatsUI: 收到了数据重置信号，正在刷新UI...");
        UpdateAllStats();
    }

    /// <summary>
    /// 一个统一的、更新所有状态显示的方法
    /// </summary>
    public void UpdateAllStats()
    {
        if (runtimePlayerData == null) return;

        // 更新血条
        if (hpSlider != null)
        {
            if (runtimePlayerData.maxHP > 0)
                hpSlider.value = (float)runtimePlayerData.currentHP / runtimePlayerData.maxHP;
        }
        if (hpText != null)
        {
            hpText.text = $"{runtimePlayerData.currentHP} / {runtimePlayerData.maxHP}";
        }

        // 更新金币和食物
        if (goldText != null)
        {
            goldText.text = runtimePlayerData.Gold.ToString();
        }
        if (foodText != null)
        {
            foodText.text = $"{runtimePlayerData.Food} / {runtimePlayerData.MaxFood}";
        }

        // *** 核心修复 3: 更新完UI后，记录当前数据 ***
        lastHp = runtimePlayerData.currentHP;
        lastMaxHp = runtimePlayerData.maxHP;
        lastGold = runtimePlayerData.Gold;
        lastFood = runtimePlayerData.Food;
        lastMaxFood = runtimePlayerData.MaxFood;
    }
}