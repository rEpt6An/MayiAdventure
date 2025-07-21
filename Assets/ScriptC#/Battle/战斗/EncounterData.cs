// EncounterData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "RPG/Encounter Data")]
public class EncounterData : ScriptableObject
{
    [Header("��������")]
    public string encounterName;

    [Tooltip("�ⳡ1v1ս���л���ֵĵ��� (PlayerData����)")]
    public PlayerData enemyData;
}