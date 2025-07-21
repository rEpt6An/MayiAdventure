// SceneSwitcher.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // ����ű�������һ���򵥵ĳ��������������Ҫ����

    /// <summary>
    /// ���������������л���ָ�����ֵĳ���
    /// </summary>
    public void SwitchScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (IsSceneInBuildSettings(sceneName))
            {
                Debug.Log($"���ڼ��س���: {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"���� '{sceneName}' δ��ӵ� Build Settings �У�");
            }
        }
        else
        {
            Debug.LogError("Ŀ�곡������Ϊ�գ�");
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