// EventImageDatabase.cs

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// ������Inspector�з���������¼�ID�����Ӧ��ͼ��
[System.Serializable]
public class EventImageMapping
{
    public string eventID;
    public Sprite eventImage;
}

[CreateAssetMenu(fileName = "New Event Image Database", menuName = "RPG/Event Image Database")]
public class EventImageDatabase : ScriptableObject
{
    // ��Inspector�����������¼���ͼ��
    public List<EventImageMapping> eventImages;

    private Dictionary<string, Sprite> imageDictionary;

    // �ڶ�������ʱ����Listת��Ϊ�ֵ��Ա���ٲ���
    public void Initialize()
    {
        imageDictionary = new Dictionary<string, Sprite>();
        if (eventImages == null) return;

        foreach (var mapping in eventImages)
        {
            if (!imageDictionary.ContainsKey(mapping.eventID))
            {
                imageDictionary.Add(mapping.eventID, mapping.eventImage);
            }
        }
    }

    /// <summary>
    /// �����¼�ID��ȡ��Ӧ��ͼ��
    /// </summary>
    public Sprite GetImage(string eventID)
    {
        if (imageDictionary == null)
        {
            // ȷ���ֵ��ѱ���ʼ��
            Initialize();
        }

        imageDictionary.TryGetValue(eventID, out Sprite image);
        return image;
    }
}