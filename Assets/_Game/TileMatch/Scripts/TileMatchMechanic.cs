using System.Collections.Generic;
using UnityEngine;
using Chassis.Core;

namespace Game.TileMatch
{
    /// <summary>
    /// Core mechanic module for the Tile Match game.
    /// Implements IGameMechanic and handles key game loop ticks, interactions, and results generation.
    /// Spawns 3D physical primitives, listens to raycasted taps, feeds tiles to TileBar,
    /// applies explosive shockwaves on match pops, and publishes lifecycle events.
    /// </summary>
    public class TileMatchMechanic : MonoBehaviour, IGameMechanic
    {
        [Tooltip("The collection bar component handling slot placements and matches.")]
        [SerializeField] private TileBar tileBar;

        private LevelData _levelData;
        private bool _isPlaying;
        private int _movesMade;
        private int _score;
        private float _timeStarted;
        private int _totalSpawnedCount;

        private readonly List<TileObject> _spawnedObjects = new List<TileObject>();

        public void Initialize(LevelData levelData)
        {
            _levelData = levelData;
            _movesMade = 0;
            _score = 0;
            _isPlaying = false;

            if (tileBar == null)
            {
                tileBar = FindFirstObjectByType<TileBar>();
            }

            var gameConfig = ServiceLocator.Get<GameConfig>();
            if (tileBar != null)
            {
                tileBar.Initialize(gameConfig != null ? gameConfig.maxBarSlots : 7);
            }
            else
            {
                Debug.LogWarning("[TileMatchMechanic] TileBar component not assigned or found in the scene.");
            }

            // Clean up any lingering objects from previous runs
            ClearSpawnedTiles();

            // Load and parse level configuration
            TileMatchLevelConfig config = ParseLevelConfig(levelData);

            // Spawn the physical objects
            SpawnLevelObjects(config);

            Debug.Log($"[TileMatchMechanic] Initialized mechanic for level: {_levelData.levelId} (Difficulty: {_levelData.difficulty})");
        }

        public void StartLevel()
        {
            _isPlaying = true;
            _timeStarted = Time.time;
            Debug.Log("[TileMatchMechanic] TileMatch Gameplay Loop Started!");
        }

