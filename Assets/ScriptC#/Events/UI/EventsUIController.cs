// EventsUIController.cs

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �¼�UI������
public class EventsUIController : MonoBehaviour
{
    public static EventsUIController Instance;

    [SerializeField] private GameObject UI_pannel;


    [Header("UI���")]
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button[] actionButtons = new Button[4];
    [SerializeField] private TextMeshProUGUI[] buttonTexts = new TextMeshProUGUI[4];
    [SerializeField] private Image eventImage;

    // *** ������һ��ר�ŵĹرհ�ť ***
    [Header("���ư�ť")]
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

    // ��ʼ��UI�����¼�
    private void InitializeUI()
    {





        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] != null && buttonTexts[i] == null)
            {
                buttonTexts[i] = actionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        // *** ������Ϊ�رհ�ť��Ӽ����¼� ***
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HideEvent); // ����رհ�ťʱ������HideEvent����
        }

        if (eventPanel != null)
        {
            eventPanel.SetActive(false); // ��ʼ�����¼����
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

    // ��ʾ�¼�
    public void DisplayEvent(GameEvent gameEvent)
    {
        //  ����UI
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

        // *** ��������ʼ���¼�ʱ�����عرհ�ť ***
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);

        }

        // ��ʼ����ť
        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] == null) continue;
            bool hasOption = i < gameEvent.buttonOptions.Length && !string.IsNullOrEmpty(gameEvent.buttonOptions[i]);
            actionButtons[i].gameObject.SetActive(hasOption);

            if (hasOption)
            {
                if (buttonTexts[i] != null) buttonTexts[i].text = gameEvent.buttonOptions[i];
                // *** ������ȷ��ѡ�ť���Խ��� ***
                actionButtons[i].interactable = true;
            }
        }
    }

    // ���ý���ı�
    public void SetResultText(string resultText)
    {
        if (displayText != null)
        {
            displayText.text = resultText;
        }

        // *** ���Ĺ��ܣ���������ѡ�ť ***
        foreach (var button in actionButtons)
        {
            if (button != null) button.interactable = false;
        }

        // *** ���Ĺ��ܣ���ʾ�رհ�ť ***
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(true);
        }
    }

    // �����¼���� (�ɹرհ�ť����)
    public void HideEvent()
    {
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
        }

        //��ʾUI
        if(UI_pannel != null)
        {
            UI_pannel.SetActive(true);
        }
    }

    // ��ť����¼�����
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