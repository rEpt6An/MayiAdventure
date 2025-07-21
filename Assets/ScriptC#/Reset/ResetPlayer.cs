// ResetPlayer.cs

using System;
using UnityEngine;
using UnityEngine.SceneManagement; // 最好也引入场景管理

public class ResetPlayer : MonoBehaviour
{
    public static ResetPlayer Instance { get; private set; }

    [Header("核心数据文件")]
    [SerializeField] private PlayerData playerTemplate;
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private GameSessionData runtimeSessionData;

    // *** 新增: 引用装备属性缓存文件 ***
    [Header("装备属性缓存 (用于清空)")]
    [SerializeField] private PlayerData currentEquipmentStats;
    [SerializeField] private PlayerData lastEquipmentStats;

    public static event Action OnPlayerDataReset;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 这个方法应该在游戏完全启动时，只调用一次
    // 比如在一个启动场景的 GameInitializer 脚本的 Start() 中调用
    public void InitializeGameOnFirstLaunch()
    {
        StartNewGame(); // 首次启动等同于开始一个新游戏
    }


    /// <summary>
    /// 当玩家在主菜单点击“新游戏”时调用
    /// </summary>
    public void StartNewGame()
    {
        Debug.Log("ResetPlayer: 开始新游戏流程...");

        // 1. 重置玩家属性
        if (playerTemplate != null && runtimePlayerData != null)
        {
            runtimePlayerData.CopyFrom(playerTemplate, true); // 使用完全复制
            Debug.Log("  - 运行时玩家数据已从模板重置。");
        }

        // 2. 重置会话数据
        if (runtimeSessionData != null)
        {
            // isNewGame 设为 true，让 MapController 知道要重新生成地图
            runtimeSessionData.isNewGame = true;
            // 清空上一次的战斗遭遇信息
            runtimeSessionData.nextEnemy = null;
            Debug.Log("  - 游戏会话数据已重置。");
        }

        // 3. *** 核心修复: 清空装备属性缓存 ***
        if (currentEquipmentStats != null)
        {
            currentEquipmentStats.ClearAllBattleStats();
            Debug.Log("  - 'CurrentEquipmentStats' 缓存已清空。");
        }
        if (lastEquipmentStats != null)
        {
            lastEquipmentStats.ClearAllBattleStats();
            Debug.Log("  - 'LastEquipmentStats' 缓存已清空。");
        }

        // 4. 发出“游戏已重置”的广播，通知其他系统（如EquipmentManager）进行初始化
        OnPlayerDataReset?.Invoke();

        Debug.Log("ResetPlayer: 新游戏数据准备就绪！");

        // 5. (可选) 直接在这里切换到地图场景
        // SceneManager.LoadScene("Map");
    }
}