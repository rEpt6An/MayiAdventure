// EnemyPool.cs

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Pool", menuName = "RPG/Enemy Pool")]
public class EnemyPool : ScriptableObject
{
    [Header("���˳�")]
    [Tooltip("������������п��ܳ��ֵĵ��� (PlayerData)")]
    public List<PlayerData> possibleEnemies;

    /// <summary>
    /// �ӳ��������ȡһ������
    /// </summary>
    public PlayerData GetRandomEnemy()
    {
        if (possibleEnemies == null || possibleEnemies.Count == 0)
        {
            Debug.LogWarning("���˳�Ϊ�գ�");
            return null;
        }
        return possibleEnemies[Random.Range(0, possibleEnemies.Count)];
    }
}