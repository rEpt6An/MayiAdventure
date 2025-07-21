// ResetPlayer.cs

using System;
using UnityEngine;
using UnityEngine.SceneManagement; // ���Ҳ���볡������

public class ResetPlayer : MonoBehaviour
{
    public static ResetPlayer Instance { get; private set; }

    [Header("���������ļ�")]
    [SerializeField] private PlayerData playerTemplate;
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private GameSessionData runtimeSessionData;

    // *** ����: ����װ�����Ի����ļ� ***
    [Header("װ�����Ի��� (�������)")]
    [SerializeField] private PlayerData currentEquipmentStats;
    [SerializeField] private PlayerData lastEquipmentStats;

    public static event Action OnPlayerDataReset;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �������Ӧ������Ϸ��ȫ����ʱ��ֻ����һ��
    // ������һ������������ GameInitializer �ű��� Start() �е���
    public void InitializeGameOnFirstLaunch()
    {
        StartNewGame(); // �״�������ͬ�ڿ�ʼһ������Ϸ
    }


    /// <summary>
    /// ����������˵����������Ϸ��ʱ����
    /// </summary>
    public void StartNewGame()
    {
        Debug.Log("ResetPlayer: ��ʼ����Ϸ����...");

        // 1. �����������
        if (playerTemplate != null && runtimePlayerData != null)
        {
            runtimePlayerData.CopyFrom(playerTemplate, true); // ʹ����ȫ����
            Debug.Log("  - ����ʱ��������Ѵ�ģ�����á�");
        }

        // 2. ���ûỰ����
        if (runtimeSessionData != null)
        {
            // isNewGame ��Ϊ true���� MapController ֪��Ҫ�������ɵ�ͼ
            runtimeSessionData.isNewGame = true;
            // �����һ�ε�ս��������Ϣ
            runtimeSessionData.nextEnemy = null;
            Debug.Log("  - ��Ϸ�Ự���������á�");
        }

        // 3. *** �����޸�: ���װ�����Ի��� ***
        if (currentEquipmentStats != null)
        {
            currentEquipmentStats.ClearAllBattleStats();
            Debug.Log("  - 'CurrentEquipmentStats' ��������ա�");
        }
        if (lastEquipmentStats != null)
        {
            lastEquipmentStats.ClearAllBattleStats();
            Debug.Log("  - 'LastEquipmentStats' ��������ա�");
        }

        // 4. ��������Ϸ�����á��Ĺ㲥��֪ͨ����ϵͳ����EquipmentManager�����г�ʼ��
        OnPlayerDataReset?.Invoke();

        Debug.Log("ResetPlayer: ����Ϸ����׼��������");

        // 5. (��ѡ) ֱ���������л�����ͼ����
        // SceneManager.LoadScene("Map");
    }
}