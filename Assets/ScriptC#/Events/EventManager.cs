// EventManager.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// ���GameEvent������ֻ��һ������ʱ������������������Ҫ��ScriptableObject
[System.Serializable]
public class GameEvent
{
    public string eventID;
    public string eventDescription;
    public string[] buttonOptions = new string[4];
    public string[] resultTexts = new string[4];
    public Sprite eventImage;

    public GameEvent(string id, string description)
    {
        eventID = id;
        eventDescription = description;
    }
}

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("����Դ")]
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private EventImageDatabase imageDatabase; // *** ����: ����ͼ�����ݿ� ***

    private Dictionary<string, GameEvent> eventDictionary = new Dictionary<string, GameEvent>();
    private List<string> eventIdList = new List<string>(); // �洢�����¼�ID�����������ȡ
    private GameEvent currentEvent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ��ʼ��ͼ�����ݿ⣬ȷ���ֵ�׼������
            imageDatabase?.Initialize();
            // �ڴ����ж��������¼�
            InitializeEventDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEventDatabase()
    {
        eventDictionary.Clear();
        eventIdList.Clear();

        // --- �¼�1: ����ɫ���⡱ ---
        var characterQuestion = new GameEvent("character_question",
            "��������λɱ¾����Ӣ�������Ľ�ɫ����������õ���˭����ɱ¾������ǿ�Ľ�ɫ�أ�");
        characterQuestion.buttonOptions = new string[] { "սʿ", "����", "����", "����" };
        characterQuestion.resultTexts = new string[] { "���ҵĶ�Ƥ˵ȥ�ɡ��������ֵ+20", "ת��תȥ�ǿ��������üӵ㹥�����ˡ�������+5", "�ӵ������������..������+2", "ϲ���ҵ�ϲŭ�޳��𣿹����ٶ�+10%" };
        AddEvent(characterQuestion);

        // --- �¼�2: ��������Ȫ�� ---
        var fountainEvent = new GameEvent("fountain_of_youth",
            "�㷢����һ��ɢ����΢���������Ȫ����Ҫ��ʲô��");
        fountainEvent.buttonOptions = new string[] { "��һ��", "�ѽ�Ҷ���ȥ", "ϴ����", "�뿪" };
        fountainEvent.resultTexts = new string[] { "��е�����������ָ�50������ֵ��", "��ҳ���ˮ�ף������˺��ˣ�" +
            "���-30��������+10%��", "��о��������ˣ������ٶ�+5%", "��������뿪�ˡ�" };
        AddEvent(fountainEvent);

        var magicCat = new GameEvent("magicCat",
    "����·�߿���һֻ����������ֵ�è�䣬������һ�ֹ�����������㡣��Ҫ��ô�죿");
        magicCat.buttonOptions = new string[] { "������", "������Ե�", "������", "�Ͻ�����" };
        magicCat.resultTexts = new string[] { "è��ͻȻ�����һ���޴��ë��Ȼ��������...����Ȼ��С�Ĺε����㣬���������Ǻܿɰ�!)����ֵ-25�������ٶ�+10%", 
            "�����˸����ã�Ȼ���������������˿ڡ�Food-25������ֵ�ظ�75��", 
            "�����۾��������ɫ����ø��ɰ��ˣ�������˫�۾����������������10%���������е�ͷ����...�������ֵ-20��", 
            "������ԶԶ�ؿ������뿪��..." };
        AddEvent(magicCat);

        // --- ��������Ӹ������¼� ---

        Debug.Log($"�¼����ݿ��ʼ����ɣ��� {eventDictionary.Count} ���¼���");
    }

    /// <summary>
    /// ��ʼһ��ָ��ID���¼�
    /// </summary>
    public void StartEvent(string eventID)
    {
        if (eventDictionary.TryGetValue(eventID, out currentEvent))
        {
            // �����ݿ��ȡ������ͼ��
            currentEvent.eventImage = imageDatabase?.GetImage(eventID);

            // ����UI��������ʾ�¼�
            EventsUIController.Instance?.DisplayEvent(currentEvent);
        }
        else
        {
            Debug.LogError($"�¼�ID '{eventID}' �����ڣ�");
        }
    }

    /// <summary>
    /// �����ʼһ���¼�
    /// </summary>
    public void StartRandomEvent()
    {
        if (eventIdList.Count == 0)
        {
            Debug.LogWarning("�¼�����û���κ��¼��ɹ������");
            return;
        }
        string randomEventID = eventIdList[Random.Range(0, eventIdList.Count)];
        StartEvent(randomEventID);
    }

    public void OnOptionSelected(int optionIndex)
    {
        if (currentEvent == null) return;
        ProcessEventEffect(currentEvent.eventID, optionIndex);

        if (optionIndex < currentEvent.resultTexts.Length)
        {
            EventsUIController.Instance?.SetResultText(currentEvent.resultTexts[optionIndex]);
        }
    }

    private void ProcessEventEffect(string eventID, int optionIndex)
    {
        if (runtimePlayerData == null) return;

        // --- �����¼���Ч���߼������������� ---
        switch (eventID)
        {
            case "character_question":
                if (optionIndex == 0) { runtimePlayerData.maxHP += 20; runtimePlayerData.currentHP += 20; }
                else if (optionIndex == 1) { runtimePlayerData.Act += 5; }
                else if (optionIndex == 2) { runtimePlayerData.Def += 2; }
                else if (optionIndex == 3) { runtimePlayerData.Act_speed += 0.1f; }
                break;

            case "fountain_of_youth":
                if (optionIndex == 0) { 
                    runtimePlayerData.currentHP = Mathf.Min(runtimePlayerData.maxHP, runtimePlayerData.currentHP + 50); 
                }
                else if (optionIndex == 1) { runtimePlayerData.Gold = Mathf.Max(0, runtimePlayerData.Gold - 30);
                    runtimePlayerData.Critical_chance += 10;
                }
                else if (optionIndex == 2) { runtimePlayerData.Act_speed += 0.05f; }
                break;

            case "magicCat":
                if (optionIndex == 0) { 
                    runtimePlayerData.currentHP = Mathf.Min(runtimePlayerData.maxHP, runtimePlayerData.currentHP -25);
                    runtimePlayerData.Act_speed += 0.1f;
                }
                else if (optionIndex == 1)
                {
                    runtimePlayerData.Food = Mathf.Max(0, runtimePlayerData.Food - 25);
                    runtimePlayerData.currentHP = Mathf.Min(runtimePlayerData.maxHP, runtimePlayerData.currentHP +75);
                }
                else if (optionIndex == 2) {
                    runtimePlayerData.maxHP -=20;
                    runtimePlayerData.Miss += 10; 
                }
                break;
        }
    }

    private void AddEvent(GameEvent gameEvent)
    {
        if (!eventDictionary.ContainsKey(gameEvent.eventID))
        {
            eventDictionary.Add(gameEvent.eventID, gameEvent);
            eventIdList.Add(gameEvent.eventID);
        }
    }
}