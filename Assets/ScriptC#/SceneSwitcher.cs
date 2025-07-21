// SceneSwitcher.cs

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
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (IsSceneInBuildSettings(sceneName))
            {
                Debug.Log($"正在加载场景: {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"场景 '{sceneName}' 未添加到 Build Settings 中！");
            }
        }
        else
        {
            Debug.LogError("目标场景名称为空！");
        }
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string scene = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (string.Equals(scene, sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}