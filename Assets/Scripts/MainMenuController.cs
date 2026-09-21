using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Sliders de volumen")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Escena")]
    [SerializeField] private string gameSceneName = "Game";

    private void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
        ShowMain();
        InitSliders();
    }

    private void InitSliders()
    {
        masterSlider.SetValueWithoutNotify(AudioManager.Instance.GetLinearVolume("MasterVol"));
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetLinearVolume("MusicVol"));
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetLinearVolume("SFXVol"));

        masterSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume("MasterVol", v));
        musicSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume("MusicVol", v));
        sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume("SFXVol", v));
    }

    public void PlayGame() => SceneManager.LoadScene(gameSceneName);
    public void ShowSettings() { mainPanel.SetActive(false); settingsPanel.SetActive(true); }
    public void ShowMain() { settingsPanel.SetActive(false); mainPanel.SetActive(true); }
    public void QuitGame() => Application.Quit();
}

