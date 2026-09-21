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
        ShowMain();
        InitSliders();
    }

    private void InitSliders()
    {
        masterSlider.SetValueWithoutNotify(AudioManager.Instance.GetLinearVolume("MasterVol"));
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetLinearVolume("MusicVol"));
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetLinearVolume("SFXVol"));

        masterSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume("MasterVol", v));
        musicSlider.onValueChanged.AddListener(v  => AudioManager.Instance.SetVolume("MusicVol",  v));
        sfxSlider.onValueChanged.AddListener(v    => AudioManager.Instance.SetVolume("SFXVol",    v));
    }

    public void PlayGame()     => SceneManager.LoadScene(gameSceneName);
    public void ShowSettings() { mainPanel.SetActive(false); settingsPanel.SetActive(true); }
    public void ShowMain()     { settingsPanel.SetActive(false); mainPanel.SetActive(true); }
    public void QuitGame()     => Application.Quit();
}

/*
 * DECISIONES DE DISEÑO
 *
 * DOS PANELES EN UN CANVAS
 * MainPanel y SettingsPanel son hijos del mismo Canvas. Mostrar uno
 * desactiva el otro. Evita cargar una escena separada solo para
 * los ajustes de audio, lo que sería overhead innecesario.
 *
 * SetValueWithoutNotify PARA INICIALIZAR LOS SLIDERS
 * Al asignar el valor guardado con SetValueWithoutNotify, el slider
 * no dispara onValueChanged. Si se usara .value directamente, el
 * listener (que todavía no está registrado en ese punto) no dispara
 * igual, pero es una práctica más limpia y explícita.
 *
 * LISTENERS EN CÓDIGO
 * Los listeners se registran en código en lugar de en el Inspector
 * para mantener la conexión con AudioManager centralizada y evitar
 * que se desconecten si se rehace la UI.
 *
 * gameSceneName COMO CAMPO SERIALIZADO
 * Si la escena de juego se renombra, se cambia solo en el Inspector
 * sin tocar el código.
 */
