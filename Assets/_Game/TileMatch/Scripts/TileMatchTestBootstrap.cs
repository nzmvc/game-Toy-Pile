using UnityEngine;
using Chassis.Core;

namespace Game.TileMatch
{
    /// <summary>
    /// Helper component to easily bootstrap and play test the mechanics inside the Main Scene.
    /// Spawns a transient LevelData and starts the game loop via GameManager.
    /// </summary>
    public class TileMatchTestBootstrap : MonoBehaviour
    {
        [Tooltip("Active gameplay mechanic implementation in the scene.")]
        [SerializeField] private TileMatchMechanic mechanic;

        private void Start()
        {
            if (mechanic == null)
            {
                mechanic = FindFirstObjectByType<TileMatchMechanic>();
            }
        }

        /// <summary>
        /// Starts the bootstrapped level loop when triggered by the UI play button.
        /// </summary>
        public void StartGameplay()
        {
            BootstrapGame();
        }

        private void BootstrapGame()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[Bootstrap] GameManager Instance is not found in the scene! Make sure to start the play mode from the 'Boot' scene.");
                return;
            }

            if (mechanic == null)
            {
                Debug.LogError("[Bootstrap] No TileMatchMechanic component assigned or found in the scene.");
                return;
            }

            // Create a temporary scriptable level configuration for test purposes
            var testLevelData = ScriptableObject.CreateInstance<LevelData>();
            testLevelData.levelId = 1;
            testLevelData.mechanicId = "tile_match";
            testLevelData.difficulty = 1;
            testLevelData.jsonData = "{}";

            Debug.Log("[Bootstrap] Test Level Setup Complete. Initializing GameManager start sequence.");
            GameManager.Instance.StartGame(mechanic, testLevelData);
        }
    }
}
