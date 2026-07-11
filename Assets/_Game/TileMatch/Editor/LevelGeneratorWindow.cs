using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Chassis.Core;

namespace Game.TileMatch.Editor
{
    /// <summary>
    /// Custom Editor Window providing tool GUI to generate, validate, and batch-create LevelData assets
    /// for levels 1 to 30. Auto-discovers GameConfig and validates output using SolvabilityBot.
    /// </summary>
    public class LevelGeneratorWindow : EditorWindow
    {
        private int startLevel = 1;
        private int endLevel = 30;
        private GameConfig gameConfig;

        [MenuItem("Window/Chassis/Level Generator")]
        public static void ShowWindow()
        {
            GetWindow<LevelGeneratorWindow>("Level Generator");
        }

        private void OnEnable()
        {
            // Auto-locate GameConfig inside the project
            if (gameConfig == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:GameConfig");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(path);
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Tile Match Seviye Üretici", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            gameConfig = (GameConfig)EditorGUILayout.ObjectField("Game Config", gameConfig, typeof(GameConfig), false);

            EditorGUILayout.Space();
            startLevel = EditorGUILayout.IntField("Başlangıç Seviyesi", startLevel);
            endLevel = EditorGUILayout.IntField("Bitiş Seviyesi", endLevel);

            EditorGUILayout.Space();
            if (GUILayout.Button("Seviyeleri Üret ve Doğrula (Generate & Validate)"))
            {
                GenerateAllLevels();
            }
        }

        private void GenerateAllLevels()
        {
            if (gameConfig == null)
            {
                EditorUtility.DisplayDialog("Hata", "Lütfen bir GameConfig atayın veya oluşturun!", "Tamam");
                return;
            }

            int successCount = 0;
            string folderPath = "Assets/_Content/Levels";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            for (int lvl = startLevel; lvl <= endLevel; lvl++)
            {
                DifficultyService.Parameters param = DifficultyService.GetParameters(lvl, gameConfig);
                bool levelCreated = false;

                // Try seeds: lvl, lvl + 1000, lvl + 2000... to find a physically solvable layout
                for (int attempt = 0; attempt < 100; attempt++)
                {
                    int seed = lvl + (attempt * 1000);
                    TileMatchLevelConfig levelConfig = new TileMatchLevelConfig();
                    levelConfig.items = GenerateItems(param.totalObjects, param.typeCount, seed);

                    // Test playability using the bot
                    if (SolvabilityBot.IsSolvable(levelConfig, seed, gameConfig, out int moves))
                    {
                        SaveLevelAsset(lvl, levelConfig, folderPath);
                        Debug.Log($"[LevelGenerator] Seviye {lvl} üretildi! Seed: {seed}, Çözüm Adımı: {moves}, Obje Sayısı: {param.totalObjects}, Tür Sayısı: {param.typeCount}");
                        levelCreated = true;
                        successCount++;
                        break;
                    }
                }

                if (!levelCreated)
                {
                    Debug.LogError($"[LevelGenerator] Seviye {lvl} için çözülebilir bir seed bulunamadı!");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Başarılı", $"{successCount} adet seviye başarıyla üretildi ve doğrulandı!", "Harika");
        }

        private List<TileItemConfig> GenerateItems(int totalObjects, int typeCount, int seed)
        {
            var random = new System.Random(seed);
            var items = new List<TileItemConfig>();

            // Give each type a baseline count of 3
            for (int t = 1; t <= typeCount; t++)
            {
                items.Add(new TileItemConfig { typeId = t, count = 3 });
            }

            // Distribute remaining count in chunks of 3
            int remaining = totalObjects - (typeCount * 3);
            while (remaining > 0)
            {
                int randIndex = random.Next(0, typeCount);
                items[randIndex].count += 3;
                remaining -= 3;
            }

            return items;
        }

        private void SaveLevelAsset(int levelId, TileMatchLevelConfig config, string folderPath)
        {
            string assetPath = $"{folderPath}/Level_{levelId}.asset";
            LevelData asset = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
            bool isNew = false;

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<LevelData>();
                isNew = true;
            }

            asset.levelId = levelId;
            asset.mechanicId = "tile_match";
            asset.difficulty = config.items.Count;
            asset.jsonData = JsonUtility.ToJson(config, true);

            if (isNew)
            {
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            else
            {
                EditorUtility.SetDirty(asset);
            }
        }
    }
}
