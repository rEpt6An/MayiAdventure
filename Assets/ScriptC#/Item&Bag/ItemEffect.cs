// ItemEffect.cs

// 我们不再需要 using UnityEngine; 因为这里没有MonoBehaviour

// 定义一个枚举，包含所有可能被物品影响的属性
public enum StatType
{
    CurrentHP,
    MaxHP,
    CurrentMP,
    MaxMP,
    Attack,
    Defense,
    AttackSpeedPercent,
    CritChance,
    MissChance,
    SuckBlood,
    Food,
    Gold
}

// 定义一个效果的数据结构
[System.Serializable] // 确保它能在Inspector中显示
public struct ItemEffect
{
    public StatType stat; // 要修改的属性类型
    public int value;     // 修改的数值（对于百分比，10代表10%）
}