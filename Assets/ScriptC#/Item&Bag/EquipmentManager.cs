// EquipmentManager.cs

using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("数据源")]
    [Tooltip("游戏运行时，代表玩家最终属性的数据文件")]
    [SerializeField] private PlayerData runtimePlayerData;

    [Header("装备属性缓存 (在编辑器中创建并引用)")]
    [Tooltip("用于计算当前背包装备总加成的数据文件")]
    [SerializeField] private PlayerData currentEquipmentStats;
    [Tooltip("用于存储上一次计算结果的数据文件")]
    [SerializeField] private PlayerData lastEquipmentStats;

    void Awake()
    {
        // 健壮性检查
        if (runtimePlayerData == null || runtimePlayerData.inventory == null ||
            currentEquipmentStats == null || lastEquipmentStats == null)
        {
            Debug.LogError("EquipmentManager: 缺少关键数据文件的引用！请在Inspector中设置。", this.gameObject);
            this.enabled = false;
            return;
        }

        // 订阅事件
        runtimePlayerData.inventory.OnInventoryChanged += RecalculateStatsWithDelta;
        ResetPlayer.OnPlayerDataReset += HandleGameReset;
    }

    void Start()
    {
        // 游戏首次启动时，进行一次完整的属性计算
        RecalculateStatsWithDelta();
    }

    void OnDestroy()
    {
        if (runtimePlayerData?.inventory != null)
        {
            runtimePlayerData.inventory.OnInventoryChanged -= RecalculateStatsWithDelta;
        }
        if (ResetPlayer.Instance != null)
        {
            ResetPlayer.OnPlayerDataReset -= HandleGameReset;
        }
    }

    /// <summary>
    /// 当开始新游戏时，清空所有装备加成缓存
    /// </summary>
    private void HandleGameReset()
    {
        // 此时 runtimePlayerData 已经被模板覆盖，我们只需清空缓存即可
        currentEquipmentStats.ClearAllBattleStats();
        lastEquipmentStats.ClearAllBattleStats();
        Debug.Log("新游戏开始，装备属性缓存已清空。");
    }

    /// <summary>
    /// 核心方法：使用“差值”来更新玩家属性
    /// </summary>
    [ContextMenu("Recalculate Stats via Delta")]
    public void RecalculateStatsWithDelta()
    {
        // 1. 清空“当前装备加成”表，准备重新计算
        currentEquipmentStats.ClearAllBattleStats();

        // 2. 遍历背包，将所有装备的属性累加到“当前装备加成”表上
        if (runtimePlayerData.inventory != null)
        {
            foreach (var slot in runtimePlayerData.inventory.slots)
            {
                if (slot.item != null && slot.item.type == ItemType.Equipment)
                {
                    ApplyItemStatsTo(currentEquipmentStats, slot.item, slot.quantity);
                }
            }
        }

        // 3. 计算“新旧”加成之间的差值
        // (为了方便，我们直接把差值应用到玩家身上)

        // a. 先减去“上一次”的加成
        runtimePlayerData.maxHP -= lastEquipmentStats.maxHP;
        runtimePlayerData.Act -= lastEquipmentStats.Act;
        runtimePlayerData.Def -= lastEquipmentStats.Def;
        runtimePlayerData.Act_speed -= (lastEquipmentStats.Act_speed);
        runtimePlayerData.Critical_chance -= lastEquipmentStats.Critical_chance;
        runtimePlayerData.Miss -= lastEquipmentStats.Miss;
        runtimePlayerData.suckblood -= lastEquipmentStats.suckblood;
        // ... 为所有会被装备影响的属性执行减法 ...

        // b. 再加上“这一次”的加成
        runtimePlayerData.maxHP += currentEquipmentStats.maxHP;
        runtimePlayerData.Act += currentEquipmentStats.Act;
        runtimePlayerData.Def += currentEquipmentStats.Def;
        runtimePlayerData.Act_speed += (currentEquipmentStats.Act_speed);

        runtimePlayerData.Critical_chance += currentEquipmentStats.Critical_chance;
        runtimePlayerData.Miss += currentEquipmentStats.Miss;
        runtimePlayerData.suckblood += currentEquipmentStats.suckblood;
        // ... 为所有会被装备影响的属性执行加法 ...

        // 4. 确保当前血量不会超过新的最大血量
        runtimePlayerData.currentHP = Mathf.Min(runtimePlayerData.currentHP, runtimePlayerData.maxHP);

        // 5. 关键一步：更新“上一次”的加成快照，为下一次计算做准备
        lastEquipmentStats.CopyFrom(currentEquipmentStats, false); // 只复制战斗属性

        Debug.Log("使用差值更新完毕。最终攻击力: " + runtimePlayerData.Act);
    }

    /// <summary>
    /// 辅助方法：将一个物品的属性加到一个目标PlayerData上
    /// </summary>
    private void ApplyItemStatsTo(PlayerData targetData, ItemData item, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            foreach (var effect in item.effects)
            {
                targetData.ApplyEffect(effect);
            }
        }
    }
}