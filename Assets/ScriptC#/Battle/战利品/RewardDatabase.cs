// RewardDatabase.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 引入Linq，方便查询

// 这个嵌套类定义了每种奖励的静态信息（图标、描述等）
[System.Serializable]
public class RewardInfo
{
    public RewardType type;
    public Sprite icon;
    [TextArea] // 让描述在Inspector里可以多行编辑
    public string defaultDescription = "{amount} an"; // {amount} 是一个占位符
}


[CreateAssetMenu(fileName = "RewardDatabase", menuName = "RPG/Reward Database")]
public class RewardDatabase : ScriptableObject
{
    // 在Inspector里，我们将在这里配置所有类型的奖励
    public List<RewardInfo> allRewardInfos;

    /// <summary>
    /// 根据奖励类型查找对应的配置信息
    /// </summary>
    public RewardInfo GetRewardInfo(RewardType type)
    {
        // 使用Linq查找匹配的第一个元素
        return allRewardInfos.FirstOrDefault(info => info.type == type);
    }
}