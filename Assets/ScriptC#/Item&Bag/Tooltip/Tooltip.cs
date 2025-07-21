// Tooltip.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class Tooltip : MonoBehaviour
{
    [Header("UI 元素")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;
    public LayoutElement layoutElement;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("Tooltip 脚本所在的GameObject上没有找到 CanvasGroup 组件！", this.gameObject);
            return;
        }
        Hide();
    }

    void Update()
    {
        if (canvasGroup.alpha > 0)
        {
            FollowMouseSmart();
        }
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerText.gameObject.SetActive(false);
        }
        else
        {
            headerText.gameObject.SetActive(true);
            headerText.text = header;
        }

        contentText.text = content;

        if (layoutElement != null)
        {
            int headerLength = headerText.text.Length;
            int contentLength = contentText.text.Length;
            layoutElement.enabled = (headerLength > 70 || contentLength > 70);
        }

        // 强制立即重建布局，以获取正确的尺寸用于位置计算
        if (gameObject.activeInHierarchy) // 只在激活时才重建
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }



    public void Show()
    {
        // *** 使用 CanvasGroup 来显示，而不是 SetActive(true) ***
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
    }

    private void FollowMouseSmart()
    {
        Vector2 mousePosition = Input.mousePosition;

        float pivotX = mousePosition.x / Screen.width;
        float pivotY = mousePosition.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = mousePosition;
    }
}