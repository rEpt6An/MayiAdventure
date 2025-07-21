// LootDrop.cs

using UnityEngine;

[System.Serializable]
public class LootDrop
{
    [Header("�������������")]
    [Tooltip("����������������Խ���������Ʒ����")]
    public RewardType type;

    [Tooltip("������������ĸ��ʣ�0-100��")]
    [Range(0, 100)]
    public float dropChance = 100f;

    // --- ���Խ���ר���ֶ� ---
    [Header("���Խ������� (������Ͳ���Item)")]
    [Tooltip("������������Сֵ")]
    public int minAmount = 1;
    [Tooltip("�������������ֵ")]
    public int maxAmount = 1;

    // --- ��Ʒ����ר���ֶ� ---
    [Header("��Ʒ�������� (���������Item)")]
    [Tooltip("ֱ������һ��ItemData�ʲ��ļ���Ϊ����")]
    public ItemData specificItem;
    [Tooltip("���������Ʒ������")]
    public int itemQuantity = 1;

    [Header("�Զ�����ʾ")]
    [Tooltip("������ʹ�����ݿ��е�Ĭ��������{amount} �� {itemName} �ᱻ�滻��")]
    [TextArea]
    public string overrideDescription;
}