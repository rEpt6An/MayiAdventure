// RewardUI.cs

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUI : MonoBehaviour
{
    [Header("UI Ԫ��")]
    public Image iconImage;
    public TextMeshProUGUI descriptionText;
    public Button takeButton;

    private Reward currentRewardData;
    private PlayerData playerData; // �������ȫ�ֵ� runtimePlayerData
    private Action<RewardUI, LootGroup> onRewardTakenCallback;
    private LootGroup sourceGroup;

    // *** ���ĸĶ�: Setup ���ڽ���һ�� Reward ���ʵ�� ***
    public void Setup(Reward rewardData, Sprite icon, string description, PlayerData targetPlayerData, LootGroup ownerGroup, Action<RewardUI, LootGroup> onRewardTaken)
    {
        currentRewardData = rewardData;
        playerData = targetPlayerData;
        sourceGroup = ownerGroup;
        onRewardTakenCallback = onRewardTaken;

        if (iconImage != null) iconImage.sprite = icon;
        if (descriptionText != null) descriptionText.text = description;

        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(HandleTakeButtonClick);
    }

    private void HandleTakeButtonClick()
    {
        ApplyReward();
        onRewardTakenCallback?.Invoke(this, sourceGroup);
        Destroy(gameObject);
    }

    private void ApplyReward()
    {
        if (playerData == null || currentRewardData == null) return;

        // *** ���ĸĶ�: ���ݽ�������ִ�в�ͬ���� ***
        if (currentRewardData.type == RewardType.Item)
        {
            // �������Ʒ������ֱ����ӵ�����
            if (currentRewardData.itemData != null)
            {
                playerData.inventory.AddItem(currentRewardData.itemData, currentRewardData.itemQuantity);
                Debug.Log($"�������Ʒ: {currentRewardData.itemData.name} x{currentRewardData.itemQuantity}");
            }
        }
        else
        {
            // ��������Խ���������ת��ΪItemEffect��Ӧ��
            ItemEffect effectToApply = new ItemEffect
            {
                // ��RewardTypeӳ�䵽StatType
                stat = (StatType)Enum.Parse(typeof(StatType), currentRewardData.type.ToString()),
                value = currentRewardData.amount
            };
            playerData.ApplyEffect(effectToApply);
            Debug.Log($"��ȡ�����Խ���: {effectToApply.stat} +{effectToApply.value}");
        }
    }
}

