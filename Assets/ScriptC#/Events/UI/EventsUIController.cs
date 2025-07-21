// EventsUIController.cs

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 事件UI控制器
public class EventsUIController : MonoBehaviour
{
    public static EventsUIController Instance;

    [SerializeField] private GameObject UI_pannel;


    [Header("UI组件")]
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button[] actionButtons = new Button[4];
    [SerializeField] private TextMeshProUGUI[] buttonTexts = new TextMeshProUGUI[4];
    [SerializeField] private Image eventImage;

    // *** 新增：一个专门的关闭按钮 ***
    [Header("控制按钮")]
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeUI();
    }

    // 初始化UI并绑定事件
    private void InitializeUI()
    {





        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] != null && buttonTexts[i] == null)
            {
                buttonTexts[i] = actionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        // *** 新增：为关闭按钮添加监听事件 ***
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HideEvent); // 点击关闭按钮时，调用HideEvent方法
        }

        if (eventPanel != null)
        {
            eventPanel.SetActive(false); // 初始隐藏事件面板
        }

        RebindButtonEvents();
    }

    public void RebindButtonEvents()
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] == null) continue;
            actionButtons[i].onClick.RemoveAllListeners();
            int index = i;
            actionButtons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    // 显示事件
    public void DisplayEvent(GameEvent gameEvent)
    {
        //  隐藏UI
        if (UI_pannel != null)
        {
            UI_pannel.SetActive(false);
        }


        if (eventPanel != null) eventPanel.SetActive(true);
        if (displayText != null) displayText.text = gameEvent.eventDescription;

        if (eventImage != null)
        {


            eventImage.gameObject.SetActive(gameEvent.eventImage != null);
            if (gameEvent.eventImage != null)
            {
                eventImage.sprite = gameEvent.eventImage;
            }
        }

        // *** 新增：开始新事件时，隐藏关闭按钮 ***
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);

        }

        // 初始化按钮
        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] == null) continue;
            bool hasOption = i < gameEvent.buttonOptions.Length && !string.IsNullOrEmpty(gameEvent.buttonOptions[i]);
            actionButtons[i].gameObject.SetActive(hasOption);

            if (hasOption)
            {
                if (buttonTexts[i] != null) buttonTexts[i].text = gameEvent.buttonOptions[i];
                // *** 新增：确保选项按钮可以交互 ***
                actionButtons[i].interactable = true;
            }
        }
    }

    // 设置结果文本
    public void SetResultText(string resultText)
    {
        if (displayText != null)
        {
            displayText.text = resultText;
        }

        // *** 核心功能：禁用所有选项按钮 ***
        foreach (var button in actionButtons)
        {
            if (button != null) button.interactable = false;
        }

        // *** 核心功能：显示关闭按钮 ***
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(true);
        }
    }

    // 隐藏事件面板 (由关闭按钮调用)
    public void HideEvent()
    {
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
        }

        //显示UI
        if(UI_pannel != null)
        {
            UI_pannel.SetActive(true);
        }
    }

    // 按钮点击事件处理
    public void OnButtonClick(int buttonIndex)
    {
        if (EventManager.Instance == null)
        {
            Debug.LogError("EventManager instance is missing!");
            return;
        }
        EventManager.Instance.OnOptionSelected(buttonIndex);
    }
}