// MainMenuController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("���˵���ť")]
    public Button startGameButton;
    public Button tutorialButton;
    public Button settingsButton;
    public Button quitGameButton;

    [Header("�������")]
    public GameObject tutorialPanel;
    public GameObject settingsPanel;

    [Header("����ڹرհ�ť")]
    public Button closeTutorialButton;
    public Button closeSettingsButton;

    [Header("��������")]
    public string gameSceneName = "Map";

    void Start()
    {
        // �����˵���ť
        startGameButton?.onClick.AddListener(StartGame);
        tutorialButton?.onClick.AddListener(OpenTutorial);
        settingsButton?.onClick.AddListener(OpenSettings);
        quitGameButton?.onClick.AddListener(QuitGame);

        // �����رհ�ť
        closeTutorialButton?.onClick.AddListener(CloseTutorial);
        closeSettingsButton?.onClick.AddListener(CloseSettings);

        // ��ʼ�ر��������
        CloseAllPanels();
    }

    #region ���˵�����
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

    #region ��忪��
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