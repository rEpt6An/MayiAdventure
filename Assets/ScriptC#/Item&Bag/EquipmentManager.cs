// EquipmentManager.cs

using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("����Դ")]
    [Tooltip("��Ϸ����ʱ����������������Ե������ļ�")]
    [SerializeField] private PlayerData runtimePlayerData;

    [Header("װ�����Ի��� (�ڱ༭���д���������)")]
    [Tooltip("���ڼ��㵱ǰ����װ���ܼӳɵ������ļ�")]
    [SerializeField] private PlayerData currentEquipmentStats;
    [Tooltip("���ڴ洢��һ�μ������������ļ�")]
    [SerializeField] private PlayerData lastEquipmentStats;

    void Awake()
    {
        // ��׳�Լ��
        if (runtimePlayerData == null || runtimePlayerData.inventory == null ||
            currentEquipmentStats == null || lastEquipmentStats == null)
        {
            Debug.LogError("EquipmentManager: ȱ�ٹؼ������ļ������ã�����Inspector�����á�", this.gameObject);
            this.enabled = false;
            return;
        }

        // �����¼�
        runtimePlayerData.inventory.OnInventoryChanged += RecalculateStatsWithDelta;
        ResetPlayer.OnPlayerDataReset += HandleGameReset;
    }

    void Start()
    {
        // ��Ϸ�״�����ʱ������һ�����������Լ���
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
    /// ����ʼ����Ϸʱ���������װ���ӳɻ���
    /// </summary>
    private void HandleGameReset()
    {
        // ��ʱ runtimePlayerData �Ѿ���ģ�帲�ǣ�����ֻ����ջ��漴��
        currentEquipmentStats.ClearAllBattleStats();
        lastEquipmentStats.ClearAllBattleStats();
        Debug.Log("����Ϸ��ʼ��װ�����Ի�������ա�");
    }

    /// <summary>
    /// ���ķ�����ʹ�á���ֵ���������������
    /// </summary>
    [ContextMenu("Recalculate Stats via Delta")]
    public void RecalculateStatsWithDelta()
    {
        // 1. ��ա���ǰװ���ӳɡ���׼�����¼���
        currentEquipmentStats.ClearAllBattleStats();

        // 2. ����������������װ���������ۼӵ�����ǰװ���ӳɡ�����
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

        // 3. ���㡰�¾ɡ��ӳ�֮��Ĳ�ֵ
        // (Ϊ�˷��㣬����ֱ�ӰѲ�ֵӦ�õ��������)

        // a. �ȼ�ȥ����һ�Ρ��ļӳ�
        runtimePlayerData.maxHP -= lastEquipmentStats.maxHP;
        runtimePlayerData.Act -= lastEquipmentStats.Act;
        runtimePlayerData.Def -= lastEquipmentStats.Def;
        runtimePlayerData.Act_speed -= (lastEquipmentStats.Act_speed);
        runtimePlayerData.Critical_chance -= lastEquipmentStats.Critical_chance;
        runtimePlayerData.Miss -= lastEquipmentStats.Miss;
        runtimePlayerData.suckblood -= lastEquipmentStats.suckblood;
        // ... Ϊ���лᱻװ��Ӱ�������ִ�м��� ...

        // b. �ټ��ϡ���һ�Ρ��ļӳ�
        runtimePlayerData.maxHP += currentEquipmentStats.maxHP;
        runtimePlayerData.Act += currentEquipmentStats.Act;
        runtimePlayerData.Def += currentEquipmentStats.Def;
        runtimePlayerData.Act_speed += (currentEquipmentStats.Act_speed);

        runtimePlayerData.Critical_chance += currentEquipmentStats.Critical_chance;
        runtimePlayerData.Miss += currentEquipmentStats.Miss;
        runtimePlayerData.suckblood += currentEquipmentStats.suckblood;
        // ... Ϊ���лᱻװ��Ӱ�������ִ�мӷ� ...

        // 4. ȷ����ǰѪ�����ᳬ���µ����Ѫ��
        runtimePlayerData.currentHP = Mathf.Min(runtimePlayerData.currentHP, runtimePlayerData.maxHP);

        // 5. �ؼ�һ�������¡���һ�Ρ��ļӳɿ��գ�Ϊ��һ�μ�����׼��
        lastEquipmentStats.CopyFrom(currentEquipmentStats, false); // ֻ����ս������

        Debug.Log("ʹ�ò�ֵ������ϡ����չ�����: " + runtimePlayerData.Act);
    }

    /// <summary>
    /// ������������һ����Ʒ�����Լӵ�һ��Ŀ��PlayerData��
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