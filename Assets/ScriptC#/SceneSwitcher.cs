// SceneSwitcher.cs (修改后)

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 这个脚本现在是一个简单的场景组件，不再需要单例

    /// <summary>
    /// 公共方法，用于切换到指定名字的场景
    /// </summary>
    public void SwitchScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("目标场景名称为空！");
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"场景 '{sceneName}' 未添加到 Build Settings 中！");
            return;
        }

        // --- *** 核心新增逻辑 *** ---
        // 在真正切换场景之前，进行准备工作
        PrepareForSceneSwitch(sceneName);

        Debug.Log($"正在加载场景: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 在场景切换前执行的准备逻辑。
    /// 目前主要用于处理音频。
    /// </summary>
    private void PrepareForSceneSwitch(string nextSceneName)
    {
        // 根据下一个场景的名称来决定是否需要暂停BGM
        switch (nextSceneName)
        {
            case "BattleScene": // 请替换为你的战斗场景的确切名称
                Debug.Log("即将进入战斗场景，暂停当前BGM。");
                AudioManager.Instance.PauseBGM();
                break;

                // 你可以为其他需要特殊处理的场景添加case
                // case "CutsceneScene":
                //     AudioManager.Instance.StopBGM(); // 比如进入过场动画时完全停止音乐
                //     break;
        }
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            // System.IO.Path.GetFileNameWithoutExtension可以安全地从路径中提取出不带后缀的文件名
            string sceneInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (string.Equals(sceneInBuild, sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}