// Bag.cs

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 代表背包中的一个格子
[System.Serializable]
public class BagSlot
{
    public ItemData item;
    public int quantity;

    public BagSlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
}


[CreateAssetMenu(fileName = "New Bag", menuName = "RPG/Bag")]
public class Bag : ScriptableObject
{
    // 事件：当背包内容更新时触发，通知UI刷新
    public event Action OnInventoryChanged;

    public List<BagSlot> slots = new List<BagSlot>();

    /// <summary>
    /// 尝试向背包添加一个物品
    /// </summary>
    public bool AddItem(ItemData item, int quantity = 1)
    {
        // 如果物品可堆叠，先尝试堆叠
        if (item.isStackable)
        {
            var existingSlot = slots.FirstOrDefault(slot => slot.item == item && slot.quantity < item.maxStackSize);
            if (existingSlot != null)
            {
                existingSlot.AddQuantity(quantity);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 如果不能堆叠，或没有可堆叠的格子，则寻找一个新格子
        // TODO: 检查背包是否已满
        slots.Add(new BagSlot(item, quantity));
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// 从背包移除一个物品
    /// </summary>
    public void RemoveItem(ItemData item, int quantity = 1)
    {
        var slotToRemove = slots.FirstOrDefault(slot => slot.item == item);
        if (slotToRemove != null)
        {
            slotToRemove.quantity -= quantity;
            if (slotToRemove.quantity <= 0)
            {
                slots.Remove(slotToRemove);
            }
            OnInventoryChanged?.Invoke();
        }
    }

    /// <summary>
    /// 清空背包（用于游戏重置）
    /// </summary>
    public void Clear()
    {
        slots.Clear();
        OnInventoryChanged?.Invoke();
    }
}