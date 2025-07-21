// BagPanel.cs

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : MonoBehaviour
{

    [Header("��������")]
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private Bag runtimeBagData;

    [Header("UI Ԫ��")]
    [Tooltip("������и���UI�ĸ�������Ҫ����Grid Layout Group")]
    public Transform slotsContainer;
    public GameObject slotPrefab;

    [Header("��ҳ��ť")]
    public Button allItemsButton;
    public Button equipmentButton;
    public Button consumablesButton;

    [Header("���ư�ť")]
    public Button closeButton;

    private List<GameObject> spawnedSlots = new List<GameObject>();
    private ItemType? currentFilter = null; // null ����ȫ����

    // Awake ���ڶ��󱻼���ʱ����OnEnable��Start֮ǰִ��
    void Awake()
    {
        if (runtimeBagData != null)
        {
            runtimeBagData.OnInventoryChanged += Redraw;
        }

        closeButton?.onClick.AddListener(ClosePanel);
        allItemsButton?.onClick.AddListener(() => SetFilterAndRedraw(null));
        equipmentButton?.onClick.AddListener(() => SetFilterAndRedraw(ItemType.Equipment));
        consumablesButton?.onClick.AddListener(() => SetFilterAndRedraw(ItemType.Consumable));
    }

    // OnEnable ���ڶ���ӷǼ���״̬��Ϊ����״̬ʱִ�е����ʱ��
    void OnEnable()
    {
        // ÿ����屻����ʱ�������»���һ�Σ�����ʾ��������
        Redraw();
    }

    void OnDestroy()
    {
        // ����ڶ�������ʱȡ�����ģ���ֹ�ڴ�й©
        if (runtimeBagData != null)
        {
            runtimeBagData.OnInventoryChanged -= Redraw;
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void SetFilterAndRedraw(ItemType? filter)
    {
        currentFilter = filter;
        Redraw();
    }

    /// <summary>
    /// ���ķ��������»���������������
    /// </summary>
    public void Redraw()
    {
        // ������δ���������Awake�׶ε���ʱ�����򲻽��л��ƣ���ʡ����
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        // 1. �������оɵĸ���
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot);
        }
        spawnedSlots.Clear();

        var itemsToShow = GetFilteredItems();

        foreach (var bagSlot in itemsToShow)
        {
            GameObject newSlotGO = Instantiate(slotPrefab, slotsContainer);
            SlotUI slotUI = newSlotGO.GetComponent<SlotUI>();
            // *** ���ĸĶ�: �����ø���ʱ�����������Ҳ����ȥ ***
            slotUI.Init(runtimePlayerData);
            slotUI.UpdateSlot(bagSlot);
            spawnedSlots.Add(newSlotGO);
        }
    }

    private List<BagSlot> GetFilteredItems()
    {
        if (runtimeBagData == null) return new List<BagSlot>();

        // ���ɸѡ��Ϊnull������������Ʒ
        if (currentFilter == null)
        {
            return runtimeBagData.slots;
        }

        // ����ʹ��Linq������Ʒ���ͽ���ɸѡ
        return runtimeBagData.slots.Where(slot => slot.item.type == currentFilter.Value).ToList();
    }
}