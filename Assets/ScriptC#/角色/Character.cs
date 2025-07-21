// Character.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour
{
    [Header("角色数据源 (由Battle脚本在运行时赋予)")]
    public PlayerData characterData;

    [Header("UI 引用 (Canvas)")]
    public Slider hpSlider;
    public Slider mpSlider;
    public TextMeshProUGUI hpText; // *** 新增: HP文本引用 ***
    public TextMeshProUGUI mpText;

    [Header("世界空间 视觉 (In-Scene)")]
    public SpriteRenderer characterSpriteRenderer;
    public TextMeshPro nameTextMesh;

    public bool isMoving = false;

    public void UpdateAllVisuals()
    {
        if (characterData == null) return;

        if (characterSpriteRenderer != null)
        {
            characterSpriteRenderer.sprite = characterData.characterSprite;
            characterSpriteRenderer.enabled = (characterData.characterSprite != null);
        }
        if (nameTextMesh != null)
        {
            nameTextMesh.text = characterData.characterName;
        }

        UpdateHPBar();
        UpdateMPBar();
    }

    // changeHP/MP现在只修改传入的数据，不再关心是实例还是模板
    public void changeHP(int change)
    {
        if (characterData == null) return;
        characterData.currentHP += change;
        characterData.currentHP = Mathf.Clamp(characterData.currentHP, 0, characterData.maxHP);
        // 不再自己调用UpdateHPBar，由Battle统一调用
    }

    public void changeMP(float magicCost)
    {
        if (characterData == null) return;
        characterData.currentMP += magicCost;
        characterData.currentMP = Mathf.Clamp(characterData.currentMP, 0, characterData.maxMP);
    }

    public void UpdateHPBar()
    {
        if (hpSlider != null && characterData != null && characterData.maxHP > 0)
        {
            hpSlider.value = (float)characterData.currentHP / characterData.maxHP;
        }
        // *** 新增: 更新HP文本 ***
        if (hpText != null && characterData != null)
        {
            hpText.text = $"{characterData.currentHP} / {characterData.maxHP}";
        }
    }

    public void UpdateMPBar()
    {
        if (mpSlider != null && characterData != null && characterData.maxMP > 0)
        {
            mpSlider.value = characterData.currentMP / characterData.maxMP;
        }
        if (mpText != null && characterData != null)
        {
            int currentMpInt = Mathf.FloorToInt(characterData.currentMP);
            int maxMpInt = Mathf.FloorToInt(characterData.maxMP);
            mpText.text = $"{currentMpInt} / {maxMpInt}";
        }
    }
}