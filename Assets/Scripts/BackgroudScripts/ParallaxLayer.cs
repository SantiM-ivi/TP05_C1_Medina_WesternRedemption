using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Velocidad")]
    [Range(0f, 1f)]
    [SerializeField] private float speedMultiplier = 0.5f;

    [Header("Biomas")]
    [SerializeField] private Sprite[] biomeSprites;

    private Transform[] pieces;
    private float pieceWidth;
    private float totalWidth;

    private void Awake()
    {
        pieces = new Transform[transform.childCount];
        for (int i = 0; i < pieces.Length; i++)
            pieces[i] = transform.GetChild(i);

        if (pieces.Length > 0 && pieces[0].TryGetComponent<SpriteRenderer>(out var sr))
            pieceWidth = sr.bounds.size.x;
        else
            pieceWidth = 20f;

        totalWidth = pieceWidth * pieces.Length;

        for (int i = 0; i < pieces.Length; i++)
            pieces[i].position = new Vector3(i * pieceWidth, pieces[i].position.y, pieces[i].position.z);
    }

    private void Update()
    {
        if (!WorldScroller.Running) return;

        float delta = WorldScroller.Speed * speedMultiplier * Time.deltaTime;

        foreach (Transform piece in pieces)
        {
            piece.Translate(Vector3.left * delta);

            if (piece.position.x < -pieceWidth)
            {
                float overlap = piece.position.x + pieceWidth;
                piece.position = new Vector3((totalWidth - pieceWidth) + overlap, piece.position.y, piece.position.z);
            }
        }
    }

    public void SetBiome(int index)
    {
        if (index < 0 || index >= biomeSprites.Length) return;

        foreach (Transform piece in pieces)
        {
            if (piece.TryGetComponent<SpriteRenderer>(out var sr))
                sr.sprite = biomeSprites[index];
        }
    }
}


