// EnemyPool.cs

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Pool", menuName = "RPG/Enemy Pool")]
public class EnemyPool : ScriptableObject
{
    [Header("敌人池")]
    [Tooltip("这个池子里所有可能出现的敌人 (PlayerData)")]
    public List<PlayerData> possibleEnemies;

    /// <summary>
    /// 从池中随机获取一个敌人
    /// </summary>
    public PlayerData GetRandomEnemy()
    {
        if (possibleEnemies == null || possibleEnemies.Count == 0)
        {
            Debug.LogWarning("敌人池为空！");
            return null;
        }
        return possibleEnemies[Random.Range(0, possibleEnemies.Count)];
    }
}