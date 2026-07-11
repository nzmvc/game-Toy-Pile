using System.Collections.Generic;
using UnityEngine;
using Chassis.Core;

namespace Game.TileMatch
{
    /// <summary>
    /// Greedy solver simulation bot. Checks if a level's physical pile layout (simulated via Y coordinates)
    /// is 100% solvable within the 7-slot bar limitation, considering only the top K tiles visible.
    /// </summary>
    public static class SolvabilityBot
    {
        /// <summary>
        /// Mock representation of a physics tile for simulation purposes.
        /// </summary>
        public class MockTile
        {
            public int typeId;
            public float y; // Height coordinate, determining pile depth/accessibility
        }

        /// <summary>
        /// Validates if a level configuration is solvable under the given seed.
        /// </summary>
        /// <param name="config">The parsed level configuration.</param>
        /// <param name="seed">The generation seed (corresponds to levelId).</param>
        /// <param name="gameConfig">The balance settings (spawn bounds and bar counts).</param>
        /// <param name="movesTaken">Output count of moves the bot took to solve the level.</param>
        /// <returns>True if the level is successfully solved, false if the bar overflows.</returns>
        public static bool IsSolvable(TileMatchLevelConfig config, int seed, GameConfig gameConfig, out int movesTaken)
        {
            movesTaken = 0;
            List<MockTile> pile = GenerateMockTiles(config, seed, gameConfig);

            // Sort the pile by Y coordinate in descending order (highest Y represents the top tiles)
            pile.Sort((a, b) => b.y.CompareTo(a.y));

            List<int> bar = new List<int>();
            int maxBarSlots = gameConfig != null ? gameConfig.maxBarSlots : 7;
            int lookaheadCount = 6; // Simulates physical occlusion (only top K tiles can be tapped)

            int safetyCounter = 0;
            while (pile.Count > 0 || bar.Count > 0)
            {
                safetyCounter++;
                if (safetyCounter > 1500)
                {
                    return false; // Loop overflow prevention
                }

                if (bar.Count >= maxBarSlots)
                {
                    return false; // Bar overflow (player loses)
                }

                if (pile.Count == 0 && bar.Count > 0)
                {
                    return false; // Pile is empty but unmatched tiles remain in the bar
                }

                // Identify currently accessible tiles (top K remaining in pile)
                int availableCount = Mathf.Min(lookaheadCount, pile.Count);

                int bestIndex = -1;
                int highestPriority = -1;

                for (int i = 0; i < availableCount; i++)
                {
                    int typeId = pile[i].typeId;
                    int countInBar = CountInBar(bar, typeId);

                    int priority;
                    if (countInBar == 2)
                    {
                        priority = 3; // Completes a triplet (highest priority, frees space)
                    }
                    else if (countInBar == 1)
                    {
                        priority = 2; // Pairs up (second priority)
                    }
                    else
                    {
                        priority = 1; // Neutral (lowest priority)
                    }

                    // Select the tile with the highest priority. If equal, pick the one higher in the pile (smaller index)
                    if (priority > highestPriority)
                    {
                        highestPriority = priority;
                        bestIndex = i;
                    }
                }

                if (bestIndex == -1)
                {
                    return false; // Fallback failure
                }

                // Pick the chosen tile
                MockTile chosen = pile[bestIndex];
                pile.RemoveAt(bestIndex);

                // Add to bar and register move
                bar.Add(chosen.typeId);
                movesTaken++;

                // Process matches (pop triplets)
                CheckMatches(bar);
            }

            return true;
        }

        private static List<MockTile> GenerateMockTiles(TileMatchLevelConfig config, int seed, GameConfig gameConfig)
        {
            // Initialize random generator with the exact same seed sequence as the mechanic spawner
            Random.InitState(seed);

            Vector2 rx = gameConfig != null ? gameConfig.spawnRangeX : new Vector2(-2f, 2f);
            Vector2 ry = gameConfig != null ? gameConfig.spawnRangeY : new Vector2(4f, 8f);
            Vector2 rz = gameConfig != null ? gameConfig.spawnRangeZ : new Vector2(-2f, 2f);

            var list = new List<MockTile>();

            foreach (var itemConfig in config.items)
            {
                for (int i = 0; i < itemConfig.count; i++)
                {
                    // Maintain exact random state sequence
                    float px = Random.Range(rx.x, rx.y);
                    float py = Random.Range(ry.x, ry.y);
                    float pz = Random.Range(rz.x, rz.y);
                    Quaternion rot = Random.rotation;

                    list.Add(new MockTile
                    {
                        typeId = itemConfig.typeId,
                        y = py
                    });
                }
            }

            return list;
        }

        private static int CountInBar(List<int> bar, int typeId)
        {
            int count = 0;
            for (int i = 0; i < bar.Count; i++)
            {
                if (bar[i] == typeId) count++;
            }
            return count;
        }

        private static void CheckMatches(List<int> bar)
        {
            for (int i = 0; i < bar.Count; i++)
            {
                int typeId = bar[i];
                if (CountInBar(bar, typeId) == 3)
                {
                    bar.RemoveAll(t => t == typeId);
                    break;
                }
            }
        }
    }
}
