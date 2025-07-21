// ItemData.cs

using System.Collections.Generic;
using System.Text; // 引入StringBuilder，高效拼接字符串
using UnityEngine;

// --- 定义一些有用的枚举 ---

public enum ItemType
{
    Equipment, // 装备
    Consumable // 消耗品
}

public enum ItemRarity
{
    Common,    // 普通
    Rare,      // 稀有
    Epic,      // 史诗
    Legendary,  // 传说

    myth  //神话
}
[CreateAssetMenu(fileName = "New Item", menuName = "RPG/Item Data")]
public class ItemData : ScriptableObject, ITooltipDataProvider
{
    [Header("基本信息")]
    public string itemName;
    public Sprite icon;
    [TextArea(3, 10)]
    public string description;
    public int price = 10;

    [Header("分类与稀有度")]
    public ItemType type;
    public ItemRarity rarity;
    public bool isStackable = true;
    public int maxStackSize = 99;

    // *** 核心改动: 使用一个效果列表来定义所有属性加成 ***
    [Header("属性效果")]
    [Tooltip("定义物品的所有效果，无论是装备的永久加成，还是消耗品的一次性效果")]
    public List<ItemEffect> effects;

    // Use方法现在将遍历效果列表并应用它们
    public virtual void Use(PlayerData playerData)
    {
        if (type != ItemType.Consumable) return; // 只有消耗品才能被“使用”

        foreach (var effect in effects)
        {
            playerData.ApplyEffect(effect);
        }
        Debug.Log($"使用了: {itemName}");
    }

    // Tooltip现在也会动态地从效果列表中生成
    public string GetTooltipHeader() { return itemName; }

    public string GetTooltipContent()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(description).AppendLine();

        if (effects.Count > 0)
        {
            sb.AppendLine("<b><color=orange>效果:</color></b>");
            foreach (var effect in effects)
            {
                // 将效果转换为人类可读的文本
                sb.AppendLine(FormatEffect(effect));
            }
        }
        return sb.ToString();
    }

    // 一个辅助方法，用来格式化效果的描述
    private string FormatEffect(ItemEffect effect)
    {
        string prefix = effect.value > 0 ? "+" : "";
        switch (effect.stat)
        {
            case StatType.CurrentHP: return $"恢复 {effect.value} 生命";
            case StatType.MaxHP: return $"最大生命 {prefix}{effect.value}";
            case StatType.Attack: return $"攻击力 {prefix}{effect.value}";
            case StatType.AttackSpeedPercent: return $"攻击速度 {prefix}{effect.value}%";
            case StatType.Defense: return $"防御力 {prefix}{effect.value}";


            case StatType.CritChance: return $"暴击率 {prefix}{effect.value}%";
            case StatType.MissChance: return $"闪避率 {prefix}{effect.value}%";
            case StatType.SuckBlood: return $"<color=red>吸血 {prefix}{effect.value}%</color>";


            default: return $"{effect.stat.ToString()} {prefix}{effect.value}";
        }
    }
}