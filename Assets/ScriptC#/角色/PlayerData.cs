// PlayerData.cs

using UnityEngine;
using System.Collections.Generic;


// *** 新增: 将全局掉落规则枚举定义在文件顶部 ***
// 这样可以确保它在整个项目中都可见
public enum GlobalLootRule
{
    ProcessAll, // 处理所有掉落组
    RollN_Groups // 从所有掉落组中随机抽取N个组来处理
}

[CreateAssetMenu(fileName = "新角色数据", menuName = "RPG/角色数据")]
public class PlayerData : ScriptableObject
{
    // *** 新增: 角色身份信息 ***
    [Header("角色信息")]
    public string characterName = "新角色";
    public Sprite characterSprite; // 用于战斗和UI中显示的角色形象

    [Header("基本属性")]
    public int maxHP;
    public int currentHP;
    public float maxMP;
    public float currentMP;

    [Header("战斗属性")]
    public int Act;
    public float Act_speed = 1;
    public int Critical_chance;
    public int Def;
    public int Miss;
    public float MP_speed = 10f;

    public float suckblood;  //吸血

    [Header("成长")]
    public int Gold;
    public int Food=100;
    public int MaxFood=100;


    // *** 新增: 全局掉落规则变量 ***
    [Header("全局掉落规则")]
    public GlobalLootRule globalRule = GlobalLootRule.ProcessAll;
    [Tooltip("当全局规则为 RollN_Groups 时，这个数值生效。")]
    public int globalRuleAmount = 1;

    [Header("掉落组 (Loot Groups)")]
    public List<LootGroup> lootGroups;

    // *** 新增: 背包数据引用 ***
    [Header("玩家背包")]
    public Bag inventory;


    /// <summary>
    /// 应用一个物品效果到当前数据上
    /// </summary>
    public void ApplyEffect(ItemEffect effect)
    {
        switch (effect.stat)
        {
            case StatType.CurrentHP:
                currentHP = Mathf.Clamp(currentHP + effect.value, 0, maxHP);
                break;
            case StatType.MaxHP:
                maxHP += effect.value;
                if (effect.value > 0) currentHP += effect.value; // 增加最大值时也回血
                break;
            case StatType.Attack:
                Act += effect.value;
                break;
            case StatType.Defense:
                Def += effect.value;
                break;
            case StatType.AttackSpeedPercent:
                Act_speed += ( effect.value / 100f);
                break;
            case StatType.SuckBlood:
                suckblood += effect.value;
                break;

            case StatType.CritChance:
                Critical_chance+= effect.value;
                break;
            case StatType.MissChance:
                Miss += effect.value;
                break;


            case StatType.Food:
                Food = Mathf.Clamp(Food + effect.value, 0, MaxFood);
                break;

            case StatType.Gold:
                Gold += effect.value;
                break;

        }
    }

    /// <summary>
    /// 移除一个物品效果（用于卸下装备）
    /// </summary>
    public void RemoveEffect(ItemEffect effect)
    {
        switch (effect.stat)
        {
            // 对于所有加法属性，直接做减法
            case StatType.MaxHP:
                maxHP -= effect.value;
                currentHP = Mathf.Min(currentHP, maxHP); // 确保当前血量不超过新的最大值
                break;
            case StatType.Attack:
                Act -= effect.value;
                break;
            case StatType.Defense:
                Def -= effect.value;
                break;
            case StatType.SuckBlood:
                suckblood -= effect.value;
                break;
            // 对于百分比乘法属性，做除法
            case StatType.AttackSpeedPercent:
                if (effect.value != -100) // 防止除以零
                    Act_speed -= ( effect.value / 100f);
                break;

            case StatType.CritChance:
                Critical_chance += effect.value;
                break;
            case StatType.MissChance:
                Miss += effect.value;
                break;
                // CurrentHP, Food等一次性效果通常不需要移除逻辑
        }
    }

    /// <summary>
    /// 将所有会被装备影响的战斗属性清零（用于缓存表）
    /// </summary>
    public void ClearAllBattleStats()
    {
        maxHP = 0;
        maxMP = 0;
        Act = 0;
        Act_speed = 1; // 攻速基础是1，不是0
        Critical_chance = 0;
        Def = 0;
        Miss = 0;
        MP_speed = 10f; // MP回复速度可以有个基础值
        suckblood = 0;
    }


    /// <param name="template">作为数据源的模板文件</param>
    /// <summary>
    /// 从模板复制数据，现在有一个选项可以只复制战斗属性
    /// </summary>
    public void CopyFrom(PlayerData template, bool copyAll = true)
    {
        if (template == null) return;

        // 复制战斗属性
        this.maxHP = template.maxHP;
        this.maxMP = template.maxMP;
        this.Act = template.Act;
        this.Act_speed = template.Act_speed;
        this.Critical_chance = template.Critical_chance;
        this.Def = template.Def;
        this.Miss = template.Miss;
        this.MP_speed = template.MP_speed;
        this.suckblood = template.suckblood;

        // 如果是完全复制（用于游戏重置）
        if (copyAll)
        {
            this.characterName = template.characterName;
            this.characterSprite = template.characterSprite;
            this.currentHP = template.maxHP;
            this.currentMP = template.maxMP;
            this.Gold = template.Gold;
            this.Food = template.Food;
            this.MaxFood = template.MaxFood;
            this.globalRule = template.globalRule;
            this.globalRuleAmount = template.globalRuleAmount;
            this.lootGroups = new List<LootGroup>(template.lootGroups);
            if (this.inventory != null)
            {
                this.inventory.Clear();
            }
        }
    }
}