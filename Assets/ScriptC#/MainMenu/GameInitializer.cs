// GameInitializer.cs
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("管理器预制件")]
    public GameObject managersPrefab;   // 只挂 ResetPlayer 所在 prefab（见下方说明）

    void Awake()
    {
        if (ResetPlayer.Instance == null)
        {
            if (managersPrefab != null)
            {
                Instantiate(managersPrefab);
                Debug.Log("GameInitializer：管理器已实例化");
            }
            else
            {
                Debug.LogError("GameInitializer：managersPrefab 未赋值！");
            }
        }
    }
}