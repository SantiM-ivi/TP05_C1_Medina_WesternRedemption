using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start() => LoadVolumes();

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") PlayMusic(menuMusic);
        else                          PlayMusic(gameplayMusic);
    }

    public void SetVolume(string param, float linear)
    {
        mixer.SetFloat(param, Mathf.Log10(Mathf.Max(linear, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat(param, linear);
        PlayerPrefs.Save();
    }

    public float GetLinearVolume(string param) => PlayerPrefs.GetFloat(param, 1f);

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || (musicSource.clip == clip && musicSource.isPlaying)) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void LoadVolumes()
    {
        SetVolume("MasterVol", GetLinearVolume("MasterVol"));
        SetVolume("MusicVol",  GetLinearVolume("MusicVol"));
        SetVolume("SFXVol",    GetLinearVolume("SFXVol"));
    }
}

/*
 * DECISIONES DE DISEÑO
 *
 * DontDestroyOnLoad
 * AudioManager vive entre escenas. Así la música de menú no se corta
 * al cargar el juego y los volúmenes configurados persisten sin volver
 * a leerlos desde PlayerPrefs en cada escena.
 *
 * SceneManager.sceneLoaded PARA CAMBIAR MÚSICA
 * En lugar de que cada escena llame PlayMenuMusic o PlayGameMusic
 * manualmente, AudioManager escucha el evento y cambia el clip solo.
 * El nombre de la escena determina qué música tocar. Si la escena
 * no se llama "MainMenu", usa el clip de gameplay.
 *
 * CONVERSIÓN LINEAL A dB
 * Los sliders van de 0 a 1 (lineal). El AudioMixer trabaja en dB.
 * La conversión es dB = log10(linear) * 20. El mínimo de 0.0001
 * evita log10(0) que es -infinito y silencia el canal correctamente.
 *
 * PlayerPrefs PARA PERSISTENCIA
 * Los volúmenes se guardan con la clave del parámetro del mixer.
 * Al arrancar, LoadVolumes los aplica tanto al mixer como al estado
 * interno. El valor por defecto si no existe la clave es 1f (100%).
 *
 * DOS AudioSource EN EL MISMO GAMEOBJECT
 * musicSource para música en loop, sfxSource para efectos con
 * PlayOneShot. PlayOneShot permite superponer sonidos sin cortar el
 * anterior, ideal para SFX de salto y coleccionables.
 *
 * SETUP
 * 1. En Project: clic derecho > Create > Audio > Audio Mixer.
 *    Llamarlo "GameMixer".
 * 2. En la ventana Audio Mixer, agregar grupos "Music" y "SFX"
 *    bajo Master.
 * 3. En cada grupo, clic derecho en el parámetro Volume > Expose.
 *    Renombrar los expuestos como "MasterVol", "MusicVol", "SFXVol"
 *    (pestaña Exposed Parameters del Audio Mixer).
 * 4. Crear un GameObject "AudioManager" en la escena MainMenu.
 *    Agregar este script y dos AudioSource hijos o en el mismo objeto.
 *    Asignar el mixer y los sources en el Inspector.
 * 5. El output de musicSource va al grupo Music, el de sfxSource a SFX.
 */
