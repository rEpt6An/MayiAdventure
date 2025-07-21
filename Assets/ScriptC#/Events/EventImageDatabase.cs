// EventImageDatabase.cs

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 用于在Inspector中方便地配置事件ID和其对应的图像
[System.Serializable]
public class EventImageMapping
{
    public string eventID;
    public Sprite eventImage;
}

[CreateAssetMenu(fileName = "New Event Image Database", menuName = "RPG/Event Image Database")]
public class EventImageDatabase : ScriptableObject
{
    // 在Inspector中配置所有事件的图像
    public List<EventImageMapping> eventImages;

    private Dictionary<string, Sprite> imageDictionary;

    // 在对象启用时，将List转换为字典以便快速查找
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
    /// 根据事件ID获取对应的图像
    /// </summary>
    public Sprite GetImage(string eventID)
    {
        if (imageDictionary == null)
        {
            // 确保字典已被初始化
            Initialize();
        }

        imageDictionary.TryGetValue(eventID, out Sprite image);
        return image;
    }
}