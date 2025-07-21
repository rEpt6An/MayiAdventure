// ShopPool.cs

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Pool", menuName = "RPG/Shop Pool")]
public class ShopPool : ScriptableObject
{
    [Header("商店商品池")]
    [Tooltip("所有可能出现在商店里的物品列表")]
    public List<ItemData> availableItems;
}