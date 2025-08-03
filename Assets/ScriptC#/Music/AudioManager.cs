// AudioManager.cs (最终修复版 v4 - 战斗音乐也带渐变)

using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // --- 单例 ---
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                _instance = go.AddComponent<AudioManager>();
            }
            return _instance;
        }
    }

    [Header("音频源")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("背景音乐片段")]
    public AudioClip mainMenuBGM;
    public AudioClip mapBGM;
    public AudioClip battleBGM;

    // --- 状态变量 ---
    private float _mainBgmPlaybackTime = 0f;
    private AudioClip _mainBGM;
    private Coroutine _volumeFadeCoroutine;

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
        StopActiveFade();
        bgmSource.volume = 1f; // 主菜单音量恢复100%
        _mainBGM = mainMenuBGM;
        PlayBGMFromStart(mainMenuBGM);
    }

    public void PlayMapBGM()
    {
        StopActiveFade();

        bool resume = (_mainBGM == mapBGM && bgmSource.clip != mapBGM);

        bgmSource.clip = mapBGM;
        bgmSource.time = resume ? _mainBgmPlaybackTime : 0;
        _mainBGM = mapBGM;

        // 地图音乐的渐变 (你已修改)
        bgmSource.volume = 0.5f;
        bgmSource.Play();
        _volumeFadeCoroutine = StartCoroutine(FadeVolume(0.5f, 1.0f, 0.3f));
    }

    // *** 核心修改点在这里 ***
    public void PlayBattleBGM()
    {
        StopActiveFade(); // 同样，先停止任何可能存在的旧渐变

        // 将战斗BGM从头播放
        PlayBGMFromStart(battleBGM);

        // 立即将音量设置为起始值 0.1f
        bgmSource.volume = 0.1f;

        // 启动音量渐变协程
        _volumeFadeCoroutine = StartCoroutine(FadeVolume(0.1f, 0.25f, 2f));
    }

    // --- 音量渐变协程 ---
    // 这个协程无需改动，因为它设计得足够通用
    private IEnumerator FadeVolume(float startVolume, float endVolume, float duration)
    {
        float elapsedTime = 0;
        Debug.Log($"[AudioManager] 开始音量渐变: 从 {startVolume} 到 {endVolume}，时长 {duration}s。");
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / duration);
            yield return null;
        }
        bgmSource.volume = endVolume;
        _volumeFadeCoroutine = null;
    }

    // --- 辅助及暂停/恢复方法 ---
    // 这些方法也无需改动

    private void StopActiveFade()
    {
        if (_volumeFadeCoroutine != null)
        {
            StopCoroutine(_volumeFadeCoroutine);
            _volumeFadeCoroutine = null;
        }
    }

    public void PauseAndStoreMainBGMTime()
    {
        StopActiveFade();
        if (bgmSource.isPlaying && bgmSource.clip == _mainBGM)
        {
            bgmSource.volume = 1.0f;
            _mainBgmPlaybackTime = bgmSource.time;
            bgmSource.Pause();
            Debug.Log($"[AudioManager] 主BGM '{_mainBGM.name}' 已在 {_mainBgmPlaybackTime}s 处暂停并记录时间。");
        }
    }

    private void PlayBGMFromStart(AudioClip clipToPlay)
    {
        if (clipToPlay == null) return;

        // *** 逻辑微调：确保即使是同一个clip也能被重新播放 ***
        // 比如从战斗A到战斗B，我们希望音乐重头开始
        // 之前的`!bgmSource.isPlaying`条件可能会阻止这个
        if (bgmSource.clip != clipToPlay || bgmSource.time > 0.1f) // 只要不是同一个clip，或者播放了一点点，就重置
        {
            bgmSource.Stop();
            bgmSource.clip = clipToPlay;
            bgmSource.time = 0;
        }
        // 确保它能播放
        if (!bgmSource.isPlaying)
        {
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
}