using System.Collections.Generic;
using UnityEngine;

public class DamagePopupPool : MonoBehaviour
{
    public static DamagePopupPool Instance;
    public GameObject popupPrefab;
    public int initialPoolSize = 20;

    private Queue<DamagePopup> pool = new Queue<DamagePopup>();

    void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 初始化对象池
        if (popupPrefab != null)
        {
            InitializePool();
        }
        else
        {
            Debug.LogError("Popup prefab is not assigned!");
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPopup();
        }
    }

    private void CreateNewPopup()
    {
        if (popupPrefab == null) return;

        GameObject obj = Instantiate(popupPrefab);
        obj.SetActive(false);

        DamagePopup popup = obj.GetComponent<DamagePopup>();
        if (popup == null)
        {
            Debug.LogError("Prefab is missing DamagePopup component!", obj);
            Destroy(obj);
            return;
        }

        pool.Enqueue(popup);
    }

    public DamagePopup GetFromPool()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool is empty, creating new instance");
            CreateNewPopup();
        }

        // 安全获取
        if (pool.Count > 0)
        {
            DamagePopup popup = pool.Dequeue();
            popup.gameObject.SetActive(true);
            return popup;
        }

        return null;
    }

    public void ReturnToPool(DamagePopup popup)
    {
        if (popup == null) return;

        popup.gameObject.SetActive(false);
        pool.Enqueue(popup);
    }
}