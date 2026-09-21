# TP05_C1_Medina_WesternRedemption

# Western Redemption

Juego 2D de tipo *endless runner* hecho en Unity. El jugador corre en su lugar mientras el mundo avanza hacia la izquierda: hay que saltar los obstáculos, juntar ítems para sumar puntaje y sobrevivir el mayor tiempo posible.


## Cómo jugar

| Acción | Teclado / Mouse | Táctil |
|---|---|---|
| Saltar | `Espacio`, `Flecha arriba` o clic izquierdo | Toque en pantalla |
| Salto largo | Mantener el botón de salto | Mantener el toque |
| Pausar / reanudar | `Esc` | (no aplica) |

- **Objetivo:** sobrevivir y acumular puntaje. El puntaje sube con el tiempo y con la velocidad del mundo.
- **Ítems:** tocar un ítem de puntaje suma puntos extra.
- **Game Over:** al chocar con un obstáculo termina la partida. Desde el panel de Game Over se puede reiniciar.

---

## Cómo abrir el proyecto

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/SantiM-ivi/TP05_C1_Medina_WesternRedemption.git
   ```
2. Abrir Unity Hub, agregar la carpeta clonada y abrirla con la versión de Unity indicada arriba. La primera vez Unity regenera la carpeta `Library`, así que tarda un rato.
3. Abrir la escena del menú: `Assets/Scenes/[NombreEscenaMenu].unity`.
4. Presionar **Play**. Siempre conviene arrancar desde la escena del menú, porque es donde vive el `AudioManager`.

> La escena de juego debe llamarse `Game`, o coincidir con el campo *Game Scene Name* de `MainMenuController`. Ambas escenas deben estar agregadas en *File → Build Profiles → Scene List*.

---

## Características

- **Movimiento del jugador** con salto variable (salto corto o largo según cuánto se mantiene el botón) y caída más pesada que la subida.
- **Coyote time** y **jump buffer**: pequeñas tolerancias de tiempo para que el salto se sienta justo.
- **Mundo en movimiento:** el jugador queda fijo en X y el suelo y los obstáculos se desplazan hacia la izquierda.
- **Generación de obstáculos** con *object pooling*. El intervalo mínimo entre obstáculos se calcula a partir del tiempo real de un salto completo, para que nunca haya una situación imposible.
- **Ítems de puntaje** que aparecen a distintas alturas, también con *object pooling*.
- **Menú principal** con panel de ajustes y sliders de volumen (general, música y efectos) que se guardan entre sesiones.
- **Pausa** con `Esc` y **Game Over** con reinicio de escena.
- **Audio:** música de menú y de gameplay, efectos de salto, aterrizaje, botón, ítem y game over, todo ruteado por un `AudioMixer`.

---

## Estructura de scripts

| Script | Responsabilidad |
|---|---|
| `PlayerController` | Movimiento vertical, salto variable, coyote time, jump buffer, doble salto (vía `MaxJumps`). Expone los eventos `Jumped` y `Landed`. |
| `PlayerData` | `ScriptableObject` con los valores de ajuste del jugador (fuerza de salto, gravedad, tiempos). |
| `PlayerAudioHandler` | Escucha los eventos del jugador y reproduce los sonidos de salto y aterrizaje. |
| `WorldScroller` | Controla la velocidad del mundo y el estado de scroll (`Speed`, `Running`, `StopScrolling`). |
| `Obstacle` | Se mueve con el mundo, dispara `OnPlayerHit` al tocar al jugador y vuelve al pool. |
| `ObstacleSpawner` | Genera obstáculos con un pool por prefab e intervalos calculados según el salto. |
| `ScoreItem` | Ítem que suma puntaje al tocar al jugador y vuelve al pool. |
| `ScoreItemSpawner` | Genera ítems de puntaje a alturas configurables. |
| `GameManager` | Puntaje, pausa, Game Over y reinicio. Escucha `Obstacle.OnPlayerHit`. |
| `AudioManager` | Singleton persistente: música, efectos, volúmenes y guardado en `PlayerPrefs`. |
| `MainMenuController` | Paneles del menú, sliders de volumen y carga de la escena de juego. |



Decisiones de diseño

- **Eventos en lugar de referencias directas.** `PlayerController` no conoce el audio: expone `Jumped` y `Landed`, y `PlayerAudioHandler` se suscribe. Del mismo modo, `Obstacle` dispara `OnPlayerHit` y `GameManager` decide qué hacer. Esto reduce el acoplamiento entre sistemas.
- **Object pooling** (`UnityEngine.Pool`) para obstáculos e ítems, para evitar crear y destruir objetos constantemente.
- **Movimiento por `transform.Translate` en `Update`** en suelo, obstáculos e ítems, para que todos usen el mismo ciclo y no haya desfase visual entre ellos.
- **Datos en un `ScriptableObject`** (`PlayerData`), así los valores de ajuste se editan sin tocar código.
- **Compatibilidad de input:** el código detecta en compilación si está activo el Input System nuevo o el clásico.
- **Volúmenes en escala lineal (0 a 1)** para los sliders, convertidos a decibelios al aplicarlos al mixer.


Configuración de audio

- El `AudioMixer` debe tener tres parámetros expuestos con estos nombres exactos: `MasterVol`, `MusicVol` y `SFXVol`.
- Los `AudioSource` de música y de efectos deben tener asignado su grupo del mixer en el campo *Output* (`Music` y `SFX`).
- El prefab del `AudioManager` va en la escena del menú y persiste entre escenas.
- Los eventos `On Click` de los botones no deben reproducir la música como efecto. Toda la música sale por `AudioManager.PlayMenuMusic()` y `AudioManager.PlayGameplayMusic()`.


