// ShopPanel.cs

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 引入TextMeshPro的命名空间

public class ShopPanel : MonoBehaviour
{
    [Header("数据引用")]
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private Bag runtimeBagData;
    [SerializeField] private ShopPool shopPool;

    [Header("UI 元素")]
    [Tooltip("将场景中预先创建好的4个商品格子UI拖到这里")]
    public List<ShopSlotUI> shopSlots;

    [Header("控制按钮")]
    public Button refreshButton;
    public Button closeButton;

    // *** 新增: 刷新按钮的成本文本 ***
    [Tooltip("用于显示刷新费用的TextMeshProUGUI组件")]
    public TextMeshProUGUI refreshCostText;

    [Header("商店设置")]
    [Tooltip("初始的刷新费用")]
    public int initialRefreshCost = 10;
    [Tooltip("每次刷新后，费用增加的数额")]
    public int refreshCostAdd = 10;

    // *** 新增: 用于追踪当前刷新费用的变量 ***
    private int currentRefreshCost;


    void Start()
    {
        closeButton?.onClick.AddListener(ClosePanel);
        refreshButton?.onClick.AddListener(RefreshShop);
    }

    void OnEnable()
    {
        // *** 核心改动 1: 每次打开商店时，重置刷新费用 ***
        // 这样可以避免上次商店的递增费用带到下一次。
        // 如果你希望费用是全局累积的，可以把这行代码移到 Start() 中。
        currentRefreshCost = initialRefreshCost;

        // 每次打开商店时，都重新生成商品并更新UI
        PopulateShop();
        UpdateRefreshButtonUI();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void RefreshShop()
    {
        // 检查刷新花费
        if (runtimePlayerData.Gold < currentRefreshCost)
        {
            Debug.Log("金币不足，无法刷新商店！");
            // 你可以在这里加一个提示UI，比如让刷新成本文本闪烁红色
            return;
        }

        // 扣除金币
        runtimePlayerData.Gold -= currentRefreshCost;

        // *** 核心改动 2: 增加刷新费用 ***
        currentRefreshCost += refreshCostAdd;

        // 重新生成商品
        PopulateShop();

        // *** 核心改动 3: 更新刷新按钮的UI显示 ***
        UpdateRefreshButtonUI();
    }

    // *** 新增: 一个专门用来更新刷新按钮状态和文本的方法 ***
    private void UpdateRefreshButtonUI()
    {
        // 更新文本显示
        if (refreshCostText != null)
        {
            refreshCostText.text = currentRefreshCost.ToString();
        }

        // 根据玩家金币更新刷新按钮的可交互状态
        if (refreshButton != null)
        {
            refreshButton.interactable = (runtimePlayerData.Gold >= currentRefreshCost);
        }
    }

    private void PopulateShop()
    {
        // ... (这部分逻辑完全不变) ...
        if (shopPool == null || shopPool.availableItems.Count == 0 || shopSlots.Count == 0)
        {
            foreach (var slot in shopSlots) slot.gameObject.SetActive(false);
            return;
        }
        List<ItemData> itemsToDisplay = new List<ItemData>();
        List<ItemData> tempPool = new List<ItemData>(shopPool.availableItems);
        int count = Mathf.Min(shopSlots.Count, tempPool.Count);
        for (int i = 0; i < count; i++)
        {
            if (tempPool.Count == 0) break;
            int randomIndex = Random.Range(0, tempPool.Count);
            itemsToDisplay.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex);
        }
        for (int i = 0; i < shopSlots.Count; i++)
        {
            if (i < itemsToDisplay.Count)
            {
                shopSlots[i].gameObject.SetActive(true);
                shopSlots[i].Setup(itemsToDisplay[i], runtimePlayerData, HandlePurchase);
            }
            else
            {
                shopSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void HandlePurchase(ItemData boughtItem, ShopSlotUI boughtSlotUI)
    {
        runtimePlayerData.Gold -= boughtItem.price;
        runtimeBagData.AddItem(boughtItem);
        boughtSlotUI.gameObject.SetActive(false);

        // 每次购买后，金币减少了，所以需要检查并更新所有UI
        UpdateAllSlotsAffordability();
        UpdateRefreshButtonUI();
    }

    // *** 新增: 一个辅助方法，用于在金币变化后更新所有UI ***
    private void UpdateAllSlotsAffordability()
    {
        foreach (var slot in shopSlots)
        {
            if (slot.gameObject.activeSelf)
            {
                slot.UpdateAffordability(runtimePlayerData.Gold);
            }
        }
    }
}