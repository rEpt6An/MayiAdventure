using UnityEngine;
using UnityEngine.UI;

public class EventTester : MonoBehaviour
{
    void Start()
    {
        // �������԰�ť
        GameObject testButton = new GameObject("TestButton");
        testButton.transform.SetParent(FindObjectOfType<Canvas>().transform);
        
        // ��Ӱ�ť���
        Button button = testButton.AddComponent<Button>();
        RectTransform rect = testButton.AddComponent<RectTransform>();
        
        // ����λ�úʹ�С
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(200, 50);
        
        // ����ı�
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(testButton.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = "�����¼�";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        // ��ӵ���¼�
        button.onClick.AddListener(StartTestEvent);
    }
    
    private void StartTestEvent()
    {
        Debug.Log("���԰�ť�����");
        
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartEvent("character_question");
        }
        else
        {
            Debug.LogError("EventManagerʵ��δ�ҵ�!");
        }
    }
}