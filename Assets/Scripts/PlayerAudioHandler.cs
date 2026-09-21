using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;

    private PlayerController player;

    private void Awake() => player = GetComponent<PlayerController>();

    private void OnEnable()
    {
        player.Jumped += OnJump;
        player.Landed += OnLand;
    }

    private void OnDisable()
    {
        player.Jumped -= OnJump;
        player.Landed -= OnLand;
    }

    private void OnJump() => AudioManager.Instance.PlaySFX(jumpClip);
    private void OnLand() => AudioManager.Instance.PlaySFX(landClip);
}

/*
 * DECISIONES DE DISEÑO
 *
 * SCRIPT SEPARADO DE PlayerController
 * PlayerController no debería saber que existe audio. Separar la
 * responsabilidad en PlayerAudioHandler permite cambiar o sacar el
 * audio sin tocar la lógica de movimiento.
 *
 * SUSCRIPCIÓN A EVENTOS EN OnEnable / OnDisable
 * Garantiza que si el GameObject se desactiva y reactiva no quedan
 * suscripciones duplicadas. Los eventos Jumped y Landed ya están
 * definidos en PlayerController.
 */
