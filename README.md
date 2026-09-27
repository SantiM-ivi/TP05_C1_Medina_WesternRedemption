# TP05_C1_Medina_WesternRedemption

# Western Redemption

**[Jugarlo en itch.io →](https://zdra.itch.io/western-redemption)**

Juego 2D de tipo *endless runner* hecho en Unity. El jugador corre en su lugar mientras el mundo avanza hacia la izquierda: hay que saltar los obstáculos, juntar ítems para sumar puntaje y sobrevivir el mayor tiempo posible.

---

## Cómo jugar

| Acción | Teclado / Mouse | Táctil |
|---|---|---|
| Saltar | `Espacio`, `Flecha arriba` o clic izquierdo | Toque en pantalla |
| Salto largo | Mantener el botón de salto | Mantener el toque |
| Pausar / reanudar | `Esc` | (no aplica) |

- **Objetivo:** sobrevivir y acumular puntaje. El puntaje sube con el tiempo y con la velocidad del mundo.
- **Ítems:** tocar un ítem de puntaje suma puntos extra. Al recogerlo sale disparado en dirección aleatoria.
- **Power ups:** la placa de sheriff otorga invencibilidad por 5 segundos (con timer en pantalla). La cantimplora da una vida extra.
- **Vidas:** al recibir un golpe con vidas disponibles se consume una vida y se activa invencibilidad temporal. Sin vidas, game over.
- **Game Over:** desde el panel de Game Over se puede reiniciar.

---

## Cómo abrir el proyecto

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/SantiM-ivi/TP05_C1_Medina_WesternRedemption.git
   ```
2. Abrir Unity Hub, agregar la carpeta clonada y abrirla con la versión de Unity indicada. La primera vez Unity regenera la carpeta `Library`, así que tarda un rato.
3. Abrir la escena del menú: `Assets/Scenes/MainMenu.unity`.
4. Presionar **Play**. Siempre conviene arrancar desde la escena del menú, porque es donde vive el `AudioManager`.

> Ambas escenas deben estar agregadas en *File → Build Profiles → Scene List*, con `MainMenu` en índice 0 y `Game` en índice 1.

---

## Características

- **Movimiento del jugador** con salto variable (salto corto o largo según cuánto se mantiene el botón) y caída más pesada que la subida.
- **Coyote time** y **jump buffer**: pequeñas tolerancias de tiempo para que el salto se sienta justo.
- **Mundo en movimiento:** el jugador queda fijo en X y el suelo y los obstáculos se desplazan hacia la izquierda.
- **Generación de obstáculos** con *object pooling*. El intervalo mínimo entre obstáculos se calcula a partir del tiempo real de un salto completo, para que nunca haya una situación imposible. Los obstáculos rotan sobre su propio eje.
- **Parallax** con tres capas de fondo a velocidades distintas, diseñado para soportar intercambio de biomas.
- **Ítems de puntaje** con efecto de lanzamiento y escala al ser recogidos, también con *object pooling*.
- **Power ups** con pool propio: invencibilidad con timer en UI y vida extra con contador de vidas en pantalla.
- **Animaciones** del jugador (run, death, pickup) controladas por `PlayerAnimator`.
- **Menú principal** con panel de ajustes y sliders de volumen (general, música y efectos) que se guardan entre sesiones.
- **Pausa** con `Esc` y **Game Over** con reinicio de escena.
- **Audio:** música de menú y de gameplay, efectos de salto, aterrizaje, botón, ítem y game over, todo ruteado por un `AudioMixer`.

---

## Estructura de scripts

| Script | Responsabilidad |
|---|---|
| `PlayerController` | Movimiento vertical, salto variable, coyote time, jump buffer, invencibilidad. Expone los eventos `Jumped`, `Landed` y `Died`. |
| `PlayerData` | `ScriptableObject` con los valores de ajuste del jugador. |
| `PlayerAudioHandler` | Escucha los eventos del jugador y reproduce los sonidos de salto y aterrizaje. |
| `PlayerAnimator` | Controla los estados del `Animator` del jugador según los eventos de `PlayerController`. |
| `WorldScroller` | Controla la velocidad del mundo y el estado de scroll. |
| `GroundScroller` | Loop del suelo y capas de parallax con corrección de overlap. Acepta `speedMultiplier` para parallax. |
| `Obstacle` | Se mueve con el mundo, dispara `OnPlayerHit` al tocar al jugador y vuelve al pool. |
| `ObstacleRotate` | Rota el obstáculo sobre su propio eje en espacio de mundo. |
| `ObstacleSpawner` | Genera obstáculos con un pool por prefab e intervalos calculados según el salto. |
| `ScoreItem` | Ítem que suma puntaje, se lanza en dirección aleatoria al ser recogido y vuelve al pool. |
| `ScoreItemSpawner` | Genera ítems de puntaje a alturas configurables. |
| `PowerUpItem` | Ítem de power up con efecto de lanzamiento. El tipo (`Invincibility` / `ExtraLife`) se configura desde el Inspector. |
| `PowerUpSpawner` | Genera power ups con intervalos más largos que los ítems de puntaje. |
| `GameManager` | Puntaje, vidas, power ups, pausa, Game Over y reinicio. Escucha `Obstacle.OnPlayerHit`. |
| `AudioManager` | Singleton persistente: música, efectos, volúmenes y guardado en `PlayerPrefs`. |
| `MainMenuController` | Paneles del menú, sliders de volumen y carga de la escena de juego. |

---

## Decisiones de diseño

- **Eventos en lugar de referencias directas.** `PlayerController` no conoce el audio ni las animaciones: expone `Jumped`, `Landed` y `Died`, y cada sistema se suscribe de forma independiente. Del mismo modo, `Obstacle` dispara `OnPlayerHit` y `GameManager` decide qué hacer.
- **Object pooling** (`UnityEngine.Pool`) para obstáculos, ítems de puntaje y power ups.
- **Movimiento por `transform.Translate(Space.World)` en `Update`** en suelo, obstáculos e ítems, para que todos usen el mismo ciclo y no haya desfase visual.
- **Datos en un `ScriptableObject`** (`PlayerData`), así los valores de ajuste se editan sin tocar código.
- **Parallax reutilizando `GroundScroller`** con un campo `speedMultiplier`. El mismo script maneja el suelo (1.0) y las tres capas de fondo (0.1, 0.3, 0.6).
- **Compatibilidad de input:** el código detecta en compilación si está activo el Input System nuevo o el clásico.
- **Volúmenes en escala lineal (0 a 1)** para los sliders, convertidos a decibelios al aplicarlos al mixer.

---

## Configuración de audio

- El `AudioMixer` debe tener tres parámetros expuestos con estos nombres exactos: `MasterVol`, `MusicVol` y `SFXVol`.
- Los `AudioSource` de música y de efectos deben tener asignado su grupo del mixer en el campo *Output* (`Music` y `SFX`).
- El prefab del `AudioManager` va en la escena del menú y persiste entre escenas.

---

## Herramientas usadas

- Videos de YouTube para partículas, menú principal y scroll de fondo:
  - https://www.youtube.com/watch?v=3Nl8UPyODgQ
  - https://www.youtube.com/watch?v=1CXVbCbqKyg
  - https://www.youtube.com/watch?v=Wz3nbQPYwss
  - https://www.youtube.com/watch?v=ZYZfKbLxoHI
  - https://www.youtube.com/watch?v=RNoJGuujbjM
- IA para consultar y resolver temas del `AudioManager`, `WorldScroller`, `PlayerAudioHandler`, sistema de power ups y parallax.
- Documentación de Unity.

---

## Proceso de desarrollo

Comencé construyendo la base del juego Dino de Google, descargué los assets originales e inicié haciendo el movimiento del jugador y seteando el sprite, luego el scroll del fondo, las colisiones con los prefabs de los obstáculos que llevan a la derrota, después el AudioMixer con todos los sonidos y por último el menú principal. Con la base terminada cambié todo el diseño a la temática western: hice los sprites tomando referencia de Red Dead Redemption 2 (la portada del juego es una referencia directa a la del juego). En la segunda iteración sumé el parallax, las animaciones del jugador y los obstáculos, el sistema de power ups con invencibilidad y vida extra, y el efecto de lanzamiento al recoger ítems.
