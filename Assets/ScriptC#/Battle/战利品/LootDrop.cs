// LootDrop.cs

using UnityEngine;

[System.Serializable]
public class LootDrop
{
    [Header("奖励类型与概率")]
    [Tooltip("定义这个掉落是属性奖励还是物品奖励")]
    public RewardType type;

    [Tooltip("掉落这个奖励的概率（0-100）")]
    [Range(0, 100)]
    public float dropChance = 100f;

    // --- 属性奖励专用字段 ---
    [Header("属性奖励设置 (如果类型不是Item)")]
    [Tooltip("掉落数量的最小值")]
    public int minAmount = 1;
    [Tooltip("掉落数量的最大值")]
    public int maxAmount = 1;

    // --- 物品奖励专用字段 ---
    [Header("物品奖励设置 (如果类型是Item)")]
    [Tooltip("直接拖入一个ItemData资产文件作为奖励")]
    public ItemData specificItem;
    [Tooltip("掉落这个物品的数量")]
    public int itemQuantity = 1;

    [Header("自定义显示")]
    [Tooltip("留空则使用数据库中的默认描述。{amount} 或 {itemName} 会被替换。")]
    [TextArea]
    public string overrideDescription;
}