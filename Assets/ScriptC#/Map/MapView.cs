// MapView.cs

using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [Header("通用预制件与容器")]
    public GameObject nodePrefab;
    public Transform container;

    [Header("地块尺寸与间距")]
    public float nodeSize = 1f;
    public float nodeSpacing = 0.2f;

    [System.Serializable]
    public class NodeTypeIcon { public NodeType type; public Sprite icon; }
    public List<NodeTypeIcon> nodeIcons; // 这个列表现在将包含所有类型的图标

    private Dictionary<NodeType, Sprite> iconMapping = new Dictionary<NodeType, Sprite>();
    private Dictionary<MapNode, NodeUI> nodeUIMap = new Dictionary<MapNode, NodeUI>();

    void Awake()
    {
        foreach (var item in nodeIcons)
        {
            if (!iconMapping.ContainsKey(item.type))
                iconMapping.Add(item.type, item.icon);
        }
    }

    public void DrawMap(Dictionary<Vector2Int, MapNode> mapData)
    {
        foreach (Transform child in container) Destroy(child.gameObject);
        nodeUIMap.Clear();

        if (mapData == null || nodePrefab == null) return;

        foreach (var pair in mapData)
        {
            MapNode node = pair.Value;
            if (node.type == NodeType.Empty) continue;

            GameObject nodeGO = Instantiate(nodePrefab, container);
            nodeGO.transform.position = GetNodePosition(node);

            NodeUI nodeUI = nodeGO.GetComponent<NodeUI>();
            if (nodeUI != null)
            {
                // *** 核心修复: 始终只查找并传递一个代表内容的图标 ***

                NodeType typeToLookup = node.type;

                // 如果是战斗，但我们想用一个通用的战斗图标，可以这样做
                // if(node.type == NodeType.Lv1Battle || node.type == NodeType.Lv2Battle ...)
                // {
                //     typeToLookup = NodeType.Battle;
                // }

                iconMapping.TryGetValue(typeToLookup, out Sprite iconToUse);

                nodeUI.Setup(node, iconToUse);
                nodeUIMap[node] = nodeUI;
            }
        }
    }

    public NodeUI GetNodeUI(MapNode node)
    {
        if (node != null && nodeUIMap.TryGetValue(node, out NodeUI ui)) return ui;
        return null;
    }

    public Vector3 GetNodePosition(MapNode node)
    {
        if (node == null) return Vector3.zero;
        float posX = node.position.x * (nodeSize + nodeSpacing);
        float posY = node.position.y * (nodeSize + nodeSpacing);
        return new Vector3(posX, posY, 0);
    }

    public void RedrawMap(Dictionary<Vector2Int, MapNode> mapData)
    {
        // 这是一个简化的重绘，只更新状态，不重新创建
        foreach (var pair in nodeUIMap)
        {
            pair.Value?.UpdateVisualState();
        }
    }

    public void HighlightCurrentNode(MapNode currentNode)
    {
        RedrawMap(null); // 调用简化的重绘
        if (currentNode != null && nodeUIMap.TryGetValue(currentNode, out NodeUI ui))
        {
            ui?.HighlightAsCurrent();
        }
    }

    public void UpdateSingleNodeVisual(MapNode node)
    {
        if (node != null && nodeUIMap.TryGetValue(node, out NodeUI ui))
        {
            ui?.UpdateVisualState();
        }
    }
}