// EventManager.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 这个GameEvent类现在只是一个运行时的数据容器，不再需要是ScriptableObject
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

    [Header("数据源")]
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private EventImageDatabase imageDatabase; // *** 新增: 引用图像数据库 ***

    private Dictionary<string, GameEvent> eventDictionary = new Dictionary<string, GameEvent>();
    private List<string> eventIdList = new List<string>(); // 存储所有事件ID，方便随机抽取
    private GameEvent currentEvent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 初始化图像数据库，确保字典准备就绪
            imageDatabase?.Initialize();
            // 在代码中定义所有事件
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

        // --- 事件1: “角色问题” ---
        var characterQuestion = new GameEvent("character_question",
            "看着这四位杀戮尖塔英俊潇洒的角色，请问你觉得到底谁才是杀戮尖塔最强的角色呢？");
        characterQuestion.buttonOptions = new string[] { "战士", "猎人", "鸡煲", "观者" };
        characterQuestion.resultTexts = new string[] { "跟我的肚皮说去吧。最大生命值+20", "转来转去是看你表演嘛？得加点攻击力了。攻击力+5", "加点防御才能启动..防御力+2", "喜欢我的喜怒无常吗？攻击速度+10%" };
        AddEvent(characterQuestion);

        // --- 事件2: “神秘喷泉” ---
        var fountainEvent = new GameEvent("fountain_of_youth",
            "你发现了一口散发着微光的神秘喷泉。你要做什么？");
        fountainEvent.buttonOptions = new string[] { "喝一口", "把金币丢进去", "洗把脸", "离开" };
        fountainEvent.resultTexts = new string[] { "你感到精神焕发！恢复50点生命值。", "金币沉入水底，你获得了好运！" +
            "金币-30，暴击率+10%。", "你感觉更清醒了！攻击速度+5%", "你谨慎地离开了。" };
        AddEvent(fountainEvent);

        var magicCat = new GameEvent("magicCat",
    "你在路边看到一只看起来很奇怪的猫咪，它正用一种诡异的眼神盯着你。你要怎么办？");
        magicCat.buttonOptions = new string[] { "摸摸他", "给他点吃的", "看着他", "赶紧跑喵" };
        magicCat.resultTexts = new string[] { "猫咪突然变成了一个巨大的毛球然后跑走了...（虽然不小心刮到了你，但是他还是很可爱!)生命值-25，攻击速度+10%", 
            "他打了个饱嗝，然后帮你舔了舔你的伤口。Food-25，生命值回复75。", 
            "他的眼睛变成了紫色，变得更可爱了！看到这双眼睛你的闪避率增加了10%，但是你有点头晕了...最大生命值-20。", 
            "哈基喵远远地看着你离开喵..." };
        AddEvent(magicCat);

        // --- 在这里添加更多新事件 ---

        Debug.Log($"事件数据库初始化完成，共 {eventDictionary.Count} 个事件。");
    }

    /// <summary>
    /// 开始一个指定ID的事件
    /// </summary>
    public void StartEvent(string eventID)
    {
        if (eventDictionary.TryGetValue(eventID, out currentEvent))
        {
            // 从数据库获取并设置图像
            currentEvent.eventImage = imageDatabase?.GetImage(eventID);

            // 命令UI控制器显示事件
            EventsUIController.Instance?.DisplayEvent(currentEvent);
        }
        else
        {
            Debug.LogError($"事件ID '{eventID}' 不存在！");
        }
    }

    /// <summary>
    /// 随机开始一个事件
    /// </summary>
    public void StartRandomEvent()
    {
        if (eventIdList.Count == 0)
        {
            Debug.LogWarning("事件池中没有任何事件可供随机！");
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

        // --- 所有事件的效果逻辑都集中在这里 ---
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