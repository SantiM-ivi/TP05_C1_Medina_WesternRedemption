using UnityEngine;

public class ObstacleRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;

    private Rigidbody2D rb;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        rb.SetRotation(rb.rotation - rotationSpeed * Time.deltaTime);
    }
}