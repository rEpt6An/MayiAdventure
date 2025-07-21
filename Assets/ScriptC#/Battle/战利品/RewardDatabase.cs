// RewardDatabase.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // ����Linq�������ѯ

// ���Ƕ���ඨ����ÿ�ֽ����ľ�̬��Ϣ��ͼ�ꡢ�����ȣ�
[System.Serializable]
public class RewardInfo
{
    public RewardType type;
    public Sprite icon;
    [TextArea] // ��������Inspector����Զ��б༭
    public string defaultDescription = "{amount} an"; // {amount} ��һ��ռλ��
}


[CreateAssetMenu(fileName = "RewardDatabase", menuName = "RPG/Reward Database")]
public class RewardDatabase : ScriptableObject
{
    // ��Inspector����ǽ������������������͵Ľ���
    public List<RewardInfo> allRewardInfos;

    /// <summary>
    /// ���ݽ������Ͳ��Ҷ�Ӧ��������Ϣ
    /// </summary>
    public RewardInfo GetRewardInfo(RewardType type)
    {
        // ʹ��Linq����ƥ��ĵ�һ��Ԫ��
        return allRewardInfos.FirstOrDefault(info => info.type == type);
    }
}