        public void Tick(float dt)
        {
            if (!_isPlaying) return;

            // Handle player input (clicks / taps)
            if (Input.GetMouseButtonDown(0))
            {
                HandleInput();
            }

            // Editor simulation hooks to facilitate testing and validation
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.W))
            {
                SimulateWin();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                SimulateFail();
            }
#endif
        }

        public void EndLevel(LevelResult result)
        {
            _isPlaying = false;
            ClearSpawnedTiles();
            Debug.Log($"[TileMatchMechanic] Tearing down level {result.levelId}. Result Success: {result.isSuccess}");
        }

        private void HandleInput()
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TileObject tile = hit.collider.GetComponent<TileObject>();
                if (tile != null && !tile.IsCollected)
                {
                    var gameConfig = ServiceLocator.Get<GameConfig>();
                    float flyDuration = gameConfig != null ? gameConfig.tileFlyDuration : 0.35f;

                    if (tileBar != null)
                    {
                        bool added = tileBar.TryAddTile(
                            tile,
                            flyDuration,
                            OnMatchPopped,
                            OnLevelFailed,
                            OnNearMiss
                        );

                        if (added)
                        {
                            _spawnedObjects.Remove(tile);
                            _movesMade++;
                            
                            // Immediately check if pile is empty and bar is empty
                            CheckWinCondition();
                        }
                    }
                }
            }
        }

        private void OnMatchPopped(TileObject t1, TileObject t2, TileObject t3)
        {
            // Calculate pop center position
            Vector3 popPos = Vector3.zero;
            int count = 0;
            if (t1 != null) { popPos += t1.transform.position; count++; }
            if (t2 != null) { popPos += t2.transform.position; count++; }
            if (t3 != null) { popPos += t3.transform.position; count++; }
            if (count > 0) popPos /= count;

            // Apply shockwave physics effect
            TriggerShockwave(popPos);

            // Destroy matched tile game objects
            if (t1 != null) Destroy(t1.gameObject);
            if (t2 != null) Destroy(t2.gameObject);
            if (t3 != null) Destroy(t3.gameObject);

            // Update scores
            var gameConfig = ServiceLocator.Get<GameConfig>();
            int addedScore = gameConfig != null ? gameConfig.matchScore : 100;
            _score += addedScore;

            // Publish popping event to EventBus for visual/juice systems
            EventBus.Publish(new MatchPoppedEvent
            {
                typeId = t1 != null ? t1.TypeId : 0,
                popPosition = popPos,
                scoreAdded = addedScore
            });

            // Update progress level bar
            if (_totalSpawnedCount > 0 && tileBar != null)
            {
                float progress = 1f - ((float)(_spawnedObjects.Count + tileBar.ActiveCount) / _totalSpawnedCount);
                EventBus.Publish(new LevelProgressChangedEvent { progress = Mathf.Clamp01(progress) });
            }

            // Re-evaluate win condition after match popped
            CheckWinCondition();
        }

        private void OnNearMiss()
        {
            Debug.Log("[TileMatchMechanic] Near Miss (6/7 bar slots occupied) detected!");
            EventBus.Publish(new NearMissEvent());
        }

        private void OnLevelFailed()
        {
            _isPlaying = false;

            var result = new LevelResult
            {
                levelId = _levelData.levelId,
                isSuccess = false,
                movesCount = _movesMade,
                score = _score,
                duration = Time.time - _timeStarted,
                failReason = "board_full"
            };

            Debug.Log("[TileMatchMechanic] Level failed: board full.");
            EventBus.Publish(new LevelFailedEvent { result = result });
        }

        private void CheckWinCondition()
        {
            if (_spawnedObjects.Count == 0 && (tileBar == null || tileBar.IsEmpty()))
            {
                _isPlaying = false;

                var result = new LevelResult
                {
                    levelId = _levelData.levelId,
                    isSuccess = true,
                    movesCount = _movesMade,
                    score = _score,
                    duration = Time.time - _timeStarted
                };

                Debug.Log("[TileMatchMechanic] Level completed successfully!");
                EventBus.Publish(new LevelCompletedEvent { result = result });
            }
        }

        private void TriggerShockwave(Vector3 position)
        {
            var gameConfig = ServiceLocator.Get<GameConfig>();
            if (gameConfig == null) return;

            Collider[] colliders = Physics.OverlapSphere(position, gameConfig.shockwaveRadius);
            foreach (var hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddExplosionForce(
                        gameConfig.shockwaveForce,
                        position,
                        gameConfig.shockwaveRadius,
                        gameConfig.shockwaveUpwardsModifier,
                        ForceMode.Impulse
                    );
                }
            }
        }

        private TileMatchLevelConfig ParseLevelConfig(LevelData levelData)
        {
            var config = new TileMatchLevelConfig();
            if (levelData != null && !string.IsNullOrEmpty(levelData.jsonData))
            {
                try
                {
                    config = JsonUtility.FromJson<TileMatchLevelConfig>(levelData.jsonData);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[TileMatchMechanic] Error parsing LevelConfig JSON: {ex.Message}");
                }
            }

            // Fallback content in case level data is missing/empty
            if (config.items == null || config.items.Count == 0)
            {
                config.items = new List<TileItemConfig>
                {
                    new TileItemConfig { typeId = 1, count = 6 },
                    new TileItemConfig { typeId = 2, count = 6 },
                    new TileItemConfig { typeId = 3, count = 6 }
                };
            }

            return config;
        }

        private void SpawnLevelObjects(TileMatchLevelConfig config)
        {
            Random.InitState(_levelData.levelId);

            var gameConfig = ServiceLocator.Get<GameConfig>();
            Vector2 rx = gameConfig != null ? gameConfig.spawnRangeX : new Vector2(-2f, 2f);
            Vector2 ry = gameConfig != null ? gameConfig.spawnRangeY : new Vector2(4f, 8f);
            Vector2 rz = gameConfig != null ? gameConfig.spawnRangeZ : new Vector2(-2f, 2f);

            _spawnedObjects.Clear();
            _totalSpawnedCount = 0;

            foreach (var itemConfig in config.items)
            {
                for (int i = 0; i < itemConfig.count; i++)
                {
                    PrimitiveType primitive = PrimitiveType.Cube;
                    switch (itemConfig.typeId % 4)
                    {
                        case 0: primitive = PrimitiveType.Cube; break;
                        case 1: primitive = PrimitiveType.Sphere; break;
                        case 2: primitive = PrimitiveType.Capsule; break;
                        case 3: primitive = PrimitiveType.Cylinder; break;
                    }

                    GameObject go = GameObject.CreatePrimitive(primitive);
                    go.name = $"Tile_{itemConfig.typeId}_{i}";

                    float px = Random.Range(rx.x, rx.y);
                    float py = Random.Range(ry.x, ry.y);
                    float pz = Random.Range(rz.x, rz.y);
                    go.transform.position = new Vector3(px, py, pz);
                    go.transform.rotation = Random.rotation;
                    go.transform.localScale = Vector3.one * 0.75f;

                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = go.AddComponent<Rigidbody>();
                    }
                    rb.mass = 1.0f;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                    TileObject tile = go.AddComponent<TileObject>();
                    Color color = GetColorForType(itemConfig.typeId);
                    tile.Setup(itemConfig.typeId, color);

                    _spawnedObjects.Add(tile);
                    _totalSpawnedCount++;
                }
            }

            Debug.Log($"[TileMatchMechanic] Spawned {_totalSpawnedCount} tile physics objects.");
        }

        private Color GetColorForType(int typeId)
        {
            float hue = (typeId * 0.1618f) % 1.0f;
            return Color.HSVToRGB(hue, 0.85f, 0.9f);
        }

        private void ClearSpawnedTiles()
        {
            foreach (var tile in _spawnedObjects)
            {
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                }
            }
            _spawnedObjects.Clear();

            if (tileBar != null)
            {
                tileBar.ClearBar();
            }

            // Cleanup any stray objects that might have been spawned dynamically in previous play tests
            var existingObjects = FindObjectsByType<TileObject>(FindObjectsSortMode.None);
            foreach (var tile in existingObjects)
            {
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                }
            }
        }

        private void SimulateWin()
        {
            var result = new LevelResult
            {
                levelId = _levelData.levelId,
                isSuccess = true,
                movesCount = _movesMade,
                score = _score + 500,
                duration = Time.time - _timeStarted
            };
            EventBus.Publish(new LevelCompletedEvent { result = result });
        }

        private void SimulateFail()
        {
            var result = new LevelResult
            {
                levelId = _levelData.levelId,
                isSuccess = false,
                movesCount = _movesMade,
                score = _score,
                duration = Time.time - _timeStarted,
                failReason = "simulated"
            };
            EventBus.Publish(new LevelFailedEvent { result = result });
        }
    }
}
