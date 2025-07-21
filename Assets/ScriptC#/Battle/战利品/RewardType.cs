// RewardType.cs

// 这个文件只包含枚举定义，方便其他脚本引用

public enum RewardType
{
    // --- 属性奖励 ---
    Gold,
    Food,
    CurrentHP, // 治疗
    MaxHP,
    CurrentMP,
    MaxMP,
    Attack,
    Defense,
    AttackSpeedPercent,
    CritChance,
    MissChance,
    SuckBlood,

    // --- 物品奖励 ---
    Item // 一个通用的“物品”类型
}