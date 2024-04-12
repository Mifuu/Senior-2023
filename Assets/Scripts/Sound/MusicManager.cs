using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource audioSource;
    public Music backgroundMusic;
    public Music bossMusic;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        audioSource.spatialBlend = 0f;
        Play(backgroundMusic);
    }

    public void Play(Music music)
    {
        audioSource.clip = music.clip;
        audioSource.volume = music.volume;
        audioSource.outputAudioMixerGroup = music.audioMixerGroup;
        audioSource.Play();
    }

    public static void PlayBackgroundMusic()
    {
        if (Instance == null)
        {
            Debug.Log("[Error] No singleton instance for MusicManager");
            return;
        }
        Instance.Play(Instance.backgroundMusic);
    }

    public static void PlayBossMusic()
    {
        if (Instance == null)
        {
            Debug.Log("[Error] No singleton instance for MusicManager");
            return;
        }
        Instance.Play(Instance.bossMusic);
    }

    public static void Stop(bool fade = false, float fadeTime = 4f)
    {
        if (!fade)
        {
            Instance.audioSource.Stop();
            return;
        }

        Instance.StartCoroutine(FadeAudioSource(0, fadeTime));
    }

    public static IEnumerator FadeAudioSource(float targetVolume, float duration)
    {
        if (Instance == null)
        {
            Debug.Log("[Error] No singleton instance for MusicManager");
            yield break;
        }

        if (duration < 0.01f)
        {
            Instance.audioSource.Stop();
            yield break;
        }

        float startVolume = Instance.audioSource.volume;
        float timer = 0;
        bool fadeIn = targetVolume > startVolume;

        while ((fadeIn && Instance.audioSource.volume <= targetVolume) || (!fadeIn && Instance.audioSource.volume >= targetVolume))
        {
            timer += Time.unscaledDeltaTime;
            Instance.audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);

            if (!fadeIn && Instance.audioSource.volume <= 0)
            {
                Instance.audioSource.Stop();
                yield break;
            }

            yield return null;
        }
    }

    void OnValidate()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}

[System.Serializable]
public class Music
{
    public AudioClip clip;

    [Range(0f, 4f)]
    public float volume = 1;
    public AudioMixerGroup audioMixerGroup;
}
