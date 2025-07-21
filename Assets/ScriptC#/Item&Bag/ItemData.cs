// ItemData.cs

using System.Collections.Generic;
using System.Text; // ����StringBuilder����Чƴ���ַ���
using UnityEngine;

// --- ����һЩ���õ�ö�� ---

public enum ItemType
{
    Equipment, // װ��
    Consumable // ����Ʒ
}

public enum ItemRarity
{
    Common,    // ��ͨ
    Rare,      // ϡ��
    Epic,      // ʷʫ
    Legendary,  // ��˵

    myth  //��
}
[CreateAssetMenu(fileName = "New Item", menuName = "RPG/Item Data")]
public class ItemData : ScriptableObject, ITooltipDataProvider
{
    [Header("������Ϣ")]
    public string itemName;
    public Sprite icon;
    [TextArea(3, 10)]
    public string description;
    public int price = 10;

    [Header("������ϡ�ж�")]
    public ItemType type;
    public ItemRarity rarity;
    public bool isStackable = true;
    public int maxStackSize = 99;

    // *** ���ĸĶ�: ʹ��һ��Ч���б��������������Լӳ� ***
    [Header("����Ч��")]
    [Tooltip("������Ʒ������Ч����������װ�������üӳɣ���������Ʒ��һ����Ч��")]
    public List<ItemEffect> effects;

    // Use�������ڽ�����Ч���б�Ӧ������
    public virtual void Use(PlayerData playerData)
    {
        if (type != ItemType.Consumable) return; // ֻ������Ʒ���ܱ���ʹ�á�

        foreach (var effect in effects)
        {
            playerData.ApplyEffect(effect);
        }
        Debug.Log($"ʹ����: {itemName}");
    }

    // Tooltip����Ҳ�ᶯ̬�ش�Ч���б�������
    public string GetTooltipHeader() { return itemName; }

    public string GetTooltipContent()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(description).AppendLine();

        if (effects.Count > 0)
        {
            sb.AppendLine("<b><color=orange>Ч��:</color></b>");
            foreach (var effect in effects)
            {
                // ��Ч��ת��Ϊ����ɶ����ı�
                sb.AppendLine(FormatEffect(effect));
            }
        }
        return sb.ToString();
    }

    // һ������������������ʽ��Ч��������
    private string FormatEffect(ItemEffect effect)
    {
        string prefix = effect.value > 0 ? "+" : "";
        switch (effect.stat)
        {
            case StatType.CurrentHP: return $"�ָ� {effect.value} ����";
            case StatType.MaxHP: return $"������� {prefix}{effect.value}";
            case StatType.Attack: return $"������ {prefix}{effect.value}";
            case StatType.AttackSpeedPercent: return $"�����ٶ� {prefix}{effect.value}%";
            case StatType.Defense: return $"������ {prefix}{effect.value}";


            case StatType.CritChance: return $"������ {prefix}{effect.value}%";
            case StatType.MissChance: return $"������ {prefix}{effect.value}%";
            case StatType.SuckBlood: return $"<color=red>��Ѫ {prefix}{effect.value}%</color>";


            default: return $"{effect.stat.ToString()} {prefix}{effect.value}";
        }
    }
}