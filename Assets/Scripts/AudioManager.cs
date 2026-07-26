using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAS;
    [SerializeField] private AudioSource audioSourcePrefab;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip[] playerAttackClips;
    [SerializeField] private AudioClip[] enemyDeathClips;
    [SerializeField] private AudioClip playerTurnClip;
    [SerializeField] private AudioClip threeTwoOneCountDownClip;
    [SerializeField] private AudioClip zeroCountDownClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private AudioClip winClip;

    private int playerAttackClipIndex = 0;
    private int enemyDeathClipIndex = 0;

    private List<AudioSource> audioSources = new List<AudioSource>();

    private const int MAX_AUDIO_SOURCES = 15;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        for (int i = 0; i < MAX_AUDIO_SOURCES; i++)
        {
            AudioSource audioSource = Instantiate(audioSourcePrefab, transform);
            audioSources.Add(audioSource);
        }
    }

    public void PlayBgm()
    {
        if (bgm == null)
        {
            Debug.LogWarning("BGM clip is null. Cannot play background music.");
            return;
        }

        if (musicAS != null)
        {
            musicAS.clip = bgm;
            musicAS.loop = true;
            musicAS.Play();
        }
    }

    public void StopBgm()
    {
        if (musicAS != null && musicAS.isPlaying)
        {
            musicAS.Stop();
        }
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Audio clip is null. Cannot play sound effect.");
            return;
        }

        AudioSource audioSource = GetAvailableAudioSource();
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        return audioSources.Find(source => !source.isPlaying);
    }

    public void PlayPlayerAttackSfx()
    {
        if (playerAttackClips.Length == 0)
        {
            Debug.LogWarning("No player attack clips assigned.");
            return;
        }

        AudioClip clip = playerAttackClips[playerAttackClipIndex];
        PlaySfx(clip);

        // Cycle through the clips for variety
        playerAttackClipIndex = (playerAttackClipIndex + 1) % playerAttackClips.Length;
    }

    public void PlayEnemyDeathSfx()
    {
        if (enemyDeathClips.Length == 0)
        {
            Debug.LogWarning("No enemy death clips assigned.");
            return;
        }

        AudioClip clip = enemyDeathClips[enemyDeathClipIndex];
        PlaySfx(clip);

        // Cycle through the clips for variety
        enemyDeathClipIndex = (enemyDeathClipIndex + 1) % enemyDeathClips.Length;
    }

    public void PlayPlayerTurnSfx() => PlaySfx(playerTurnClip);
    public void PlayThreeTwoOneCountdownSfx() => PlaySfx(threeTwoOneCountDownClip);
    public void PlayZeroCountdownSfx() => PlaySfx(zeroCountDownClip);
    public void PlayLoseSfx() => PlaySfx(loseClip);
    public void PlayWinSfx() => PlaySfx(winClip);
}
