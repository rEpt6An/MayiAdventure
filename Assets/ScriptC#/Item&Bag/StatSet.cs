// StatSet.cs

// ����һ����ͨ��C#�࣬���ڴ洢һ��������
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

    // һ������������Եķ���
    public void Clear()
    {
        maxHP = 0;
        maxMP = 0;
        attack = 0;
        attackSpeed = 0; // ע�⣺�ٷֱȼӳ�Ӧ�������ڻ���ֵ1��
        critChance = 0;
        defense = 0;
        missChance = 0;
        mpRegen = 0;
        suckBlood = 0;
    }

    // ����һ��StatSet�����Լӵ��Լ�����
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