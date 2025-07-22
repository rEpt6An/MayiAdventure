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
    public Transform slotsContainer;
    public GameObject slotPrefab;

    // *** ����: ȫ��Ψһ�ĸ�����ʹ�á���ť ***
    [Tooltip("����������干���Ψһһ����ʹ�á���ť")]
    public Button floatingUseButton;

    [Header("��ҳ��ť")]
    public Button allItemsButton;
    public Button equipmentButton;
    public Button consumablesButton;

    [Header("���ư�ť")]
    public Button closeButton;

    private List<GameObject> spawnedSlots = new List<GameObject>();
    private ItemType? currentFilter = null;
    private BagSlot currentSelectedSlot; // ��¼��ǰ�ĸ����ӱ�ѡ��

    void Awake()
    {
        if (runtimeBagData != null) runtimeBagData.OnInventoryChanged += Redraw;
        closeButton?.onClick.AddListener(ClosePanel);
        allItemsButton?.onClick.AddListener(() => SetFilterAndRedraw(null));
        equipmentButton?.onClick.AddListener(() => SetFilterAndRedraw(ItemType.Equipment));
        consumablesButton?.onClick.AddListener(() => SetFilterAndRedraw(ItemType.Consumable));

        // *** ����: ��ʼ��������ť ***
        floatingUseButton?.gameObject.SetActive(false);
        floatingUseButton?.onClick.AddListener(OnFloatingUseButtonClicked);
    }

    void OnEnable()
    {
        Redraw();
        // ÿ�δ���壬���ظ�����ť
        floatingUseButton?.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // �ر����ʱҲ���ظ�����ť���Է���һ
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

    // *** ����: SlotUI��������������������ʾ��ʹ�á���ť ***
    public void RequestShowUseButton(SlotUI clickedSlotUI, BagSlot slotData)
    {
        if (floatingUseButton == null) return;

        // ����������ͬһ����ѡ�еĸ��ӣ����л���ʾ״̬
        if (currentSelectedSlot == slotData && floatingUseButton.gameObject.activeSelf)
        {
            floatingUseButton.gameObject.SetActive(false);
            return;
        }

        // ���򣬸���Ŀ�겢��ʾ
        currentSelectedSlot = slotData;
        floatingUseButton.gameObject.SetActive(true);

        // *** �����޸ĵ������� ***
        // ��������ť�ƶ�����������ӵ�λ�ã���Ӧ��Y��ƫ��

        // ����1��ֱ���޸� position �� y ֵ
        Vector3 targetPosition = clickedSlotUI.transform.position;
        targetPosition.y -= 0.25f; // ע�⣺�������絥λ(World Units)��ƫ��
        floatingUseButton.transform.position = targetPosition;

        // --- ���ߣ�������ϲ�������ص�λ˼�������������� ---

        // ����2��ʹ�� RectTransform �� Canvas ���и���ȷ�����ؼ�ƫ�ƣ��Ƽ���
        // �������Canvas��Screen Space - Overlayģʽ
        RectTransform buttonRect = floatingUseButton.GetComponent<RectTransform>();
        RectTransform slotRect = clickedSlotUI.GetComponent<RectTransform>();

        // ��slot��ê��λ��ת��Ϊ�������꣬Ȼ�����ø���ť
        buttonRect.position = slotRect.position;

        // �ڱ�������ϵ�������ƶ������ֱ���޸�����������ɿ���������Ļ�ֱ���Ӱ�졣
        // y����-25���أ�����Ը��ݰ�ť�͸��ӵ�ʵ�ʴ�С�������ֵ��
        buttonRect.anchoredPosition += new Vector2(0, -25f);
    }
    private void OnFloatingUseButtonClicked()
    {
        if (currentSelectedSlot != null && runtimePlayerData != null)
        {
            currentSelectedSlot.item.Use(runtimePlayerData);
            runtimePlayerData.inventory.RemoveItem(currentSelectedSlot.item, 1);

            // ʹ�ú���Ʒ������ʧ���������ػ棬��ʱ������ťҲӦ����
            floatingUseButton.gameObject.SetActive(false);
            currentSelectedSlot = null; // ���ѡ��״̬
        }
    }

    private void SetFilterAndRedraw(ItemType? filter)
    {
        currentFilter = filter;
        floatingUseButton?.gameObject.SetActive(false); // �л�ɸѡʱ���ذ�ť
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

            // *** �޸�: Init��������ֻ��Ҫ����BagPanel�����PlayerData ***
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