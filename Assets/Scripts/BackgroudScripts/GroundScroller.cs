using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private Transform[] pieces;

    private float pieceWidth;
    private float totalWidth;

    private void Awake()
    {
       
        if (pieces.Length > 0 && pieces[0].TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            pieceWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
           
            pieceWidth = 20f;
        }

        totalWidth = pieceWidth * pieces.Length;

        
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].position = new Vector3(i * pieceWidth, pieces[i].position.y, pieces[i].position.z);
        }
    }

    private void Update()
    {
     
        if (!WorldScroller.Running) return;

        float movement = WorldScroller.Speed * Time.deltaTime;

        foreach (Transform piece in pieces)
        {
           
            piece.Translate(Vector3.left * movement);

            
            if (piece.position.x < -pieceWidth)
            {
                
                float overlap = piece.position.x + pieceWidth;
                piece.position = new Vector3((totalWidth - pieceWidth) + overlap, piece.position.y, piece.position.z);
            }
        }
    }
}

