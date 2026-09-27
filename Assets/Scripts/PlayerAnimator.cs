using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int DeathTrigger   = Animator.StringToHash("Death");
    private static readonly int PickupTrigger  = Animator.StringToHash("Pickup");

    private Animator animator;
    private PlayerController player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player   = GetComponent<PlayerController>();
        Debug.Log("PlayerController encontrado: " + (player != null));
    }

    private void OnEnable()  => player.Died += OnDeath;
    private void OnDisable() => player.Died -= OnDeath;

    private void OnDeath()
    {
        Debug.Log("Death trigger disparado");
        animator.SetTrigger(DeathTrigger);
    }

    public void TriggerPickup() => animator.SetTrigger(PickupTrigger);
}
