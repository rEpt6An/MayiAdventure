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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        Debug.Log($"[SceneMusicManager] 场景 '{sceneName}' 加载完成，请求切换BGM。");

        switch (sceneName)
        {
            case "Map":
                AudioManager.Instance.PlayMapBGM();
                break;
            case "BattleScene":
                AudioManager.Instance.PlayBattleBGM();
                break;


            case "MainMenu": // 请替换成你的主菜单场景的确切名称
                AudioManager.Instance.PlayMainMenuBGM();
                break;

            default:
                // 如果是未定义的场景，可以选择停止音乐或什么都不做
                // AudioManager.Instance.StopBGM();
                Debug.LogWarning($"场景 '{sceneName}' 没有配置对应的BGM。");
                break;
        }
    }
}