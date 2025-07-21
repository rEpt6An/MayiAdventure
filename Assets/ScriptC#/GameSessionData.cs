// GameSessionData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "GameSessionData", menuName = "RPG/Game Session Data")]
public class GameSessionData : ScriptableObject
{
    [Header("玩家地图状态")]
    public Vector2Int playerMapPosition;
    public bool isNewGame = true;

    [Header("战斗遭遇信息")]
    [Tooltip("下一个要进入战斗的敌人数据")]
    public PlayerData nextEnemy; // 从 EncounterData 改为 PlayerData

    [Header("战斗状态")]
    public bool isFinalBossBattle = false;

    public void ResetSession(Vector2Int startPosition)
    {
        playerMapPosition = startPosition;
        isNewGame = true;
        nextEnemy = null;

        isFinalBossBattle = false; // 新游戏时重置

    }
}