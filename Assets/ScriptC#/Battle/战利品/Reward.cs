// Reward.cs

// RewardType 枚举现在在它自己的文件里

public class Reward
{
    public RewardType type;

    // 对于属性奖励
    public int amount;

    // 对于物品奖励
    public ItemData itemData;
    public int itemQuantity;
}