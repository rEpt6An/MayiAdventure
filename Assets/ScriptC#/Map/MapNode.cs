// MapNode.cs

using UnityEngine;

public enum NodeType
{
    Empty,
    Start,
    End,
    Battle,
    EliteBattle,
    Shop,
    Event,
    RestSite
}

[System.Serializable]
public class MapNode
{
    public Vector2Int position;
    public NodeType type;

    [Tooltip("存储这个节点的具体内容，例如一个敌人的PlayerData")]
    public ScriptableObject contentData;

    public bool isVisible = false;
    public bool isCompleted = false;

    // *** 核心修复: 确保这个字段存在 ***
    [Tooltip("地块的难度等级，用于决定外观和遭遇")]
    public int level = 1; // 默认等级为1

    public MapNode(int x, int y, NodeType nodeType = NodeType.Empty)
    {
        position = new Vector2Int(x, y);
        type = nodeType;
    }
}