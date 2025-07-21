// BagPanel.cs

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : MonoBehaviour
{

    [Header("数据引用")]
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private Bag runtimeBagData;

    [Header("UI 元素")]
    [Tooltip("存放所有格子UI的父对象，需要挂载Grid Layout Group")]
    public Transform slotsContainer;
    public GameObject slotPrefab;

    [Header("分页按钮")]
    public Button allItemsButton;
    public Button equipmentButton;
    public Button consumablesButton;

    [Header("控制按钮")]
    public Button closeButton;

    private List<GameObject> spawnedSlots = new List<GameObject>();
    private ItemType? currentFilter = null; // null 代表“全部”

    // Awake 会在对象被激活时，在OnEnable和Start之前执行
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

    // OnEnable 是在对象从非激活状态变为激活状态时执行的最佳时机
    void OnEnable()
    {
        // 每次面板被激活时，都重新绘制一次，以显示最新内容
        Redraw();
    }

    void OnDestroy()
    {
        // 务必在对象销毁时取消订阅，防止内存泄漏
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
    /// 核心方法：重新绘制整个背包界面
    /// </summary>
    public void Redraw()
    {
        // 如果面板未激活（比如在Awake阶段调用时），则不进行绘制，节省性能
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        // 1. 销毁所有旧的格子
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
            // *** 核心改动: 在设置格子时，把玩家数据也传进去 ***
            slotUI.Init(runtimePlayerData);
            slotUI.UpdateSlot(bagSlot);
            spawnedSlots.Add(newSlotGO);
        }
    }

    private List<BagSlot> GetFilteredItems()
    {
        if (runtimeBagData == null) return new List<BagSlot>();

        // 如果筛选器为null，返回所有物品
        if (currentFilter == null)
        {
            return runtimeBagData.slots;
        }

        // 否则，使用Linq根据物品类型进行筛选
        return runtimeBagData.slots.Where(slot => slot.item.type == currentFilter.Value).ToList();
    }
}