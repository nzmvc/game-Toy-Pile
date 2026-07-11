using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chassis.Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Playing,
        LevelEnd
    }

    /// <summary>
    /// Central manager overseeing the high-level game state machine and scene flows.
    /// Manages the application lifecycle, service registrations, and mechanic executions.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene Configuration")]
        [SerializeField] private string bootSceneName = "Boot";
        [SerializeField] private string mainSceneName = "Main";

        [Header("Balance Configuration")]
        [SerializeField] private GameConfig gameConfig;

        public GameState CurrentState { get; private set; } = GameState.Boot;
        public IGameMechanic ActiveMechanic { get; private set; }
        public int CurrentLevelId => _currentLevelId;

        private float _levelStartTime;
        private int _attemptNo = 1;
        private int _currentLevelId = 1;
        
        // Caching level data for restart purposes
        private LevelData _currentLevelData;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeServices();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            TransitionToState(GameState.Boot);
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing && ActiveMechanic != null)
            {
                ActiveMechanic.Tick(Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            EventBus.Register<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Register<LevelFailedEvent>(OnLevelFailed);
        }

        private void OnDisable()
        {
            EventBus.Unregister<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Unregister<LevelFailedEvent>(OnLevelFailed);
        }

        private void InitializeServices()
        {
            // Register default systems to the locator
            ServiceLocator.Register<IAnalyticsProvider>(new ConsoleAnalyticsProvider());
            
            if (gameConfig != null)
            {
                ServiceLocator.Register<GameConfig>(gameConfig);
            }
            else
            {
                Debug.LogWarning("[GameManager] GameConfig is not assigned in the GameManager inspector!");
            }
            
            var adsProvider = new DummyAdsProvider();
            ServiceLocator.Register<IAdsProvider>(adsProvider);
            adsProvider.Initialize(() => Debug.Log("[GameManager] Ads Provider initialized."));
        }

        public void TransitionToState(GameState newState)
        {
            Debug.Log($"[GameManager] Transitioning State: <b>{CurrentState}</b> -> <b>{newState}</b>");
            ExitState(CurrentState);
            CurrentState = newState;
            EnterState(CurrentState);

            // Publish state change event for UI and other decoupled systems
            EventBus.Publish(new GameStateChangedEvent { NewState = newState });
        }

        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Boot:
                    Debug.Log("[GameManager] System Boot initialized.");
                    LoadMainScene();
                    break;

                case GameState.MainMenu:
                    Debug.Log("[GameManager] Main Menu active. Idle state.");
                    break;

                case GameState.Playing:
                    Debug.Log($"[GameManager] Level {_currentLevelId} match active.");
                    _levelStartTime = Time.time;

                    // Log level_start to analytics
                    if (ServiceLocator.TryGet<IAnalyticsProvider>(out var analytics))
                    {
                        var startParams = new Dictionary<string, object>
                        {
                            { AnalyticsEvents.Params.LevelId, _currentLevelId },
                            { AnalyticsEvents.Params.MechanicId, ActiveMechanic?.GetType().Name ?? "unknown" },
                            { AnalyticsEvents.Params.AttemptNo, _attemptNo }
                        };
                        analytics.LogEvent(AnalyticsEvents.LevelStart, startParams);
                    }

                    if (ActiveMechanic != null)
                    {
                        ActiveMechanic.StartLevel();
                    }
                    break;

                case GameState.LevelEnd:
                    Debug.Log("[GameManager] Game segment completed.");
                    break;
            }
        }

        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    // Cleanup can be performed generic or via mechanic teardown
                    break;
            }
        }

        private void LoadMainScene()
        {
            // Check if we are already in the Main Scene to prevent redundant loading
            if (SceneManager.GetActiveScene().name == mainSceneName)
            {
                TransitionToState(GameState.MainMenu);
                return;
            }

            Debug.Log($"[GameManager] Loading scene: {mainSceneName}");
            var loadOperation = SceneManager.LoadSceneAsync(mainSceneName);
            loadOperation.completed += (op) =>
            {
                Debug.Log($"[GameManager] Scene {mainSceneName} loaded successfully.");
                TransitionToState(GameState.MainMenu);
            };
        }

        public void StartGame(IGameMechanic mechanic, LevelData data)
        {
            if (CurrentState != GameState.MainMenu)
            {
                Debug.LogWarning("[GameManager] Execution order error. Games can only start from MainMenu.");
                return;
            }

            ActiveMechanic = mechanic;
            _currentLevelData = data;
            
            // Set level properties from data overrides if any
            if (data != null)
            {
                _currentLevelId = data.levelId;
            }

            ActiveMechanic.Initialize(data);
            TransitionToState(GameState.Playing);
        }

        private void OnLevelCompleted(LevelCompletedEvent ev)
        {
            if (CurrentState != GameState.Playing) return;

            float duration = Time.time - _levelStartTime;
            ev.result.duration = duration;
            ev.result.levelId = _currentLevelId;
            ev.result.isSuccess = true;

            ActiveMechanic?.EndLevel(ev.result);

            // Log level_complete to analytics
            if (ServiceLocator.TryGet<IAnalyticsProvider>(out var analytics))
            {
                var completionParams = new Dictionary<string, object>
                {
                    { AnalyticsEvents.Params.LevelId, _currentLevelId },
                    { AnalyticsEvents.Params.Duration, duration },
                    { AnalyticsEvents.Params.Moves, ev.result.movesCount }
                };
                analytics.LogEvent(AnalyticsEvents.LevelComplete, completionParams);
            }

            _currentLevelId++;
            _attemptNo = 1;

            TransitionToState(GameState.LevelEnd);
        }

        private void OnLevelFailed(LevelFailedEvent ev)
        {
            if (CurrentState != GameState.Playing) return;

            float duration = Time.time - _levelStartTime;
            ev.result.duration = duration;
            ev.result.levelId = _currentLevelId;
            ev.result.isSuccess = false;

            ActiveMechanic?.EndLevel(ev.result);

            // Log level_fail to analytics
            if (ServiceLocator.TryGet<IAnalyticsProvider>(out var analytics))
            {
                var failParams = new Dictionary<string, object>
                {
                    { AnalyticsEvents.Params.LevelId, _currentLevelId },
                    { AnalyticsEvents.Params.Duration, duration },
                    { AnalyticsEvents.Params.FailReason, ev.result.failReason ?? "failed" }
                };
                analytics.LogEvent(AnalyticsEvents.LevelFail, failParams);
            }

            _attemptNo++;

            TransitionToState(GameState.LevelEnd);
        }

        public void RestartLevel()
        {
            if (CurrentState != GameState.LevelEnd) return;
            TransitionToState(GameState.MainMenu);
        }

        public void GoToNextLevel()
        {
            if (CurrentState != GameState.LevelEnd) return;
            TransitionToState(GameState.MainMenu);
        }
    }

    public struct GameStateChangedEvent
    {
        public GameState NewState;
    }
}
