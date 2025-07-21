// ResetPlayerButton.cs

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResetPlayerButton : MonoBehaviour
{
    private Button resetButton;

    void Start()
    {
        resetButton = GetComponent<Button>();
        resetButton.onClick.AddListener(HandleResetClick);
    }

    private void HandleResetClick()
    {
        if (ResetPlayer.Instance != null)
        {
            Debug.Log("调试按钮被点击，正在开始新游戏...");

            // *** 核心修复: 调用正确的方法名 ***
            // ResetPlayer.Instance.ResetPlayerData(); // 旧的、不存在的方法
            ResetPlayer.Instance.StartNewGame();   // 新的、正确的方法
        }
        else
        {
            Debug.LogError("无法找到ResetPlayer的实例！");
        }
    }
}