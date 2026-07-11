using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Chassis.Core;

namespace Chassis.UI
{
    /// <summary>
    /// Decoupled UI controller that registers with the EventBus to update application screen states.
    /// Handles panel activation/deactivation, booster ads integration, sound/haptic settings, and interstitial frequency capping.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject failPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("MainMenu References")]
        [SerializeField] private TextMeshProUGUI streakText;

        [Header("HUD References")]
        [SerializeField] private TextMeshProUGUI levelNoText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Button shuffleButton;
        [SerializeField] private Button addSlotButton;

        [Header("Win Screen References")]
        [SerializeField] private TextMeshProUGUI winCoinsText;
        [SerializeField] private Button win2xButton;

        [Header("Fail Screen References")]
        [SerializeField] private Button continueFailButton;

        [Header("Settings References")]
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle hapticsToggle;

        private float _lastInterstitialTime = -999f;
        private Coroutine _coinsCountCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadPlayerPrefs();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeSettingsToggles();
        }

        private void OnEnable()
        {
            EventBus.Register<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Register<LevelProgressChangedEvent>(OnProgressChanged);
            EventBus.Register<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Register<LevelFailedEvent>(OnLevelFailed);
        }

        private void OnDisable()
        {
            EventBus.Unregister<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unregister<LevelProgressChangedEvent>(OnProgressChanged);
            EventBus.Unregister<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Unregister<LevelFailedEvent>(OnLevelFailed);
        }

        private void LoadPlayerPrefs()
        {
            if (!PlayerPrefs.HasKey("SoundEnabled")) PlayerPrefs.SetInt("SoundEnabled", 1);
            if (!PlayerPrefs.HasKey("HapticsEnabled")) PlayerPrefs.SetInt("HapticsEnabled", 1);
            PlayerPrefs.Save();
        }

        private void InitializeSettingsToggles()
        {
            if (soundToggle != null)
            {
                soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
                soundToggle.onValueChanged.AddListener(OnSoundToggled);
            }
            if (hapticsToggle != null)
            {
                hapticsToggle.isOn = PlayerPrefs.GetInt("HapticsEnabled", 1) == 1;
                hapticsToggle.onValueChanged.AddListener(OnHapticsToggled);
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent ev)
        {
            Debug.Log($"[UIManager] Game state transitioned to: <b>{ev.NewState}</b>. Updating active screens.");
            
            switch (ev.NewState)
            {
                case GameState.Boot:
                    SetPanelStates(false, false, false, false, false);
                    break;
                case GameState.MainMenu:
                    SetPanelStates(true, false, false, false, false);
                    UpdateStreakDisplay();
                    break;
                case GameState.Playing:
                    SetPanelStates(false, true, false, false, false);
                    UpdateHudDisplay();
                    break;
                case GameState.LevelEnd:
                    // Panel is toggled by specific LevelCompleted / LevelFailed events
                    break;
            }
        }

        private void SetPanelStates(bool menu, bool hud, bool win, bool fail, bool settings)
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(menu);
            if (hudPanel != null) hudPanel.SetActive(hud);
            if (winPanel != null) winPanel.SetActive(win);
            if (failPanel != null) failPanel.SetActive(fail);
            if (settingsPanel != null) settingsPanel.SetActive(settings);
        }

        private void UpdateStreakDisplay()
        {
            if (streakText != null)
            {
                int streak = PlayerPrefs.GetInt("WinStreak", 0);
                streakText.text = $"Streak: {streak}";
            }
        }

        private void UpdateHudDisplay()
        {
            if (levelNoText != null && GameManager.Instance != null)
            {
                levelNoText.text = $"Level {GameManager.Instance.CurrentLevelId}";
            }
            if (progressBar != null)
            {
                progressBar.value = 0f;
            }
            if (addSlotButton != null)
            {
                addSlotButton.interactable = true;
            }
        }

        private void OnProgressChanged(LevelProgressChangedEvent ev)
        {
            if (progressBar != null)
            {
                progressBar.value = ev.progress;
            }
        }

        private void OnLevelCompleted(LevelCompletedEvent ev)
        {
            SetPanelStates(false, false, true, false, false);
            if (win2xButton != null) win2xButton.interactable = true;

            int score = ev.result.score;
            int coinsEarned = score / 10; // 10 score points = 1 coin

            if (_coinsCountCoroutine != null) StopCoroutine(_coinsCountCoroutine);
            _coinsCountCoroutine = StartCoroutine(CoinCountUpCoroutine(coinsEarned));
        }

        private void OnLevelFailed(LevelFailedEvent ev)
        {
            SetPanelStates(false, false, false, true, false);
            if (continueFailButton != null) continueFailButton.interactable = true;
        }

        private IEnumerator CoinCountUpCoroutine(int targetCoins)
        {
            int current = 0;
            float duration = 1.0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                current = Mathf.RoundToInt(Mathf.Lerp(0, targetCoins, elapsed / duration));
                if (winCoinsText != null) winCoinsText.text = $"+{current} Coins";
                yield return null;
            }

            if (winCoinsText != null) winCoinsText.text = $"+{targetCoins} Coins";
        }

        #region Buttons and Listeners

        public void OnPlayClicked()
        {
            var bootstrap = FindFirstObjectByType<TileMatchTestBootstrap>();
            if (bootstrap != null)
            {
                bootstrap.StartGameplay();
            }
            else
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GoToNextLevel();
                }
            }
        }

        public void OnShuffleClicked()
        {
            if (ServiceLocator.TryGet<IAdsProvider>(out var ads))
            {
                ads.ShowRewarded("shuffle_booster", (success) =>
                {
                    if (success)
                    {
                        var mechanic = FindFirstObjectByType<TileMatchMechanic>();
                        if (mechanic != null)
                        {
                            mechanic.ShufflePile();
                        }
                    }
                });
            }
        }

        public void OnAddSlotClicked()
        {
            if (ServiceLocator.TryGet<IAdsProvider>(out var ads))
            {
                ads.ShowRewarded("add_slot_booster", (success) =>
                {
                    if (success)
                    {
                        var tileBar = FindFirstObjectByType<TileBar>();
                        if (tileBar != null)
                        {
                            tileBar.ExpandMaxSlots(8);
                        }
                        if (addSlotButton != null) addSlotButton.interactable = false;
                    }
                });
            }
        }

        public void OnWin2xClicked()
        {
            if (ServiceLocator.TryGet<IAdsProvider>(out var ads))
            {
                ads.ShowRewarded("win_2x_reward", (success) =>
                {
                    if (success)
                    {
                        Debug.Log("[UI] 2x rewards successfully claimed.");
                        if (win2xButton != null) win2xButton.interactable = false;
                    }
                });
            }
        }

        public void OnContinueFailClicked()
        {
            if (ServiceLocator.TryGet<IAdsProvider>(out var ads))
            {
                ads.ShowRewarded("continue_fail_booster", (success) =>
                {
                    if (success)
                    {
                        var tileBar = FindFirstObjectByType<TileBar>();
                        if (tileBar != null)
                        {
                            tileBar.ExpandMaxSlots(8);
                        }
                        
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.TransitionToState(GameState.Playing);
                        }
                        if (continueFailButton != null) continueFailButton.interactable = false;
                    }
                });
            }
        }

        public void OnNextLevelClicked()
        {
            if (GameManager.Instance == null) return;

            float elapsed = Time.time - _lastInterstitialTime;
            var config = ServiceLocator.Get<GameConfig>();
            float cap = config != null ? config.interstitialFrequencyCap : 30f;

            if (elapsed >= cap)
            {
                _lastInterstitialTime = Time.time;
                if (ServiceLocator.TryGet<IAdsProvider>(out var ads))
                {
                    ads.ShowInterstitial("level_end", () =>
                    {
                        GameManager.Instance.GoToNextLevel();
                    });
                }
                else
                {
                    GameManager.Instance.GoToNextLevel();
                }
            }
            else
            {
                GameManager.Instance.GoToNextLevel();
            }
        }

        public void OnRetryClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
        }

        public void OnOpenSettingsClicked()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                if (soundToggle != null) soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
                if (hapticsToggle != null) hapticsToggle.isOn = PlayerPrefs.GetInt("HapticsEnabled", 1) == 1;
            }
        }

        public void OnCloseSettingsClicked()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        public void OnPrivacyPolicyClicked()
        {
            Application.OpenURL("https://example.com/privacy-policy");
            Debug.Log("[UI] Privacy Policy opened (placeholder url).");
        }

        private void OnSoundToggled(bool value)
        {
            PlayerPrefs.SetInt("SoundEnabled", value ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"[UI] Sound Enabled set to: {value}");
        }

        private void OnHapticsToggled(bool value)
        {
            PlayerPrefs.SetInt("HapticsEnabled", value ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"[UI] Haptics Enabled set to: {value}");
        }

        #endregion
    }
}
