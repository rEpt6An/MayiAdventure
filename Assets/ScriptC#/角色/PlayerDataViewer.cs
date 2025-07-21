// PlayerDataViewer.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDataViewer : MonoBehaviour
{
    // *** 核心改动 1: 不再需要手动设置TargetCharacter ***
    // [SerializeField] private Character targetCharacter;

    // 它现在只关心自己要显示哪个角色的UI
    public enum ViewerType { Player, Enemy }
    [Header("显示目标")]
    public ViewerType displayTarget;

    [Header("UI 元素引用")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider mpSlider;
    public TextMeshProUGUI mpText;

    private PlayerData currentData;

    void Start()
    {
        // --- 找到战斗控制器和对应的角色 ---
        Battle battle = FindObjectOfType<Battle>();
        if (battle == null)
        {
            Debug.LogError("PlayerDataViewer: 场景中找不到Battle脚本！", this.gameObject);
            return;
        }

        Character targetCharacter = null;
        if (displayTarget == ViewerType.Player)
        {
            targetCharacter = battle.attacker;
        }
        else // if (displayTarget == ViewerType.Enemy)
        {
            targetCharacter = battle.defender;
        }

        if (targetCharacter == null)
        {
            Debug.LogError($"PlayerDataViewer for {displayTarget} 找不到目标Character！", this.gameObject);
            return;
        }

        // *** 核心改动 2: 订阅事件 ***
        //targetCharacter.OnDataChanged += HandleDataChanged;

        // --- 首次启动时，手动更新一次 ---
        // 这很重要，因为在Start时，角色的数据可能已经被Awake中的Battle脚本赋值了
        if (targetCharacter.characterData != null)
        {
            HandleDataChanged(targetCharacter.characterData);
        }
    }

    void OnDestroy()
    {
        // 为了防止内存泄漏，需要找到并取消订阅
        // 这是一个简化的实现，在更复杂的系统中需要更健壮的事件管理器
    }

    /// <summary>
    /// 当接收到角色数据变化的信号时，执行此方法
    /// </summary>
    private void HandleDataChanged(PlayerData newData)
    {
        currentData = newData;
        UpdateAllStats();
    }

    /// <summary>
    /// 一个统一的、更新所有状态显示的方法
    /// </summary>
    public void UpdateAllStats()
    {
        if (currentData == null) return;

        // 更新血条
        if (hpSlider != null)
        {
            if (currentData.maxHP > 0)
                hpSlider.value = (float)currentData.currentHP / currentData.maxHP;
        }
        if (hpText != null)
        {
            hpText.text = $"{currentData.currentHP} / {currentData.maxHP}";
        }

        // 更新蓝条
        int currentMpInt = Mathf.FloorToInt(currentData.currentMP);
        int maxMpInt = Mathf.FloorToInt(currentData.maxMP);
        if (mpSlider != null)
        {
            if (currentData.maxMP > 0)
                mpSlider.value = currentData.currentMP / currentData.maxMP;
        }
        if (mpText != null)
        {
            mpText.text = $"{currentMpInt} / {maxMpInt}";
        }
    }
}