// MapGenerator.cs

using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("���ݳ�����")]
    public EnemyPool level1Enemies;
    public EnemyPool level2Enemies;
    public EnemyPool level3Enemies;
    public EnemyPool level4Enemies;
    public EnemyPool level5Enemies_Boss; // ר��ΪBossս׼���ĳ���

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

        // ���ǲ��ٰ��յ���ΪEnd������ֱ����ΪBossս�����������״���
        if (generatedMap.ContainsKey(endPos))
        {
            MapNode bossNode = generatedMap[endPos];
            bossNode.type = NodeType.EliteBattle; // ʹ�þ�Ӣս������������Bossս
            bossNode.contentData = level5Enemies_Boss?.GetRandomEnemy();
            if (bossNode.contentData == null)
            {
                Debug.LogError("�޷�Ϊ�յ�����Boss��Level 5 Boss��Ϊ�ջ�δ���ã�");
            }
        }

        PlaceSpecialShops(generatedMap, endPos);
        GeneratePath(generatedMap, startPos, endPos, width, height);
        FillRemainingNodes(generatedMap, width, height);

        Debug.Log("���򻯵�ͼ�����ɣ��յ�ΪBossս��");
        return generatedMap;
    }
    private void PlaceSpecialShops(Dictionary<Vector2Int, MapNode> mapData, Vector2Int endPos)
    {
        Vector2Int shopPos1 = endPos + Vector2Int.left;
        Vector2Int shopPos2 = endPos + Vector2Int.down;

        // ȷ�����Ḳ�ǵ��յ㱾��
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
            if (node.type == NodeType.Empty || node.type == NodeType.Battle) // Battle��·���ϵģ�Ҳ��Ҫ����ȼ�
            {
                // �����·���ϵģ�ǿ��Ϊս��
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

    // *** ���ĸĶ�: ��д�ȼ����򻮷��߼� ***
    private EnemyPool GetEnemyPoolForNode(MapNode node, int width, int height)
    {
        int x = node.position.x;
        int y = node.position.y;

        //���ֳ�
        if (x <= 2 && y <= 2)
        {
            return level1Enemies;
        }

        // ����2: �յ��3x3����
        if (x >= width - 3 && y >= height-3)
        {
            return level4Enemies;
        }

        //6*6-3*3
        if (x <= 5 && y <= 5 && (x > 2 || y > 2))
        {
            return level2Enemies;
        }

        // ����4: ��������������Lv3
        return level3Enemies;
    }
}