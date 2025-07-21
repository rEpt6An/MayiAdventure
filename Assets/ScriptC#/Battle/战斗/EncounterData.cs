// EncounterData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "RPG/Encounter Data")]
public class EncounterData : ScriptableObject
{
    [Header("遭遇配置")]
    public string encounterName;

    [Tooltip("这场1v1战斗中会出现的敌人 (PlayerData类型)")]
    public PlayerData enemyData;
}