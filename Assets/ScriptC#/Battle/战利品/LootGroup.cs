// LootGroup.cs

using UnityEngine;
using System.Collections.Generic;

public enum LootGroupRule
{
    // 规则1: 掉落组内所有满足概率的物品，玩家可以全部拾取
    TakeAll,
    // 规则2: 掉落组内所有满足概率的物品，但玩家只能选择 N 个
    PickN,
    // 规则3: 从组内随机抽取 N 个物品作为最终掉落
    RollN
}

[System.Serializable]
public class LootGroup
{
    [Header("掉落组规则")]
    public LootGroupRule rule = LootGroupRule.TakeAll;

    [Tooltip("当规则为 PickN 或 RollN 时，这个数值生效。")]
    public int ruleAmount = 1;

    [Header("组内掉落项")]
    public List<LootDrop> drops;
}