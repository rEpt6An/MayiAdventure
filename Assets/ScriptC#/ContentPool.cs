// ContentPool.cs

using System.Collections.Generic;
using UnityEngine;

// 这是一个通用的泛型类，但Unity不支持泛型的ScriptableObject，所以我们为每种类型创建一个具体的子类
public abstract class ContentPool<T> : ScriptableObject where T : Object
{
    public List<T> items;

    public T GetRandom()
    {
        if (items == null || items.Count == 0) return null;
        return items[Random.Range(0, items.Count)];
    }
}

// --- 创建具体的怪物池 ---
[CreateAssetMenu(fileName = "New Encounter Pool", menuName = "RPG/Pools/Encounter Pool")]
public class EncounterPool : ContentPool<EncounterData> { }


