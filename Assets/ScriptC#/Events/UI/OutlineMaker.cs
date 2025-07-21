using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class OutlineMaker : MonoBehaviour
{
    public Color outlineColor = Color.white;
    public int outlineSize = 3;

    private Image targetImage;
    private CanvasGroup canvasGroup;

    void Start()
    {
        targetImage = GetComponent<Image>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void OnEnable()
    {
        // ��ӱ߿�Ч��
        AddOutline();
    }

    void OnDisable()
    {
        // �Ƴ��߿�Ч��
        RemoveOutline();
    }

    void AddOutline()
    {
        // �����߿�
        for (int i = 0; i < outlineSize; i++)
        {
            GameObject outlineObj = new GameObject("Outline_" + i);
            outlineObj.transform.SetParent(transform.parent);
            outlineObj.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            outlineObj.AddComponent<RectTransform>();
            outlineObj.AddComponent<CanvasGroup>();
            Image outlineImage = outlineObj.AddComponent<Image>();
            outlineImage.sprite = targetImage.sprite;
            outlineImage.color = outlineColor;
            outlineImage.material = new Material(Shader.Find("UI/Default"));
            outlineImage.material.mainTextureOffset = new Vector2(0, 0);
            outlineImage.material.mainTextureScale = new Vector2(1, 1);
        }
    }

    void RemoveOutline()
    {
        // �Ƴ��߿�
        foreach (Transform child in transform.parent)
        {
            if (child.name.StartsWith("Outline_"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}