// RewardUI.cs

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUI : MonoBehaviour
{
    [Header("UI 元素")]
    public Image iconImage;
    public TextMeshProUGUI descriptionText;
    public Button takeButton;

    private Reward currentRewardData;
    private PlayerData playerData; // 这个将是全局的 runtimePlayerData
    private Action<RewardUI, LootGroup> onRewardTakenCallback;
    private LootGroup sourceGroup;

    // *** 核心改动: Setup 现在接收一个 Reward 类的实例 ***
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

        // *** 核心改动: 根据奖励类型执行不同操作 ***
        if (currentRewardData.type == RewardType.Item)
        {
            // 如果是物品奖励，直接添加到背包
            if (currentRewardData.itemData != null)
            {
                playerData.inventory.AddItem(currentRewardData.itemData, currentRewardData.itemQuantity);
                Debug.Log($"获得了物品: {currentRewardData.itemData.name} x{currentRewardData.itemQuantity}");
            }
        }
        else
        {
            // 如果是属性奖励，将其转换为ItemEffect并应用
            ItemEffect effectToApply = new ItemEffect
            {
                // 将RewardType映射到StatType
                stat = (StatType)Enum.Parse(typeof(StatType), currentRewardData.type.ToString()),
                value = currentRewardData.amount
            };
            playerData.ApplyEffect(effectToApply);
            Debug.Log($"领取了属性奖励: {effectToApply.stat} +{effectToApply.value}");
        }
    }
}

