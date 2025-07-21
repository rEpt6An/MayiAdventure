// MainMenuController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("主菜单按钮")]
    public Button startGameButton;
    public Button tutorialButton;
    public Button settingsButton;
    public Button quitGameButton;

    [Header("弹出面板")]
    public GameObject tutorialPanel;
    public GameObject settingsPanel;

    [Header("面板内关闭按钮")]
    public Button closeTutorialButton;
    public Button closeSettingsButton;

    [Header("场景设置")]
    public string gameSceneName = "Map";

    void Start()
    {
        // 绑定主菜单按钮
        startGameButton?.onClick.AddListener(StartGame);
        tutorialButton?.onClick.AddListener(OpenTutorial);
        settingsButton?.onClick.AddListener(OpenSettings);
        quitGameButton?.onClick.AddListener(QuitGame);

        // 绑定面板关闭按钮
        closeTutorialButton?.onClick.AddListener(CloseTutorial);
        closeSettingsButton?.onClick.AddListener(CloseSettings);

        // 初始关闭所有面板
        CloseAllPanels();
    }

    #region 主菜单功能
    public void StartGame()
    {
        ResetPlayer.Instance?.StartNewGame();
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region 面板开关
    public void OpenTutorial() => SetPanel(tutorialPanel, true);
    public void CloseTutorial() => SetPanel(tutorialPanel, false);

    public void OpenSettings() => SetPanel(settingsPanel, true);
    public void CloseSettings() => SetPanel(settingsPanel, false);

    private void CloseAllPanels()
    {
        SetPanel(tutorialPanel, false);
        SetPanel(settingsPanel, false);
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
    #endregion
}