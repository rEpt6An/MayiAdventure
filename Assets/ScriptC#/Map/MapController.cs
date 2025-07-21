// MapController.cs

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class MapController : MonoBehaviour
{
    [Header("核心组件引用")]
    public MapGenerator mapGenerator;
    public MapView mapView;
    public PlayerMarker playerMarker;
    public PlayerData playerData;

    [Header("地图配置")]
    public int mapWidth = 10;
    public int mapHeight = 10;

    [Header("内容与控制器")]
    public UI_MapManager uiMapManager;
    public SceneSwitcher sceneSwitcher;
    public GameSessionData sessionData;

    [Header("游戏逻辑")]
    public int foodCostPerMove = 10;
    public int extraFoodCostForVisited = 5;

    private static Dictionary<Vector2Int, MapNode> persistentMapData = new Dictionary<Vector2Int, MapNode>();
    private MapNode currentNode;

    void Awake()
    {
        // 点击逻辑由Update处理
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
            if (currentNode == null) { Debug.LogError("找不到起点！"); return; }
            currentNode.isCompleted = true;
            sessionData.playerMapPosition = currentNode.position;
            sessionData.isNewGame = false;
        }
        else
        {
            if (!persistentMapData.TryGetValue(sessionData.playerMapPosition, out currentNode))
            {
                Debug.LogError($"无法恢复玩家位置: {sessionData.playerMapPosition}。正在重新生成...");
                InitializeMapState();
                return;
            }
        }
        mapView.DrawMap(persistentMapData);
        playerMarker.transform.position = mapView.GetNodePosition(currentNode);
        mapView.HighlightCurrentNode(currentNode);
    }

    // *** 核心改动: 重构 HandleNodeClicked 以处理食物/生命值消耗 ***
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
            Debug.Log($"食物不足，需要 {totalFoodCost}，但只有 {playerData.Food}。");
            if (playerData.currentHP > totalFoodCost)
            {
                Debug.Log($"生命值充足 ({playerData.currentHP})，准备移动。");
                StartCoroutine(MoveAndProcessNode(clickedNode, 0, totalFoodCost));
            }
            else
            {
                Debug.Log($"生命值不足 ({playerData.currentHP})，无法移动！");
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

    // *** 核心改动: MoveAndProcessNode 现在接收两种消耗参数 ***
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
                // *** 核心修复: 使用一个不同的变量名来接收类型转换的结果 ***
                if (node.contentData is PlayerData enemyData)
                {
                    // 将检查到的敌人数据赋值给会话
                    sessionData.nextEnemy = enemyData;

                    // 检查是否是终点
                    bool isFinalBoss = (node.position.x == mapWidth - 1 && node.position.y == mapHeight - 1);
                    sessionData.isFinalBossBattle = isFinalBoss;

                    // 切换场景
                    sceneSwitcher.SwitchScene("BattleScene");
                }
                else
                {
                    Debug.LogWarning($"节点是战斗类型，但没有分配有效的敌人数据。位置: {node.position}");
                }
                break;
            case NodeType.Shop:
                uiMapManager?.ToggleShopPanel();
                break;
            case NodeType.Event:
                EventManager.Instance?.StartRandomEvent();
                break;
            case NodeType.End: // 我们之前的逻辑是用Boss战代替了End，但保留这个case以备后用
                Debug.Log("到达名义上的终点（但它现在是Boss战）。");
                break;
        }
    }
}