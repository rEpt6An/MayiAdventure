// NodeUI.cs

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NodeUI : MonoBehaviour
{
    [Header("视觉元素")]
    [Tooltip("用于显示类型或等级图标的SpriteRenderer")]
    public SpriteRenderer iconRenderer;
    [Tooltip("地块的背景板")]
    public SpriteRenderer backgroundRenderer;

    [Header("视觉参数")]
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
            Debug.LogError("NodeUI: iconRenderer 或 backgroundRenderer 未在Inspector中设置！", this.gameObject);
        }
    }

    /// <summary>
    /// 最终的Setup方法，只接收一个代表内容的图标
    /// </summary>
    public void Setup(MapNode node, Sprite icon)
    {
        associatedNode = node;

        // 设置图标
        if (iconRenderer != null)
        {
            iconRenderer.sprite = icon;
            iconRenderer.enabled = (icon != null);
            AdjustIconSize();
        }

        // 背景始终存在，只更新颜色状态
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
            // 动画时，图标也跟着变色
            if (iconRenderer != null) iconRenderer.color = Color.Lerp(originalIconColor, highlightColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        HighlightAsCurrent(); // 确保最终是高亮状态
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
            iconRenderer.color = Color.white; // 当前高亮时，图标恢复正常颜色
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