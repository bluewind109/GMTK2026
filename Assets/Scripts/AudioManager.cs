using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAS;
    [SerializeField] private AudioSource audioSourcePrefab;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip playerAttackClip;
    [SerializeField] private AudioClip enemyAttackClip;
    [SerializeField] private AudioClip countdownClip;

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

    public void PlayPlayerAttackSfx() => PlaySfx(playerAttackClip);
    public void PlayEnemyAttackSfx() => PlaySfx(enemyAttackClip);
    public void PlayCountdownSfx() => PlaySfx(countdownClip);
}
