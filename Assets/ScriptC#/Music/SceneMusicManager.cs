// SceneMusicManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 这个脚本的唯一职责是：监听场景加载事件，并根据新场景的名称，
/// 指挥AudioManager播放正确的背景音乐。
/// 它应该和AudioManager一样，是一个持久化的单例。
/// </summary>
public class SceneMusicManager : MonoBehaviour
{
    // --- 单例模式 ---
    private static SceneMusicManager _instance;

    void Awake()
    {
        // 实现持久化单例
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // --- 核心逻辑：订阅场景加载事件 ---
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // 良好习惯：当对象被销毁时，取消订阅事件以防止内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 当一个新场景加载完成时，这个方法会被自动调用。
    /// </summary>
    /// <param name="scene">加载的场景信息</param>
    /// <param name="mode">加载模式 (Single, Additive)</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 使用场景名称来决定播放哪个BGM
        string sceneName = scene.name;

        Debug.Log($"场景 '{sceneName}' 加载完毕，准备切换BGM。");

        switch (sceneName)
        {
            case "MainMenu": // 请替换成你的主菜单场景的确切名称
                AudioManager.Instance.PlayMainMenuBGM();
                break;

            case "Map": // 请替换成你的地图场景的确切名称
                AudioManager.Instance.PlayMapBGM();
                break;

            case "BattleScene": // 请替换成你的战斗场景的确切名称
                // 进入战斗前，我们希望地图音乐是暂停的，而不是完全停止
                // 这样回到地图时才能继续。
                // 这个“暂停”的动作，最好由触发战斗的地方发起。
                AudioManager.Instance.PauseBGM(); // 暂停当前音乐（可能是地图音乐）
                AudioManager.Instance.PlayBattleBGM();
                break;

            // 你可以为其他任何场景添加BGM
            // case "ShopScene":
            //     AudioManager.Instance.PlayShopBGM();
            //     break;

            default:
                // 如果是未定义的场景，可以选择停止音乐或什么都不做
                // AudioManager.Instance.StopBGM();
                Debug.LogWarning($"场景 '{sceneName}' 没有配置对应的BGM。");
                break;
        }
    }
}