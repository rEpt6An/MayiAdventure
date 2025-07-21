// MapController.cs

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class MapController : MonoBehaviour
{
    [Header("�����������")]
    public MapGenerator mapGenerator;
    public MapView mapView;
    public PlayerMarker playerMarker;
    public PlayerData playerData;

    [Header("��ͼ����")]
    public int mapWidth = 10;
    public int mapHeight = 10;

    [Header("�����������")]
    public UI_MapManager uiMapManager;
    public SceneSwitcher sceneSwitcher;
    public GameSessionData sessionData;

    [Header("��Ϸ�߼�")]
    public int foodCostPerMove = 10;
    public int extraFoodCostForVisited = 5;

    private static Dictionary<Vector2Int, MapNode> persistentMapData = new Dictionary<Vector2Int, MapNode>();
    private MapNode currentNode;

    void Awake()
    {
        // ����߼���Update����
    }

    void Start()
    {
        InitializeMapState();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                NodeUI clickedNodeUI = hit.collider.GetComponent<NodeUI>();
                if (clickedNodeUI != null)
                {
                    HandleNodeClicked(clickedNodeUI.GetNode());
                }
            }
        }
    }

    private void InitializeMapState()
    {
        if (sessionData.isNewGame || persistentMapData.Count == 0)
        {
            persistentMapData = mapGenerator.GenerateMap(mapWidth, mapHeight);
            currentNode = persistentMapData.Values.FirstOrDefault(node => node.type == NodeType.Start);
            if (currentNode == null) { Debug.LogError("�Ҳ�����㣡"); return; }
            currentNode.isCompleted = true;
            sessionData.playerMapPosition = currentNode.position;
            sessionData.isNewGame = false;
        }
        else
        {
            if (!persistentMapData.TryGetValue(sessionData.playerMapPosition, out currentNode))
            {
                Debug.LogError($"�޷��ָ����λ��: {sessionData.playerMapPosition}��������������...");
                InitializeMapState();
                return;
            }
        }
        mapView.DrawMap(persistentMapData);
        playerMarker.transform.position = mapView.GetNodePosition(currentNode);
        mapView.HighlightCurrentNode(currentNode);
    }

    // *** ���ĸĶ�: �ع� HandleNodeClicked �Դ���ʳ��/����ֵ���� ***
    private void HandleNodeClicked(MapNode clickedNode)
    {
        if (currentNode == null || !IsMoveValid(clickedNode)) return;

        int totalFoodCost = foodCostPerMove;
        if (clickedNode.isCompleted)
        {
            totalFoodCost += extraFoodCostForVisited;
        }

        if (playerData.Food < totalFoodCost)
        {
            Debug.Log($"ʳ�ﲻ�㣬��Ҫ {totalFoodCost}����ֻ�� {playerData.Food}��");
            if (playerData.currentHP > totalFoodCost)
            {
                Debug.Log($"����ֵ���� ({playerData.currentHP})��׼���ƶ���");
                StartCoroutine(MoveAndProcessNode(clickedNode, 0, totalFoodCost));
            }
            else
            {
                Debug.Log($"����ֵ���� ({playerData.currentHP})���޷��ƶ���");
                return;
            }
        }
        else
        {
            StartCoroutine(MoveAndProcessNode(clickedNode, totalFoodCost, 0));
        }
    }
    private bool IsMoveValid(MapNode targetNode)
    {
        if (targetNode == currentNode) return false;
        Vector2Int distance = targetNode.position - currentNode.position;
        return Mathf.Abs(distance.x) + Mathf.Abs(distance.y) == 1;
    }

    // *** ���ĸĶ�: MoveAndProcessNode ���ڽ����������Ĳ��� ***
    private IEnumerator MoveAndProcessNode(MapNode targetNode, int foodToConsume, int healthToConsume)
    {
        if (foodToConsume > 0) playerData.Food -= foodToConsume;
        if (healthToConsume > 0) playerData.currentHP -= healthToConsume;

        currentNode.isCompleted = true;
        mapView.UpdateSingleNodeVisual(currentNode);
        yield return playerMarker.MoveTo(mapView.GetNodePosition(targetNode));
        currentNode = targetNode;
        sessionData.playerMapPosition = currentNode.position;
        mapView.HighlightCurrentNode(currentNode);
        NodeUI targetNodeUI = mapView.GetNodeUI(targetNode);
        if (targetNodeUI != null)
        {
            yield return targetNodeUI.PlayEnterAnimation();
        }
        ProcessNodeType(targetNode);
    }
    private void ProcessNodeType(MapNode node)
    {
        if (node.isCompleted) return;

            if (!node.isCompleted)
        {
            node.isCompleted = true;
        }

        switch (node.type)
        {
            case NodeType.Battle:
            case NodeType.EliteBattle:
                // *** �����޸�: ʹ��һ����ͬ�ı���������������ת���Ľ�� ***
                if (node.contentData is PlayerData enemyData)
                {
                    // ����鵽�ĵ������ݸ�ֵ���Ự
                    sessionData.nextEnemy = enemyData;

                    // ����Ƿ����յ�
                    bool isFinalBoss = (node.position.x == mapWidth - 1 && node.position.y == mapHeight - 1);
                    sessionData.isFinalBossBattle = isFinalBoss;

                    // �л�����
                    sceneSwitcher.SwitchScene("BattleScene");
                }
                else
                {
                    Debug.LogWarning($"�ڵ���ս�����ͣ���û�з�����Ч�ĵ������ݡ�λ��: {node.position}");
                }
                break;
            case NodeType.Shop:
                uiMapManager?.ToggleShopPanel();
                break;
            case NodeType.Event:
                EventManager.Instance?.StartRandomEvent();
                break;
            case NodeType.End: // ����֮ǰ���߼�����Bossս������End�����������case�Ա�����
                Debug.Log("���������ϵ��յ㣨����������Bossս����");
                break;
        }
    }
}