// SlotUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ��������Ҫ��Button��TooltipTrigger���������
[RequireComponent(typeof(Button), typeof(TooltipTrigger))]
public class SlotUI : MonoBehaviour
{
    [Header("UI Ԫ��")]
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Button useButton;

    private BagSlot currentSlotData;
    private TooltipTrigger tooltipTrigger;
    private Button slotButton; // �����������ӵİ�ť

    private PlayerData runtimePlayerData;

    void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
        slotButton = GetComponent<Button>();

        // *** �����޸�: ���ӽ�׳�Լ�� ***
        if (tooltipTrigger == null)
        {
            Debug.LogError("SlotUI: ���ڵ�GameObject��ȱ�� TooltipTrigger �����", this.gameObject);
        }
        if (slotButton == null)
        {
            Debug.LogError("SlotUI: ���ڵ�GameObject��ȱ�� Button �����", this.gameObject);
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
            tooltipTrigger?.SetDataProvider(null); // ��ȫ����
            slotButton?.onClick.RemoveAllListeners();
            return;
        }

        icon.enabled = true;
        icon.sprite = slotData.item.icon;
        quantityText.enabled = (slotData.quantity > 1);
        if (quantityText != null) quantityText.text = slotData.quantity.ToString();

        tooltipTrigger?.SetDataProvider(slotData.item); // ��ȫ����

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
            // ʹ����Ʒ������ItemData�ڲ���ͨ��Ч��Ӧ���߼���
            currentSlotData.item.Use(runtimePlayerData);
            // �ӱ������Ƴ�һ��
            runtimePlayerData.inventory.RemoveItem(currentSlotData.item, 1);

            useButton?.gameObject.SetActive(false);
        }
    }
}