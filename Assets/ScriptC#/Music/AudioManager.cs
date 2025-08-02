// AudioManager.cs

using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // --- 单例模式 ---
    // 这是确保AudioManager在整个游戏中只有一个实例的标准写法
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 如果场景中没有AudioManager，就动态创建一个
                GameObject go = new GameObject("AudioManager");
                _instance = go.AddComponent<AudioManager>();
            }
            return _instance;
        }
    }

    [Header("音频源 (Audio Sources)")]
    [Tooltip("专门用于播放背景音乐的AudioSource")]
    public AudioSource bgmSource;
    [Tooltip("专门用于播放音效的AudioSource")]
    public AudioSource sfxSource; // 预留，用于未来播放点击、攻击等音效

    [Header("背景音乐 (BGM Clips)")]
    public AudioClip mainMenuBGM;
    public AudioClip mapBGM;
    public AudioClip battleBGM;

    // 用于存储当前正在播放的BGM，以便进行比较
    private AudioClip _currentBGM;

    void Awake()
    {
        // --- 实现持久化单例 ---
        if (_instance == null)
        {
            // 如果_instance为空，说明这是第一个实例
            _instance = this;
            // 让这个AudioManager在加载新场景时不被销毁
            DontDestroyOnLoad(gameObject);

            // 动态创建并配置AudioSource组件
            InitializeAudioSources();
        }
        else if (_instance != this)
        {
            // 如果场景中已经存在一个AudioManager，就销毁这个新的（重复的）
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // 如果Inspector中没有手动指定，就自动创建
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true; // BGM通常是循环的
            bgmSource.playOnAwake = false; // 不要自动播放
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// 播放主菜单BGM
    /// </summary>
    public void PlayMainMenuBGM()
    {
        PlayBGM(mainMenuBGM, true); // `forceRestart = true` 确保每次都从头播放
    }

    /// <summary>
    /// 播放地图BGM (支持暂停/继续)
    /// </summary>
    public void PlayMapBGM()
    {
        // 只有当要播放的音乐不是当前正在播放的地图音乐时，才强制从头播放
        // 否则，它会继续播放
        bool forceRestart = (_currentBGM != mapBGM);
        PlayBGM(mapBGM, forceRestart);
    }

    /// <summary>
    /// 播放战斗BGM
    /// </summary>
    public void PlayBattleBGM()
    {
        PlayBGM(battleBGM, true); // `forceRestart = true` 确保每次都从头播放
    }

    /// <summary>
    /// 暂停当前BGM
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    /// <summary>
    /// 停止当前BGM
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
        _currentBGM = null;
    }

    /// <summary>
    /// 核心的BGM播放逻辑
    /// </summary>
    /// <param name="clipToPlay">要播放的音频片段</param>
    /// <param name="forceRestart">是否强制从头播放</param>
    private void PlayBGM(AudioClip clipToPlay, bool forceRestart)
    {
        if (clipToPlay == null)
        {
            Debug.LogWarning("要播放的BGM为空！");
            StopBGM();
            return;
        }

        // 情况1：如果要求强制重播，或者当前BGM与要播放的不同
        if (forceRestart || _currentBGM != clipToPlay)
        {
            bgmSource.Stop();
            bgmSource.clip = clipToPlay;
            bgmSource.Play();
            _currentBGM = clipToPlay;
        }
        // 情况2：如果要播放的音乐与当前音乐相同，且当前音乐是暂停状态，则继续播放
        else if (_currentBGM == clipToPlay && !bgmSource.isPlaying)
        {
            bgmSource.UnPause();
        }
        // 情况3：如果要播放的音乐与当前音乐相同，且正在播放，则什么都不做
    }

    // (可选) 播放音效的方法
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}