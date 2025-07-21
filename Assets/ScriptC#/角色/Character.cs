// Character.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour
{
    [Header("��ɫ����Դ (��Battle�ű�������ʱ����)")]
    public PlayerData characterData;

    [Header("UI ���� (Canvas)")]
    public Slider hpSlider;
    public Slider mpSlider;
    public TextMeshProUGUI hpText; // *** ����: HP�ı����� ***
    public TextMeshProUGUI mpText;

    [Header("����ռ� �Ӿ� (In-Scene)")]
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

    // changeHP/MP����ֻ�޸Ĵ�������ݣ����ٹ�����ʵ������ģ��
    public void changeHP(int change)
    {
        if (characterData == null) return;
        characterData.currentHP += change;
        characterData.currentHP = Mathf.Clamp(characterData.currentHP, 0, characterData.maxHP);
        // �����Լ�����UpdateHPBar����Battleͳһ����
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
        // *** ����: ����HP�ı� ***
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