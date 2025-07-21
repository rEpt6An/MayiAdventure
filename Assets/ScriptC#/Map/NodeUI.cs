// NodeUI.cs

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NodeUI : MonoBehaviour
{
    [Header("�Ӿ�Ԫ��")]
    [Tooltip("������ʾ���ͻ�ȼ�ͼ���SpriteRenderer")]
    public SpriteRenderer iconRenderer;
    [Tooltip("�ؿ�ı�����")]
    public SpriteRenderer backgroundRenderer;

    [Header("�Ӿ�����")]
    public Vector2 iconDisplaySize = new Vector2(0.8f, 0.8f);
    public Color completedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    public Color highlightColor = Color.yellow;
    public float highlightDuration = 0.5f;

    private MapNode associatedNode;
    private Color originalBackgroundColor;
    private Color originalIconColor;

    void Awake()
    {
        if (iconRenderer == null || backgroundRenderer == null)
        {
            Debug.LogError("NodeUI: iconRenderer �� backgroundRenderer δ��Inspector�����ã�", this.gameObject);
        }
    }

    /// <summary>
    /// ���յ�Setup������ֻ����һ���������ݵ�ͼ��
    /// </summary>
    public void Setup(MapNode node, Sprite icon)
    {
        associatedNode = node;

        // ����ͼ��
        if (iconRenderer != null)
        {
            iconRenderer.sprite = icon;
            iconRenderer.enabled = (icon != null);
            AdjustIconSize();
        }

        // ����ʼ�մ��ڣ�ֻ������ɫ״̬
        UpdateVisualState();
    }

    public MapNode GetNode()
    {
        return associatedNode;
    }
    public IEnumerator PlayEnterAnimation()
    {
        if (backgroundRenderer == null) yield break;
        float elapsedTime = 0f;
        originalBackgroundColor = backgroundRenderer.color;
        originalIconColor = iconRenderer.color;

        while (elapsedTime < highlightDuration)
        {
            float t = elapsedTime / highlightDuration;
            backgroundRenderer.color = Color.Lerp(originalBackgroundColor, highlightColor, t);
            // ����ʱ��ͼ��Ҳ���ű�ɫ
            if (iconRenderer != null) iconRenderer.color = Color.Lerp(originalIconColor, highlightColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        HighlightAsCurrent(); // ȷ�������Ǹ���״̬
    }

    public void UpdateVisualState()
    {
        if (backgroundRenderer == null || associatedNode == null) return;

        if (associatedNode.isCompleted)
        {
            backgroundRenderer.color = completedColor;
            if (iconRenderer != null) iconRenderer.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        }
        else
        {
            backgroundRenderer.color = Color.white;
            if (iconRenderer != null) iconRenderer.color = Color.white;
        }
    }

    public void HighlightAsCurrent()
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.color = highlightColor;
        }
        if (iconRenderer != null)
        {
            iconRenderer.color = Color.white; // ��ǰ����ʱ��ͼ��ָ�������ɫ
        }
    }

    private void AdjustIconSize()
    {
        if (iconRenderer == null || iconRenderer.sprite == null) return;

        float spriteWidth = iconRenderer.sprite.bounds.size.x;
        float spriteHeight = iconRenderer.sprite.bounds.size.y;

        if (spriteWidth == 0 || spriteHeight == 0) return;

        float scaleX = iconDisplaySize.x / spriteWidth;
        float scaleY = iconDisplaySize.y / spriteHeight;

        iconRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}