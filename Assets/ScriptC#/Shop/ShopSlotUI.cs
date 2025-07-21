// ShopSlotUI.cs

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TooltipTrigger))] // ͬ��ǿ����Ӵ�����
public class ShopSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private TooltipTrigger tooltipTrigger;
    private ItemData currentItem;
    private Action<ItemData, ShopSlotUI> onPurchaseCallback;

    void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
    }

    public void Setup(ItemData item, PlayerData playerData, Action<ItemData, ShopSlotUI> onPurchase)
    {
        currentItem = item;
        onPurchaseCallback = onPurchase;

        iconImage.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(HandleBuyClick);
        UpdateAffordability(playerData.Gold);

        // ����Ʒ���ݴ��ݸ�Tooltip������
        tooltipTrigger.SetDataProvider(currentItem);
    }

    private void HandleBuyClick()
    {
        onPurchaseCallback?.Invoke(currentItem, this);
    }

    public void UpdateAffordability(int playerGold)
    {
        buyButton.interactable = (playerGold >= currentItem.price);
    }
}