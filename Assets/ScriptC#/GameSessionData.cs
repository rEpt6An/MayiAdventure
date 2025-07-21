// GameSessionData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "GameSessionData", menuName = "RPG/Game Session Data")]
public class GameSessionData : ScriptableObject
{
    [Header("��ҵ�ͼ״̬")]
    public Vector2Int playerMapPosition;
    public bool isNewGame = true;

    [Header("ս��������Ϣ")]
    [Tooltip("��һ��Ҫ����ս���ĵ�������")]
    public PlayerData nextEnemy; // �� EncounterData ��Ϊ PlayerData

    [Header("ս��״̬")]
    public bool isFinalBossBattle = false;

    public void ResetSession(Vector2Int startPosition)
    {
        playerMapPosition = startPosition;
        isNewGame = true;
        nextEnemy = null;

        isFinalBossBattle = false; // ����Ϸʱ����

    }
}