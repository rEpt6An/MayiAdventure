// PlayerData.cs

using UnityEngine;
using System.Collections.Generic;


// *** ����: ��ȫ�ֵ������ö�ٶ������ļ����� ***
// ��������ȷ������������Ŀ�ж��ɼ�
public enum GlobalLootRule
{
    ProcessAll, // �������е�����
    RollN_Groups // �����е������������ȡN����������
}

[CreateAssetMenu(fileName = "�½�ɫ����", menuName = "RPG/��ɫ����")]
public class PlayerData : ScriptableObject
{
    // *** ����: ��ɫ�����Ϣ ***
    [Header("��ɫ��Ϣ")]
    public string characterName = "�½�ɫ";
    public Sprite characterSprite; // ����ս����UI����ʾ�Ľ�ɫ����

    [Header("��������")]
    public int maxHP;
    public int currentHP;
    public float maxMP;
    public float currentMP;

    [Header("ս������")]
    public int Act;
    public float Act_speed = 1;
    public int Critical_chance;
    public int Def;
    public int Miss;
    public float MP_speed = 10f;

    public float suckblood;  //��Ѫ

    [Header("�ɳ�")]
    public int Gold;
    public int Food=100;
    public int MaxFood=100;


    // *** ����: ȫ�ֵ��������� ***
    [Header("ȫ�ֵ������")]
    public GlobalLootRule globalRule = GlobalLootRule.ProcessAll;
    [Tooltip("��ȫ�ֹ���Ϊ RollN_Groups ʱ�������ֵ��Ч��")]
    public int globalRuleAmount = 1;

    [Header("������ (Loot Groups)")]
    public List<LootGroup> lootGroups;

    // *** ����: ������������ ***
    [Header("��ұ���")]
    public Bag inventory;


    /// <summary>
    /// Ӧ��һ����ƷЧ������ǰ������
    /// </summary>
    public void ApplyEffect(ItemEffect effect)
    {
        switch (effect.stat)
        {
            case StatType.CurrentHP:
                currentHP = Mathf.Clamp(currentHP + effect.value, 0, maxHP);
                break;
            case StatType.MaxHP:
                maxHP += effect.value;
                if (effect.value > 0) currentHP += effect.value; // �������ֵʱҲ��Ѫ
                break;
            case StatType.Attack:
                Act += effect.value;
                break;
            case StatType.Defense:
                Def += effect.value;
                break;
            case StatType.AttackSpeedPercent:
                Act_speed += ( effect.value / 100f);
                break;
            case StatType.SuckBlood:
                suckblood += effect.value;
                break;

            case StatType.CritChance:
                Critical_chance+= effect.value;
                break;
            case StatType.MissChance:
                Miss += effect.value;
                break;


            case StatType.Food:
                Food = Mathf.Clamp(Food + effect.value, 0, MaxFood);
                break;

            case StatType.Gold:
                Gold += effect.value;
                break;

        }
    }

    /// <summary>
    /// �Ƴ�һ����ƷЧ��������ж��װ����
    /// </summary>
    public void RemoveEffect(ItemEffect effect)
    {
        switch (effect.stat)
        {
            // �������мӷ����ԣ�ֱ��������
            case StatType.MaxHP:
                maxHP -= effect.value;
                currentHP = Mathf.Min(currentHP, maxHP); // ȷ����ǰѪ���������µ����ֵ
                break;
            case StatType.Attack:
                Act -= effect.value;
                break;
            case StatType.Defense:
                Def -= effect.value;
                break;
            case StatType.SuckBlood:
                suckblood -= effect.value;
                break;
            // ���ڰٷֱȳ˷����ԣ�������
            case StatType.AttackSpeedPercent:
                if (effect.value != -100) // ��ֹ������
                    Act_speed -= ( effect.value / 100f);
                break;

            case StatType.CritChance:
                Critical_chance += effect.value;
                break;
            case StatType.MissChance:
                Miss += effect.value;
                break;
                // CurrentHP, Food��һ����Ч��ͨ������Ҫ�Ƴ��߼�
        }
    }

    /// <summary>
    /// �����лᱻװ��Ӱ���ս���������㣨���ڻ����
    /// </summary>
    public void ClearAllBattleStats()
    {
        maxHP = 0;
        maxMP = 0;
        Act = 0;
        Act_speed = 1; // ���ٻ�����1������0
        Critical_chance = 0;
        Def = 0;
        Miss = 0;
        MP_speed = 10f; // MP�ظ��ٶȿ����и�����ֵ
        suckblood = 0;
    }


    /// <param name="template">��Ϊ����Դ��ģ���ļ�</param>
    /// <summary>
    /// ��ģ�帴�����ݣ�������һ��ѡ�����ֻ����ս������
    /// </summary>
    public void CopyFrom(PlayerData template, bool copyAll = true)
    {
        if (template == null) return;

        // ����ս������
        this.maxHP = template.maxHP;
        this.maxMP = template.maxMP;
        this.Act = template.Act;
        this.Act_speed = template.Act_speed;
        this.Critical_chance = template.Critical_chance;
        this.Def = template.Def;
        this.Miss = template.Miss;
        this.MP_speed = template.MP_speed;
        this.suckblood = template.suckblood;

        // �������ȫ���ƣ�������Ϸ���ã�
        if (copyAll)
        {
            this.characterName = template.characterName;
            this.characterSprite = template.characterSprite;
            this.currentHP = template.maxHP;
            this.currentMP = template.maxMP;
            this.Gold = template.Gold;
            this.Food = template.Food;
            this.MaxFood = template.MaxFood;
            this.globalRule = template.globalRule;
            this.globalRuleAmount = template.globalRuleAmount;
            this.lootGroups = new List<LootGroup>(template.lootGroups);
            if (this.inventory != null)
            {
                this.inventory.Clear();
            }
        }
    }
}