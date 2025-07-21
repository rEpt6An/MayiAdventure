// ShopPool.cs

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Pool", menuName = "RPG/Shop Pool")]
public class ShopPool : ScriptableObject
{
    [Header("�̵���Ʒ��")]
    [Tooltip("���п��ܳ������̵������Ʒ�б�")]
    public List<ItemData> availableItems;
}