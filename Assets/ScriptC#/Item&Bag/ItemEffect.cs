// ItemEffect.cs

// ���ǲ�����Ҫ using UnityEngine; ��Ϊ����û��MonoBehaviour

// ����һ��ö�٣��������п��ܱ���ƷӰ�������
public enum StatType
{
    CurrentHP,
    MaxHP,
    CurrentMP,
    MaxMP,
    Attack,
    Defense,
    AttackSpeedPercent,
    CritChance,
    MissChance,
    SuckBlood,
    Food,
    Gold
}

// ����һ��Ч�������ݽṹ
[System.Serializable] // ȷ��������Inspector����ʾ
public struct ItemEffect
{
    public StatType stat; // Ҫ�޸ĵ���������
    public int value;     // �޸ĵ���ֵ�����ڰٷֱȣ�10����10%��
}