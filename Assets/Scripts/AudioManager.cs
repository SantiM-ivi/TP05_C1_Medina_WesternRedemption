using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips de música")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Clips de SFX")]
    [SerializeField] private AudioClip buttonClip;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumes();
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayGameplayMusic() => PlayMusic(gameplayMusic);
    public void PlayButtonSFX() => PlaySFX(buttonClip);
    public void StopMusic() => musicSource.Stop();
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void SetVolume(string param, float linear)
    {
        mixer.SetFloat(param, Mathf.Log10(Mathf.Max(linear, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat(param, linear);
        PlayerPrefs.Save();
    }

    public float GetLinearVolume(string param) => PlayerPrefs.GetFloat(param, 1f);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || (musicSource.clip == clip && musicSource.isPlaying)) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void LoadVolumes()
    {
        SetVolume("MasterVol", GetLinearVolume("MasterVol"));
        SetVolume("MusicVol", GetLinearVolume("MusicVol"));
        SetVolume("SFXVol", GetLinearVolume("SFXVol"));
    }

}


