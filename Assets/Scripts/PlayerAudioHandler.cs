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

