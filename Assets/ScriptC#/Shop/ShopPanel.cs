// ShopPanel.cs

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // ����TextMeshPro�������ռ�

public class ShopPanel : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private Bag runtimeBagData;
    [SerializeField] private ShopPool shopPool;

    [Header("UI Ԫ��")]
    [Tooltip("��������Ԥ�ȴ����õ�4����Ʒ����UI�ϵ�����")]
    public List<ShopSlotUI> shopSlots;

    [Header("���ư�ť")]
    public Button refreshButton;
    public Button closeButton;

    // *** ����: ˢ�°�ť�ĳɱ��ı� ***
    [Tooltip("������ʾˢ�·��õ�TextMeshProUGUI���")]
    public TextMeshProUGUI refreshCostText;

    [Header("�̵�����")]
    [Tooltip("��ʼ��ˢ�·���")]
    public int initialRefreshCost = 10;
    [Tooltip("ÿ��ˢ�º󣬷������ӵ�����")]
    public int refreshCostAdd = 10;

    // *** ����: ����׷�ٵ�ǰˢ�·��õı��� ***
    private int currentRefreshCost;


    void Start()
    {
        closeButton?.onClick.AddListener(ClosePanel);
        refreshButton?.onClick.AddListener(RefreshShop);
    }

    void OnEnable()
    {
        // *** ���ĸĶ� 1: ÿ�δ��̵�ʱ������ˢ�·��� ***
        // �������Ա����ϴ��̵�ĵ������ô�����һ�Ρ�
        // �����ϣ��������ȫ���ۻ��ģ����԰����д����Ƶ� Start() �С�
        currentRefreshCost = initialRefreshCost;

        // ÿ�δ��̵�ʱ��������������Ʒ������UI
        PopulateShop();
        UpdateRefreshButtonUI();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void RefreshShop()
    {
        // ���ˢ�»���
        if (runtimePlayerData.Gold < currentRefreshCost)
        {
            Debug.Log("��Ҳ��㣬�޷�ˢ���̵꣡");
            // ������������һ����ʾUI��������ˢ�³ɱ��ı���˸��ɫ
            return;
        }

        // �۳����
        runtimePlayerData.Gold -= currentRefreshCost;

        // *** ���ĸĶ� 2: ����ˢ�·��� ***
        currentRefreshCost += refreshCostAdd;

        // ����������Ʒ
        PopulateShop();

        // *** ���ĸĶ� 3: ����ˢ�°�ť��UI��ʾ ***
        UpdateRefreshButtonUI();
    }

    // *** ����: һ��ר����������ˢ�°�ť״̬���ı��ķ��� ***
    private void UpdateRefreshButtonUI()
    {
        // �����ı���ʾ
        if (refreshCostText != null)
        {
            refreshCostText.text = currentRefreshCost.ToString();
        }

        // ������ҽ�Ҹ���ˢ�°�ť�Ŀɽ���״̬
        if (refreshButton != null)
        {
            refreshButton.interactable = (runtimePlayerData.Gold >= currentRefreshCost);
        }
    }

    private void PopulateShop()
    {
        // ... (�ⲿ���߼���ȫ����) ...
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

        // ÿ�ι���󣬽�Ҽ����ˣ�������Ҫ��鲢��������UI
        UpdateAllSlotsAffordability();
        UpdateRefreshButtonUI();
    }

    // *** ����: һ�����������������ڽ�ұ仯���������UI ***
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