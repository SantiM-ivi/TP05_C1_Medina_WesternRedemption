using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private Transform[] pieces;
    [SerializeField] private bool autoDetectWidth = true;
    [SerializeField] private float manualWidth = 20f;

    private float pieceWidth;

    private void Awake()
    {
        pieceWidth = autoDetectWidth
            ? Camera.main.orthographicSize * 2f * Camera.main.aspect
            : manualWidth;

        if (pieces.Length >= 2)
            pieces[1].position = new Vector3(pieceWidth, pieces[1].position.y, pieces[1].position.z);
    }

    private void Update()
    {
        foreach (Transform piece in pieces)
        {
            piece.Translate(Vector3.left * WorldScroller.Speed * Time.deltaTime);

            if (piece.position.x < -pieceWidth)
                piece.position += Vector3.right * pieceWidth * pieces.Length;
        }
    }
}

/*
 * DECISIONES DE DISEÑO
 *
 * ANCHO AUTOMÁTICO DESDE LA CÁMARA
 * Con autoDetectWidth ON, el ancho se calcula como:
 *   orthographicSize * 2 * aspect
 * Esto da exactamente el ancho visible de la cámara ortográfica en
 * unidades de mundo, sin depender de un valor manual que hay que
 * mantener sincronizado con el sprite.
 *
 * DOS PIEZAS
 * Se necesitan dos GameObjects con el mismo sprite. Cuando el de la
 * izquierda sale de pantalla, se teletransporta a la derecha del otro.
 * El salto es de pieceWidth * 2 (largo total del loop), así siempre
 * hay una pieza cubriendo la pantalla visible.
 *
 * POSICIONAMIENTO AUTOMÁTICO DE pieces[1]
 * En Awake, la segunda pieza se posiciona en X = pieceWidth para que
 * no haya que hacerlo a mano en el editor. Solo hay que dejar las dos
 * piezas en X=0 y el script las acomoda.
 *
 * WRAP MODE REPEAT
 * El Wrap Mode del sprite no afecta este script porque cada pieza
 * es un GameObject separado con su propio SpriteRenderer. El Repeat
 * entra en juego si se quiere escalar el sprite más allá de 1x1 en
 * un material tiling. Para este enfoque de dos piezas no es necesario.
 *
 * SETUP EN ESCENA
 * - Crear dos GameObjects de suelo, cada uno con el sprite que cubre
 *   la pantalla completa y un BoxCollider2D en layer Ground.
 * - Dejar ambos en X=0 (Awake posiciona el segundo automáticamente).
 * - Asignarlos al array pieces en orden: primero el izquierdo, después el derecho.
 * - Dejar autoDetectWidth en true si la cámara es ortográfica estándar.
 */