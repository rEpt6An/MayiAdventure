// SlotUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 我们现在要求Button和TooltipTrigger都必须存在
[RequireComponent(typeof(Button), typeof(TooltipTrigger))]
public class SlotUI : MonoBehaviour
{
    [Header("UI 元素")]
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Button useButton;

    private BagSlot currentSlotData;
    private TooltipTrigger tooltipTrigger;
    private Button slotButton; // 缓存整个格子的按钮

    private PlayerData runtimePlayerData;

    void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
        slotButton = GetComponent<Button>();

        // *** 核心修复: 增加健壮性检查 ***
        if (tooltipTrigger == null)
        {
            Debug.LogError("SlotUI: 所在的GameObject上缺少 TooltipTrigger 组件！", this.gameObject);
        }
        if (slotButton == null)
        {
            Debug.LogError("SlotUI: 所在的GameObject上缺少 Button 组件！", this.gameObject);
        }

        useButton?.gameObject.SetActive(false);
        useButton?.onClick.AddListener(OnUseButtonClicked);
    }

    public void Init(PlayerData playerData)
    {
        this.runtimePlayerData = playerData;
    }

    public void UpdateSlot(BagSlot slotData)
    {
        currentSlotData = slotData;

        if (slotData == null || slotData.item == null)
        {
            icon.enabled = false;
            quantityText.enabled = false;
            tooltipTrigger?.SetDataProvider(null); // 安全调用
            slotButton?.onClick.RemoveAllListeners();
            return;
        }

        icon.enabled = true;
        icon.sprite = slotData.item.icon;
        quantityText.enabled = (slotData.quantity > 1);
        if (quantityText != null) quantityText.text = slotData.quantity.ToString();

        tooltipTrigger?.SetDataProvider(slotData.item); // 安全调用

        slotButton?.onClick.RemoveAllListeners();
        slotButton?.onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        if (currentSlotData?.item.type == ItemType.Consumable)
        {
            useButton?.gameObject.SetActive(!useButton.gameObject.activeSelf);
        }
    }

    private void OnUseButtonClicked()
    {
        if (currentSlotData != null && runtimePlayerData != null)
        {
            // 使用物品（调用ItemData内部的通用效果应用逻辑）
            currentSlotData.item.Use(runtimePlayerData);
            // 从背包中移除一个
            runtimePlayerData.inventory.RemoveItem(currentSlotData.item, 1);

            useButton?.gameObject.SetActive(false);
        }
    }
}