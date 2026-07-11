using NUnit.Framework;
using UnityEngine;
using Chassis.Core;
using Chassis.UI;

namespace Chassis.UI.Tests
{
    [TestFixture]
    public class UIManagerTests
    {
        private GameObject _uiManagerGo;
        private UIManager _uiManager;

        private GameObject _mainMenuPanel;
        private GameObject _hudPanel;
        private GameObject _winPanel;
        private GameObject _failPanel;
        private GameObject _settingsPanel;

        [SetUp]
        public void SetUp()
        {
            _uiManagerGo = new GameObject("Test_UIManager");
            _uiManager = _uiManagerGo.AddComponent<UIManager>();

            // Create dummy panels and set them to UIManager via Reflection
            _mainMenuPanel = new GameObject("MainMenuPanel");
            _hudPanel = new GameObject("HudPanel");
            _winPanel = new GameObject("WinPanel");
            _failPanel = new GameObject("FailPanel");
            _settingsPanel = new GameObject("SettingsPanel");

            // Parents under manager to keep clean
            _mainMenuPanel.transform.SetParent(_uiManagerGo.transform);
            _hudPanel.transform.SetParent(_uiManagerGo.transform);
            _winPanel.transform.SetParent(_uiManagerGo.transform);
            _failPanel.transform.SetParent(_uiManagerGo.transform);
            _settingsPanel.transform.SetParent(_uiManagerGo.transform);

            // Reflectively set serializable fields
            SetPrivateField(_uiManager, "mainMenuPanel", _mainMenuPanel);
            SetPrivateField(_uiManager, "hudPanel", _hudPanel);
            SetPrivateField(_uiManager, "winPanel", _winPanel);
            SetPrivateField(_uiManager, "failPanel", _failPanel);
            SetPrivateField(_uiManager, "settingsPanel", _settingsPanel);
            
            // Enable UIManager to register EventBus
            _uiManagerGo.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_uiManagerGo);
        }

        [Test]
        public void GameState_MainMenu_Activates_MainMenuPanel_Only()
        {
            // Act - publish event
            EventBus.Publish(new GameStateChangedEvent { NewState = GameState.MainMenu });

            // Assert
            Assert.IsTrue(_mainMenuPanel.activeSelf, "MainMenu panel should be active.");
            Assert.IsFalse(_hudPanel.activeSelf, "HUD panel should be inactive.");
            Assert.IsFalse(_winPanel.activeSelf, "Win panel should be inactive.");
            Assert.IsFalse(_failPanel.activeSelf, "Fail panel should be inactive.");
            Assert.IsFalse(_settingsPanel.activeSelf, "Settings panel should be inactive.");
        }

        [Test]
        public void GameState_Playing_Activates_HUDPanel_Only()
        {
            // Act - publish event
            EventBus.Publish(new GameStateChangedEvent { NewState = GameState.Playing });

            // Assert
            Assert.IsFalse(_mainMenuPanel.activeSelf, "MainMenu panel should be inactive.");
            Assert.IsTrue(_hudPanel.activeSelf, "HUD panel should be active.");
            Assert.IsFalse(_winPanel.activeSelf, "Win panel should be inactive.");
            Assert.IsFalse(_failPanel.activeSelf, "Fail panel should be inactive.");
            Assert.IsFalse(_settingsPanel.activeSelf, "Settings panel should be inactive.");
        }

        [Test]
        public void GameState_Boot_Deactivates_AllPanels()
        {
            // Act - publish event
            EventBus.Publish(new GameStateChangedEvent { NewState = GameState.Boot });

            // Assert
            Assert.IsFalse(_mainMenuPanel.activeSelf, "MainMenu panel should be inactive.");
            Assert.IsFalse(_hudPanel.activeSelf, "HUD panel should be inactive.");
            Assert.IsFalse(_winPanel.activeSelf, "Win panel should be inactive.");
            Assert.IsFalse(_failPanel.activeSelf, "Fail panel should be inactive.");
            Assert.IsFalse(_settingsPanel.activeSelf, "Settings panel should be inactive.");
        }

        private void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                Debug.LogError($"Field {fieldName} not found on {target.GetType()}");
            }
        }
    }
}
