// DamagePopup.cs

using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    [Header("动画设置")]
    public float moveSpeed = 1.5f;
    public float fadeDuration = 1.2f;
    public float verticalOffset = -0.8f;
    public float horizontalRange = 0.5f;

    [Header("字体设置")]
    public int normalFontSize = 20;
    public int criticalFontSize = 20;
    public int missFontSize = 20;
    public int healFontSize = 20; // 新增：治疗字号

    [Header("颜色设置")]
    public Color normalDamageColor = Color.white;
    public Color criticalDamageColor = Color.red;
    public Color missColor = Color.blue;
    public Color healColor = Color.green; // 新增：治疗颜色

    [SerializeField] private TextMeshPro textMesh;
    private Camera mainCamera;
    private Vector3 originalScale;

    void Awake()
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.raycastTarget = false;
            textMesh.sortingOrder = 9999;
            textMesh.outlineWidth = 0.15f;
            textMesh.outlineColor = Color.black;
            originalScale = transform.localScale;
        }
        mainCamera = Camera.main;
    }

    // *** 核心改动: ShowDamage 方法现在接收一个新的 isHeal 参数 ***
    public void ShowDamage(int amount, bool isCritical, bool isMiss, bool isHeal, Vector3 worldPosition)
    {
        // === 位置设置 (逻辑不变) ===
        Vector3 basePosition = worldPosition + Vector3.up * verticalOffset;
        Vector3 randomOffset = new Vector3(Random.Range(-horizontalRange, horizontalRange), 0, 0);
        transform.position = basePosition + randomOffset;
        transform.localScale = originalScale;
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }

        // === 文本设置 (逻辑更新) ===
        if (textMesh != null)
        {
            textMesh.fontStyle = FontStyles.Normal;

            if (isMiss)
            {
                textMesh.text = "MISS";
                textMesh.color = missColor;
                textMesh.fontSize = missFontSize;
            }
            else if (isHeal)
            {
                textMesh.text = "+" + amount.ToString(); // 治疗数字带 "+" 号
                textMesh.color = healColor;
                textMesh.fontSize = healFontSize;
            }
            else if (isCritical)
            {
                textMesh.text = "-" + amount.ToString() + "!";
                textMesh.color = criticalDamageColor;
                textMesh.fontSize = criticalFontSize;
                textMesh.fontStyle = FontStyles.Bold;
            }
            else // 普通伤害
            {
                textMesh.text = "-" + amount.ToString();
                textMesh.color = normalDamageColor;
                textMesh.fontSize = normalFontSize;
            }

            textMesh.alpha = 1f;
        }

        gameObject.SetActive(true);
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < fadeDuration)
        {
            float progress = elapsed / fadeDuration;
            transform.position = startPos + Vector3.up * moveSpeed * elapsed;
            if (textMesh != null)
            {
                textMesh.alpha = Mathf.Lerp(1f, 0f, progress);
            }
            if (textMesh != null && textMesh.fontStyle == FontStyles.Bold)
            {
                float scaleFactor = 1.0f + Mathf.Sin(progress * Mathf.PI) * 0.3f;
                transform.localScale = originalScale * scaleFactor;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (DamagePopupPool.Instance != null)
        {
            DamagePopupPool.Instance.ReturnToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}