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
    public Transform slotsContainer;
    public GameObject slotPrefab;

    // *** 新增: 全局唯一的浮动“使用”按钮 ***
    [Tooltip("整个背包面板共享的唯一一个“使用”按钮")]
    public Button floatingUseButton;

    [Header("分页按钮")]
    public Button allItemsButton;
    public Button equipmentButton;
    public Button consumablesButton;

    [Header("控制按钮")]
    public Button closeButton;

    private List<GameObject> spawnedSlots = new List<GameObject>();
    private ItemType? currentFilter = null;
    private BagSlot currentSelectedSlot; // 记录当前哪个格子被选中

    void Awake()
    {
        if (runtimeBagData != null) runtimeBagData.OnInventoryChanged += Redraw;
        closeButton?.onClick.AddListener(ClosePanel);
        allItemsButton?.onClick.AddListener(() => SetFilterAndRedraw(null));
        equipmentButton?.onClick.AddListener(() => SetFilterAndRedraw(ItemType.Equipment));
        consumablesButton?.onClick.AddListener(() => SetFilterAndRedraw(ItemType.Consumable));

        // *** 新增: 初始化浮动按钮 ***
        floatingUseButton?.gameObject.SetActive(false);
        floatingUseButton?.onClick.AddListener(OnFloatingUseButtonClicked);
    }

    void OnEnable()
    {
        Redraw();
        // 每次打开面板，隐藏浮动按钮
        floatingUseButton?.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // 关闭面板时也隐藏浮动按钮，以防万一
        floatingUseButton?.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (runtimeBagData != null) runtimeBagData.OnInventoryChanged -= Redraw;
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // *** 新增: SlotUI会调用这个方法来请求显示“使用”按钮 ***
    public void RequestShowUseButton(SlotUI clickedSlotUI, BagSlot slotData)
    {
        if (floatingUseButton == null) return;

        // 如果点击的是同一个已选中的格子，则切换显示状态
        if (currentSelectedSlot == slotData && floatingUseButton.gameObject.activeSelf)
        {
            floatingUseButton.gameObject.SetActive(false);
            return;
        }

        // 否则，更新目标并显示
        currentSelectedSlot = slotData;
        floatingUseButton.gameObject.SetActive(true);

        // *** 核心修改点在这里 ***
        // 将浮动按钮移动到被点击格子的位置，并应用Y轴偏移

        // 方法1：直接修改 position 的 y 值
        Vector3 targetPosition = clickedSlotUI.transform.position;
        targetPosition.y -= 0.25f; // 注意：这是世界单位(World Units)的偏移
        floatingUseButton.transform.position = targetPosition;

        // --- 或者，如果你更喜欢用像素单位思考，可以这样做 ---

        // 方法2：使用 RectTransform 和 Canvas 进行更精确的像素级偏移（推荐）
        // 假设你的Canvas是Screen Space - Overlay模式
        RectTransform buttonRect = floatingUseButton.GetComponent<RectTransform>();
        RectTransform slotRect = clickedSlotUI.GetComponent<RectTransform>();

        // 将slot的锚点位置转换为世界坐标，然后设置给按钮
        buttonRect.position = slotRect.position;

        // 在本地坐标系下向下移动。这比直接修改世界坐标更可靠，不受屏幕分辨率影响。
        // y坐标-25像素，你可以根据按钮和格子的实际大小调整这个值。
        buttonRect.anchoredPosition += new Vector2(0, -25f);
    }
    private void OnFloatingUseButtonClicked()
    {
        if (currentSelectedSlot != null && runtimePlayerData != null)
        {
            currentSelectedSlot.item.Use(runtimePlayerData);
            runtimePlayerData.inventory.RemoveItem(currentSelectedSlot.item, 1);

            // 使用后，物品可能消失，背包会重绘，此时浮动按钮也应隐藏
            floatingUseButton.gameObject.SetActive(false);
            currentSelectedSlot = null; // 清除选中状态
        }
    }

    private void SetFilterAndRedraw(ItemType? filter)
    {
        currentFilter = filter;
        floatingUseButton?.gameObject.SetActive(false); // 切换筛选时隐藏按钮
        Redraw();
    }

    public void Redraw()
    {
        if (!gameObject.activeInHierarchy) return;

        foreach (var slot in spawnedSlots) Destroy(slot);
        spawnedSlots.Clear();

        var itemsToShow = GetFilteredItems();

        foreach (var bagSlot in itemsToShow)
        {
            GameObject newSlotGO = Instantiate(slotPrefab, slotsContainer);
            SlotUI slotUI = newSlotGO.GetComponent<SlotUI>();

            // *** 修改: Init方法现在只需要传递BagPanel自身和PlayerData ***
            slotUI.Init(runtimePlayerData, this);

            slotUI.UpdateSlot(bagSlot);
            spawnedSlots.Add(newSlotGO);
        }
    }

    private List<BagSlot> GetFilteredItems()
    {
        if (runtimeBagData == null) return new List<BagSlot>();
        if (currentFilter == null) return runtimeBagData.slots;
        return runtimeBagData.slots.Where(slot => slot.item.type == currentFilter.Value).ToList();
    }
}