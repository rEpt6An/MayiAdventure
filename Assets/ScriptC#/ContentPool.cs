// ContentPool.cs

using System.Collections.Generic;
using UnityEngine;

// ����һ��ͨ�õķ����࣬��Unity��֧�ַ��͵�ScriptableObject����������Ϊÿ�����ʹ���һ�����������
public abstract class ContentPool<T> : ScriptableObject where T : Object
{
    public List<T> items;

    public T GetRandom()
    {
        if (items == null || items.Count == 0) return null;
        return items[Random.Range(0, items.Count)];
    }
}

// --- ��������Ĺ���� ---
[CreateAssetMenu(fileName = "New Encounter Pool", menuName = "RPG/Pools/Encounter Pool")]
public class EncounterPool : ContentPool<EncounterData> { }


