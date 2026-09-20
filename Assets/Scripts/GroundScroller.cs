using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private Transform[] pieces;

    private float pieceWidth;
    private float totalWidth;

    private void Awake()
    {
        // 1. Buscamos el ancho REAL del sprite para que encajen perfecto sin importar la cámara
        if (pieces.Length > 0 && pieces[0].TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            pieceWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
            // Salvavidas por si no hay SpriteRenderer
            pieceWidth = 20f;
        }

        totalWidth = pieceWidth * pieces.Length;

        // 2. Acomodamos las piezas en fila india de forma automática en el inicio
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].position = new Vector3(i * pieceWidth, pieces[i].position.y, pieces[i].position.z);
        }
    }

    private void Update()
    {
        // Si el juego está pausado o el scroller detenido, no hacemos nada
        if (!WorldScroller.Running) return;

        float movement = WorldScroller.Speed * Time.deltaTime;

        foreach (Transform piece in pieces)
        {
            // Movemos la pieza hacia la izquierda
            piece.Translate(Vector3.left * movement);

            // 3. El truco del Smooth: Si la pieza pasó el límite izquierdo...
            if (piece.position.x < -pieceWidth)
            {
                // En vez de sumarle un valor fijo, calculamos el exceso exacto que se pasó 
                // de la pantalla en este frame y lo compensamos al mandarla a la derecha.
                float overlap = piece.position.x + pieceWidth;
                piece.position = new Vector3((totalWidth - pieceWidth) + overlap, piece.position.y, piece.position.z);
            }
        }
    }
}

/*
 * DECISIONES DE DISEÑO (Para la bitácora)
 *
 * ANCHO BASADO EN SPRITE (Chau gap visual)
 * Reemplacé el cálculo de la cámara por spriteRenderer.bounds.size.x. 
 * Medir la pantalla fallaba si el sprite no escalaba perfecto. Midiendo el 
 * renderizador nos aseguramos de que el encastre entre piezas sea milimétrico.
 * 
 * BUCLE AUTOMÁTICO DINÁMICO
 * Ahora el Awake posiciona CUALQUIER cantidad de piezas en fila (pieces.Length). 
 * Podés usar 2, 3 o 5 piezas y el script las acomoda solas a lo ancho sin tocar nada.
 * 
 * CORRECCIÓN DE OVERLAP (El secreto del Smooth)
 * Si una pieza se pasa del límite, restarle o sumarle un valor fijo rompe el loop 
 * a los pocos segundos por la pérdida de precisión de los floats en Unity. 
 * Calculando 'overlap' (cuánto se pasó en ese frame exacto) y sumándolo al reposicionar, 
 * el scroll se vuelve infinitamente fluido y no se nota nunca el salto.
 * 
 * COMPROBACIÓN DE RUNNING
 * Le agregué la condición de WorldScroller.Running. Si el jugador pierde y el scroller 
 * se frena, las piezas dejan de moverse al unísono inmediatamente.
 */
