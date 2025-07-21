// StatSet.cs

// 这是一个普通的C#类，用于存储一整套属性
[System.Serializable]
public class StatSet
{
    public int maxHP;
    public float maxMP;
    public int attack;
    public float attackSpeed = 1;
    public int critChance;
    public int defense;
    public int missChance;
    public float mpRegen = 10f;
    public float suckBlood;

    // 一个清空所有属性的方法
    public void Clear()
    {
        maxHP = 0;
        maxMP = 0;
        attack = 0;
        attackSpeed = 0; // 注意：百分比加成应该作用于基础值1上
        critChance = 0;
        defense = 0;
        missChance = 0;
        mpRegen = 0;
        suckBlood = 0;
    }

    // 将另一个StatSet的属性加到自己身上
    public void Add(StatSet other)
    {
        this.maxHP += other.maxHP;
        this.maxMP += other.maxMP;
        this.attack += other.attack;
        this.attackSpeed += other.attackSpeed;
        this.critChance += other.critChance;
        this.defense += other.defense;
        this.missChance += other.missChance;
        this.mpRegen += other.mpRegen;
        this.suckBlood += other.suckBlood;
    }
}