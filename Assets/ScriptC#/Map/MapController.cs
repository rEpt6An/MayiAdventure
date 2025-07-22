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

    // *** BUG FIX: 添加一个全局状态锁来防止连续点击和移动 ***
    private bool isPlayerActionInProgress = false;

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
        // *** BUG FIX: 在处理点击前，先检查状态锁 ***
        if (isPlayerActionInProgress)
        {
            return; // 如果正在处理一个行动，则忽略所有新的点击
        }

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

    private void HandleNodeClicked(MapNode clickedNode)
    {
        // *** BUG FIX: 再次检查状态锁，作为双重保险 ***
        if (isPlayerActionInProgress || currentNode == null || !IsMoveValid(clickedNode)) return;

        int totalFoodCost = foodCostPerMove;
        if (clickedNode.isCompleted)
        {
            totalFoodCost += extraFoodCostForVisited;
        }

        // 检查资源是否足够
        bool canPayWithFood = playerData.Food >= totalFoodCost;
        bool canPayWithHealth = playerData.currentHP > totalFoodCost; // 注意是大于，如果等于就死了

        if (canPayWithFood)
        {
            // *** BUG FIX: 在启动协程前，立刻锁上状态 ***
            isPlayerActionInProgress = true;
            StartCoroutine(MoveAndProcessNode(clickedNode, totalFoodCost, 0));
        }
        else if (canPayWithHealth)
        {
            Debug.Log($"食物不足，但生命值充足。消耗生命值移动。");
            // *** BUG FIX: 在启动协程前，立刻锁上状态 ***
            isPlayerActionInProgress = true;
            StartCoroutine(MoveAndProcessNode(clickedNode, 0, totalFoodCost));
        }
        else
        {
            Debug.Log($"食物和生命值都不足，无法移动！");
            // 可以在这里触发一个UI提示
            return;
        }
    }

    private bool IsMoveValid(MapNode targetNode)
    {
        if (targetNode == currentNode) return false;
        Vector2Int distance = targetNode.position - currentNode.position;
        return Mathf.Abs(distance.x) + Mathf.Abs(distance.y) == 1;
    }

    private IEnumerator MoveAndProcessNode(MapNode targetNode, int foodToConsume, int healthToConsume)
    {
        // *** BUG FIX: 使用try...finally确保状态锁一定会被释放 ***
        try
        {
            // 资源消耗和移动逻辑
            if (foodToConsume > 0) playerData.Food -= foodToConsume;
            if (healthToConsume > 0) playerData.currentHP -= healthToConsume;

            currentNode.isCompleted = true;
            mapView.UpdateSingleNodeVisual(currentNode);
            yield return playerMarker.MoveTo(mapView.GetNodePosition(targetNode));
            currentNode = targetNode;
            sessionData.playerMapPosition = currentNode.position;
            mapView.HighlightCurrentNode(currentNode);

            // 播放进入动画
            NodeUI targetNodeUI = mapView.GetNodeUI(targetNode);
            if (targetNodeUI != null)
            {
                yield return targetNodeUI.PlayEnterAnimation();
            }

            // 处理节点事件
            ProcessNodeType(targetNode);
        }
        finally
        {
            // *** BUG FIX: 无论协程如何结束（正常完成或因场景切换而中断），都解锁 ***
            // 如果ProcessNodeType切换了场景，这个对象会被销毁，锁自然消失。
            // 如果没切换场景（如商店），则必须在这里手动解锁以允许下一次行动。
            isPlayerActionInProgress = false;
        }
    }

    private void ProcessNodeType(MapNode node)
    {
        // 如果节点之前已经完成，并且不是商店这种可以重复进入的，就直接返回
        if (node.isCompleted && node.type != NodeType.Shop) return;

        // 标记节点为已完成
        if (!node.isCompleted)
        {
            node.isCompleted = true;
        }

        switch (node.type)
        {
            case NodeType.Battle:
            case NodeType.EliteBattle:
                if (node.contentData is PlayerData enemyData)
                {
                    sessionData.nextEnemy = enemyData;
                    // 在MapGenerator中我们已经确保了终点就是Boss战，这里可以简化
                    sessionData.isFinalBossBattle = (node.type == NodeType.EliteBattle && node.position == new Vector2Int(mapWidth - 1, mapHeight - 1));
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
            case NodeType.End:
                Debug.Log("到达终点。");
                break;
        }
    }
}