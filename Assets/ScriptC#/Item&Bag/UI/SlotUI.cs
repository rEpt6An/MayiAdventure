// SlotUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button), typeof(TooltipTrigger))]
public class SlotUI : MonoBehaviour
{
    [Header("UI Ԫ��")]
    public Image icon;
    public TextMeshProUGUI quantityText;

    private BagSlot currentSlotData;
    private TooltipTrigger tooltipTrigger;
    private Button slotButton;

    // ������Ҫֱ������PlayerData�����в���ͨ��BagPanel����
    private BagPanel parentBagPanel;

    void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
        slotButton = GetComponent<Button>();

        if (tooltipTrigger == null) Debug.LogError("SlotUI: ȱ�� TooltipTrigger �����", this.gameObject);
        if (slotButton == null) Debug.LogError("SlotUI: ȱ�� Button �����", this.gameObject);
    }

    // *** �޸�: Init��������ֻ��Ҫһ����BagPanel������ ***
    public void Init(PlayerData playerData, BagPanel bagPanel) // PlayerData����ȥ�����������Է���һ
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
            // *** �����߼�: ֻ��֪ͨ����� ***
            parentBagPanel?.RequestShowUseButton(this, currentSlotData);
        }
        else
        {
            // ���������Ƿ�����Ʒ�����Կ������ظ�����ť
            // parentBagPanel?.floatingUseButton.SetActive(false);
        }
    }

    // ������Ҫ useButton ����صı��ط�����
}