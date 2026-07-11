using UnityEngine;
using Chassis.Core;

namespace Chassis.UI
{
    /// <summary>
    /// Decoupled UI controller that registers with the EventBus to update application screen states.
    /// Eliminates direct references (God objects) between state components and UI layouts.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            EventBus.Register<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Register<LevelProgressChangedEvent>(OnProgressChanged);
        }

        private void OnDisable()
        {
            EventBus.Unregister<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unregister<LevelProgressChangedEvent>(OnProgressChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent ev)
        {
            Debug.Log($"[UIManager] Game state transitioned to: <b>{ev.NewState}</b>. Updating active screens.");
            
            switch (ev.NewState)
            {
                case GameState.Boot:
                    // Display splash / initialization loader
                    break;
                case GameState.MainMenu:
                    // Toggle Main Menu Panel = ON, HUD/Win/Fail = OFF
                    break;
                case GameState.Playing:
                    // Toggle HUD Panel = ON, Main Menu = OFF
                    break;
                case GameState.LevelEnd:
                    // Toggle Win/Fail screens depending on outcome
                    break;
            }
        }

        private void OnProgressChanged(LevelProgressChangedEvent ev)
        {
            Debug.Log($"[UIManager] Match Progress update received: {ev.progress * 100:F0}%");
        }
    }
}
