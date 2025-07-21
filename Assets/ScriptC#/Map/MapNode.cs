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

    [Tooltip("�洢����ڵ�ľ������ݣ�����һ�����˵�PlayerData")]
    public ScriptableObject contentData;

    public bool isVisible = false;
    public bool isCompleted = false;

    // *** �����޸�: ȷ������ֶδ��� ***
    [Tooltip("�ؿ���Ѷȵȼ������ھ�����ۺ�����")]
    public int level = 1; // Ĭ�ϵȼ�Ϊ1

    public MapNode(int x, int y, NodeType nodeType = NodeType.Empty)
    {
        position = new Vector2Int(x, y);
        type = nodeType;
    }
}