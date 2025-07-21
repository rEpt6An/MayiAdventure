// PlayerDataViewer.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDataViewer : MonoBehaviour
{
    // *** ���ĸĶ� 1: ������Ҫ�ֶ�����TargetCharacter ***
    // [SerializeField] private Character targetCharacter;

    // ������ֻ�����Լ�Ҫ��ʾ�ĸ���ɫ��UI
    public enum ViewerType { Player, Enemy }
    [Header("��ʾĿ��")]
    public ViewerType displayTarget;

    [Header("UI Ԫ������")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider mpSlider;
    public TextMeshProUGUI mpText;

    private PlayerData currentData;

    void Start()
    {
        // --- �ҵ�ս���������Ͷ�Ӧ�Ľ�ɫ ---
        Battle battle = FindObjectOfType<Battle>();
        if (battle == null)
        {
            Debug.LogError("PlayerDataViewer: �������Ҳ���Battle�ű���", this.gameObject);
            return;
        }

        Character targetCharacter = null;
        if (displayTarget == ViewerType.Player)
        {
            targetCharacter = battle.attacker;
        }
        else // if (displayTarget == ViewerType.Enemy)
        {
            targetCharacter = battle.defender;
        }

        if (targetCharacter == null)
        {
            Debug.LogError($"PlayerDataViewer for {displayTarget} �Ҳ���Ŀ��Character��", this.gameObject);
            return;
        }

        // *** ���ĸĶ� 2: �����¼� ***
        //targetCharacter.OnDataChanged += HandleDataChanged;

        // --- �״�����ʱ���ֶ�����һ�� ---
        // �����Ҫ����Ϊ��Startʱ����ɫ�����ݿ����Ѿ���Awake�е�Battle�ű���ֵ��
        if (targetCharacter.characterData != null)
        {
            HandleDataChanged(targetCharacter.characterData);
        }
    }

    void OnDestroy()
    {
        // Ϊ�˷�ֹ�ڴ�й©����Ҫ�ҵ���ȡ������
        // ����һ���򻯵�ʵ�֣��ڸ����ӵ�ϵͳ����Ҫ����׳���¼�������
    }

    /// <summary>
    /// �����յ���ɫ���ݱ仯���ź�ʱ��ִ�д˷���
    /// </summary>
    private void HandleDataChanged(PlayerData newData)
    {
        currentData = newData;
        UpdateAllStats();
    }

    /// <summary>
    /// һ��ͳһ�ġ���������״̬��ʾ�ķ���
    /// </summary>
    public void UpdateAllStats()
    {
        if (currentData == null) return;

        // ����Ѫ��
        if (hpSlider != null)
        {
            if (currentData.maxHP > 0)
                hpSlider.value = (float)currentData.currentHP / currentData.maxHP;
        }
        if (hpText != null)
        {
            hpText.text = $"{currentData.currentHP} / {currentData.maxHP}";
        }

        // ��������
        int currentMpInt = Mathf.FloorToInt(currentData.currentMP);
        int maxMpInt = Mathf.FloorToInt(currentData.maxMP);
        if (mpSlider != null)
        {
            if (currentData.maxMP > 0)
                mpSlider.value = currentData.currentMP / currentData.maxMP;
        }
        if (mpText != null)
        {
            mpText.text = $"{currentMpInt} / {maxMpInt}";
        }
    }
}