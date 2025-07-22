// SlotUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button), typeof(TooltipTrigger))]
public class SlotUI : MonoBehaviour
{
    [Header("UI 元素")]
    public Image icon;
    public TextMeshProUGUI quantityText;

    private BagSlot currentSlotData;
    private TooltipTrigger tooltipTrigger;
    private Button slotButton;

    // 不再需要直接引用PlayerData，所有操作通过BagPanel进行
    private BagPanel parentBagPanel;

    void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
        slotButton = GetComponent<Button>();

        if (tooltipTrigger == null) Debug.LogError("SlotUI: 缺少 TooltipTrigger 组件！", this.gameObject);
        if (slotButton == null) Debug.LogError("SlotUI: 缺少 Button 组件！", this.gameObject);
    }

    // *** 修改: Init方法现在只需要一个对BagPanel的引用 ***
    public void Init(PlayerData playerData, BagPanel bagPanel) // PlayerData可以去掉，但保留以防万一
    {
        this.parentBagPanel = bagPanel;
    }

    public void UpdateSlot(BagSlot slotData)
    {
        currentSlotData = slotData;

        if (slotData == null || slotData.item == null)
        {
            icon.enabled = false;
            quantityText.enabled = false;
            tooltipTrigger?.SetDataProvider(null);
            slotButton?.onClick.RemoveAllListeners();
            return;
        }

        icon.enabled = true;
        icon.sprite = slotData.item.icon;
        quantityText.enabled = (slotData.quantity > 1);
        if (quantityText != null) quantityText.text = slotData.quantity.ToString();

        tooltipTrigger?.SetDataProvider(slotData.item);

        slotButton?.onClick.RemoveAllListeners();
        slotButton?.onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        if (currentSlotData?.item.type == ItemType.Consumable)
        {
            // *** 核心逻辑: 只需通知父面板 ***
            parentBagPanel?.RequestShowUseButton(this, currentSlotData);
        }
        else
        {
            // 如果点击的是非消耗品，可以考虑隐藏浮动按钮
            // parentBagPanel?.floatingUseButton.SetActive(false);
        }
    }

    // 不再需要 useButton 和相关的本地方法了
}