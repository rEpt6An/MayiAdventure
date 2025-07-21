// LootGroup.cs

using UnityEngine;
using System.Collections.Generic;

public enum LootGroupRule
{
    // ����1: ������������������ʵ���Ʒ����ҿ���ȫ��ʰȡ
    TakeAll,
    // ����2: ������������������ʵ���Ʒ�������ֻ��ѡ�� N ��
    PickN,
    // ����3: �����������ȡ N ����Ʒ��Ϊ���յ���
    RollN
}

[System.Serializable]
public class LootGroup
{
    [Header("���������")]
    public LootGroupRule rule = LootGroupRule.TakeAll;

    [Tooltip("������Ϊ PickN �� RollN ʱ�������ֵ��Ч��")]
    public int ruleAmount = 1;

    [Header("���ڵ�����")]
    public List<LootDrop> drops;
}