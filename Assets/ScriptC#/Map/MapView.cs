// MapView.cs

using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [Header("ͨ��Ԥ�Ƽ�������")]
    public GameObject nodePrefab;
    public Transform container;

    [Header("�ؿ�ߴ�����")]
    public float nodeSize = 1f;
    public float nodeSpacing = 0.2f;

    [System.Serializable]
    public class NodeTypeIcon { public NodeType type; public Sprite icon; }
    public List<NodeTypeIcon> nodeIcons; // ����б����ڽ������������͵�ͼ��

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
                // *** �����޸�: ʼ��ֻ���Ҳ�����һ���������ݵ�ͼ�� ***

                NodeType typeToLookup = node.type;

                // �����ս��������������һ��ͨ�õ�ս��ͼ�꣬����������
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
        // ����һ���򻯵��ػ棬ֻ����״̬�������´���
        foreach (var pair in nodeUIMap)
        {
            pair.Value?.UpdateVisualState();
        }
    }

    public void HighlightCurrentNode(MapNode currentNode)
    {
        RedrawMap(null); // ���ü򻯵��ػ�
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