// GameInitializer.cs
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("������Ԥ�Ƽ�")]
    public GameObject managersPrefab;   // ֻ�� ResetPlayer ���� prefab�����·�˵����

    void Awake()
    {
        if (ResetPlayer.Instance == null)
        {
            if (managersPrefab != null)
            {
                Instantiate(managersPrefab);
                Debug.Log("GameInitializer����������ʵ����");
            }
            else
            {
                Debug.LogError("GameInitializer��managersPrefab δ��ֵ��");
            }
        }
    }
}