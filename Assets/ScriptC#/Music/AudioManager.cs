// AudioManager.cs (最终修复版 v2)

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // --- 单例 ---
    private static AudioManager _instance;
    public static AudioManager Instance { get { /*...*/ return _instance; } }

    [Header("音频源")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("背景音乐片段")]
    public AudioClip mainMenuBGM;
    public AudioClip mapBGM;
    public AudioClip battleBGM;

    // --- 核心改动：用一个变量手动保存主BGM的播放时间 ---
    private float _mainBgmPlaybackTime = 0f;
    private AudioClip _mainBGM; // 仍然需要它来记住哪个是主BGM

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        if (bgmSource == null) { bgmSource = gameObject.AddComponent<AudioSource>(); bgmSource.loop = true; bgmSource.playOnAwake = false; }
        if (sfxSource == null) { sfxSource = gameObject.AddComponent<AudioSource>(); sfxSource.loop = false; }
    }

    // --- 公共播放方法 ---

    public void PlayMainMenuBGM()
    {
        _mainBGM = mainMenuBGM;
        PlayBGMFromStart(mainMenuBGM);
    }

    public void PlayMapBGM()
    {
        // 检查是否是从战斗等临时BGM状态恢复
        if (_mainBGM == mapBGM && bgmSource.clip != mapBGM)
        {
            // 这是从战斗回来的情况，需要恢复播放
            ResumeMainBGM();
        }
        else
        {
            // 这是第一次进入地图，或从主菜单等其他主BGM场景过来
            _mainBGM = mapBGM;
            PlayBGMFromStart(mapBGM);
        }
    }

    public void PlayBattleBGM()
    {
        // 战斗音乐是临时的，直接播放，不改变 _mainBGM
        PlayBGMFromStart(battleBGM);
    }

    // --- 核心改动：新的暂停和恢复逻辑 ---

    /// <summary>
    /// 暂停当前的主BGM，并记录它的播放时间点。
    /// </summary>
    public void PauseAndStoreMainBGMTime()
    {
        if (bgmSource.isPlaying && bgmSource.clip == _mainBGM)
        {
            _mainBgmPlaybackTime = bgmSource.time;
            bgmSource.Pause();
            Debug.Log($"[AudioManager] 主BGM '{_mainBGM.name}' 已在 {_mainBgmPlaybackTime}s 处暂停并记录时间。");
        }
    }

    /// <summary>
    /// 恢复播放主BGM
    /// </summary>
    private void ResumeMainBGM()
    {
        if (_mainBGM == null) return;

        Debug.Log($"[AudioManager] 准备从 {_mainBgmPlaybackTime}s 处恢复播放主BGM '{_mainBGM.name}'。");
        bgmSource.Stop(); // 停止当前播放的任何音乐（如战斗音乐）
        bgmSource.clip = _mainBGM;
        bgmSource.time = _mainBgmPlaybackTime; // !!! 关键：设置播放时间 !!!
        bgmSource.Play();
    }

    // --- 私有辅助方法 ---

    /// <summary>
    /// 从头开始播放一个BGM。
    /// </summary>
    private void PlayBGMFromStart(AudioClip clipToPlay)
    {
        if (clipToPlay == null) return;

        // 只有当要播放的音乐和当前的不同时才切换
        if (bgmSource.clip != clipToPlay)
        {
            bgmSource.Stop();
            bgmSource.clip = clipToPlay;
            bgmSource.time = 0; // 确保从头播放
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // 以下这两个方法现在不再被直接使用，但保留以备后用
    public void PauseBGM() { bgmSource.Pause(); }
    public void StopBGM() { bgmSource.Stop(); _mainBGM = null; }
}