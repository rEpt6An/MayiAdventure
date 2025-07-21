// Bag.cs

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// �������е�һ������
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
    // �¼������������ݸ���ʱ������֪ͨUIˢ��
    public event Action OnInventoryChanged;

    public List<BagSlot> slots = new List<BagSlot>();

    /// <summary>
    /// �����򱳰����һ����Ʒ
    /// </summary>
    public bool AddItem(ItemData item, int quantity = 1)
    {
        // �����Ʒ�ɶѵ����ȳ��Զѵ�
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

        // ������ܶѵ�����û�пɶѵ��ĸ��ӣ���Ѱ��һ���¸���
        // TODO: ��鱳���Ƿ�����
        slots.Add(new BagSlot(item, quantity));
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// �ӱ����Ƴ�һ����Ʒ
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
    /// ��ձ�����������Ϸ���ã�
    /// </summary>
    public void Clear()
    {
        slots.Clear();
        OnInventoryChanged?.Invoke();
    }
}