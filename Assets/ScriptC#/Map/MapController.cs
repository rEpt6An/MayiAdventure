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

    // *** BUG FIX: ���һ��ȫ��״̬������ֹ����������ƶ� ***
    private bool isPlayerActionInProgress = false;

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
        // *** BUG FIX: �ڴ�����ǰ���ȼ��״̬�� ***
        if (isPlayerActionInProgress)
        {
            return; // ������ڴ���һ���ж�������������µĵ��
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

    private void HandleNodeClicked(MapNode clickedNode)
    {
        // *** BUG FIX: �ٴμ��״̬������Ϊ˫�ر��� ***
        if (isPlayerActionInProgress || currentNode == null || !IsMoveValid(clickedNode)) return;

        int totalFoodCost = foodCostPerMove;
        if (clickedNode.isCompleted)
        {
            totalFoodCost += extraFoodCostForVisited;
        }

        // �����Դ�Ƿ��㹻
        bool canPayWithFood = playerData.Food >= totalFoodCost;
        bool canPayWithHealth = playerData.currentHP > totalFoodCost; // ע���Ǵ��ڣ�������ھ�����

        if (canPayWithFood)
        {
            // *** BUG FIX: ������Э��ǰ����������״̬ ***
            isPlayerActionInProgress = true;
            StartCoroutine(MoveAndProcessNode(clickedNode, totalFoodCost, 0));
        }
        else if (canPayWithHealth)
        {
            Debug.Log($"ʳ�ﲻ�㣬������ֵ���㡣��������ֵ�ƶ���");
            // *** BUG FIX: ������Э��ǰ����������״̬ ***
            isPlayerActionInProgress = true;
            StartCoroutine(MoveAndProcessNode(clickedNode, 0, totalFoodCost));
        }
        else
        {
            Debug.Log($"ʳ�������ֵ�����㣬�޷��ƶ���");
            // ���������ﴥ��һ��UI��ʾ
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
        // *** BUG FIX: ʹ��try...finallyȷ��״̬��һ���ᱻ�ͷ� ***
        try
        {
            // ��Դ���ĺ��ƶ��߼�
            if (foodToConsume > 0) playerData.Food -= foodToConsume;
            if (healthToConsume > 0) playerData.currentHP -= healthToConsume;

            currentNode.isCompleted = true;
            mapView.UpdateSingleNodeVisual(currentNode);
            yield return playerMarker.MoveTo(mapView.GetNodePosition(targetNode));
            currentNode = targetNode;
            sessionData.playerMapPosition = currentNode.position;
            mapView.HighlightCurrentNode(currentNode);

            // ���Ž��붯��
            NodeUI targetNodeUI = mapView.GetNodeUI(targetNode);
            if (targetNodeUI != null)
            {
                yield return targetNodeUI.PlayEnterAnimation();
            }

            // ����ڵ��¼�
            ProcessNodeType(targetNode);
        }
        finally
        {
            // *** BUG FIX: ����Э����ν�����������ɻ��򳡾��л����жϣ��������� ***
            // ���ProcessNodeType�л��˳������������ᱻ���٣�����Ȼ��ʧ��
            // ���û�л����������̵꣩��������������ֶ�������������һ���ж���
            isPlayerActionInProgress = false;
        }
    }

    private void ProcessNodeType(MapNode node)
    {
        // ����ڵ�֮ǰ�Ѿ���ɣ����Ҳ����̵����ֿ����ظ�����ģ���ֱ�ӷ���
        if (node.isCompleted && node.type != NodeType.Shop) return;

        // ��ǽڵ�Ϊ�����
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
                    // ��MapGenerator�������Ѿ�ȷ�����յ����Bossս��������Լ�
                    sessionData.isFinalBossBattle = (node.type == NodeType.EliteBattle && node.position == new Vector2Int(mapWidth - 1, mapHeight - 1));
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
            case NodeType.End:
                Debug.Log("�����յ㡣");
                break;
        }
    }
}