// MapGenerator.cs

using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("内容池引用")]
    public EnemyPool level1Enemies;
    public EnemyPool level2Enemies;
    public EnemyPool level3Enemies;
    public EnemyPool level4Enemies;
    public EnemyPool level5Enemies_Boss; // 专门为Boss战准备的池子

    public Dictionary<Vector2Int, MapNode> GenerateMap(int width, int height)
    {
        Dictionary<Vector2Int, MapNode> generatedMap = new Dictionary<Vector2Int, MapNode>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                generatedMap.Add(new Vector2Int(x, y), new MapNode(x, y));
            }
        }

        Vector2Int startPos = new Vector2Int(0, 0);
        Vector2Int endPos = new Vector2Int(width - 1, height - 1);

        if (generatedMap.ContainsKey(startPos)) generatedMap[startPos].type = NodeType.Start;

        // 我们不再把终点设为End，而是直接设为Boss战，这样更容易处理
        if (generatedMap.ContainsKey(endPos))
        {
            MapNode bossNode = generatedMap[endPos];
            bossNode.type = NodeType.EliteBattle; // 使用精英战斗类型来代表Boss战
            bossNode.contentData = level5Enemies_Boss?.GetRandomEnemy();
            if (bossNode.contentData == null)
            {
                Debug.LogError("无法为终点生成Boss：Level 5 Boss池为空或未设置！");
            }
        }

        PlaceSpecialShops(generatedMap, endPos);
        GeneratePath(generatedMap, startPos, endPos, width, height);
        FillRemainingNodes(generatedMap, width, height);

        Debug.Log("区域化地图已生成，终点为Boss战！");
        return generatedMap;
    }
    private void PlaceSpecialShops(Dictionary<Vector2Int, MapNode> mapData, Vector2Int endPos)
    {
        Vector2Int shopPos1 = endPos + Vector2Int.left;
        Vector2Int shopPos2 = endPos + Vector2Int.down;

        // 确保不会覆盖掉终点本身
        if (mapData.ContainsKey(shopPos1) && mapData[shopPos1].type == NodeType.Empty)
        {
            mapData[shopPos1].type = NodeType.Shop;
        }
        if (mapData.ContainsKey(shopPos2) && mapData[shopPos2].type == NodeType.Empty)
        {
            mapData[shopPos2].type = NodeType.Shop;
        }
    }

    private void GeneratePath(Dictionary<Vector2Int, MapNode> mapData, Vector2Int start, Vector2Int end, int width, int height)
    {
        Vector2Int current = start;
        int safetyBreak = 0;
        while (current != end && safetyBreak < (width * height * 2))
        {
            if (mapData.ContainsKey(current) && mapData[current].type == NodeType.Empty)
            {
                mapData[current].type = NodeType.Battle;
            }
            List<Vector2Int> possibleMoves = new List<Vector2Int>();
            if (current.x < end.x) possibleMoves.Add(current + Vector2Int.right);
            if (current.y < end.y) possibleMoves.Add(current + Vector2Int.up);
            if (possibleMoves.Count == 0)
            {
                if (current.x > 0 && mapData.ContainsKey(current + Vector2Int.left)) possibleMoves.Add(current + Vector2Int.left);
                if (current.y > 0 && mapData.ContainsKey(current + Vector2Int.down)) possibleMoves.Add(current + Vector2Int.down);
                if (possibleMoves.Count == 0)
                {
                    if (current.x + 1 < width) possibleMoves.Add(current + Vector2Int.right);
                    else if (current.y + 1 < height) possibleMoves.Add(current + Vector2Int.up);
                    else break;
                }
            }
            current = possibleMoves[Random.Range(0, possibleMoves.Count)];
            if (!mapData.ContainsKey(current)) break;
            safetyBreak++;
        }
    }

    private void FillRemainingNodes(Dictionary<Vector2Int, MapNode> mapData, int width, int height)
    {
        foreach (var node in mapData.Values)
        {
            if (node.type == NodeType.Empty || node.type == NodeType.Battle) // Battle是路径上的，也需要分配等级
            {
                // 如果是路径上的，强制为战斗
                if (node.type != NodeType.Battle)
                {
                    float rand = Random.value;
                    if (rand < 0.65f) node.type = NodeType.Battle;
                    else if (rand < 0.90f) node.type = NodeType.Event;
                    else node.type = NodeType.Shop;
                }

                if (node.type == NodeType.Battle)
                {
                    node.contentData = GetEnemyPoolForNode(node, width, height)?.GetRandomEnemy();
                }
            }
        }
    }

    // *** 核心改动: 重写等级区域划分逻辑 ***
    private EnemyPool GetEnemyPoolForNode(MapNode node, int width, int height)
    {
        int x = node.position.x;
        int y = node.position.y;

        //新手池
        if (x <= 2 && y <= 2)
        {
            return level1Enemies;
        }

        // 规则2: 终点的3x3区域
        if (x >= width - 3 && y >= height-3)
        {
            return level4Enemies;
        }

        //6*6-3*3
        if (x <= 5 && y <= 5 && (x > 2 || y > 2))
        {
            return level2Enemies;
        }

        // 规则4: 其他所有区域都是Lv3
        return level3Enemies;
    }
}