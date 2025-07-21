using UnityEngine;
using UnityEngine.UI;

public class EventTester : MonoBehaviour
{
    void Start()
    {
        // 创建测试按钮
        GameObject testButton = new GameObject("TestButton");
        testButton.transform.SetParent(FindObjectOfType<Canvas>().transform);
        
        // 添加按钮组件
        Button button = testButton.AddComponent<Button>();
        RectTransform rect = testButton.AddComponent<RectTransform>();
        
        // 设置位置和大小
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(200, 50);
        
        // 添加文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(testButton.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = "测试事件";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        // 添加点击事件
        button.onClick.AddListener(StartTestEvent);
    }
    
    private void StartTestEvent()
    {
        Debug.Log("测试按钮被点击");
        
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartEvent("character_question");
        }
        else
        {
            Debug.LogError("EventManager实例未找到!");
        }
    }
}