using UnityEngine;
using Chassis.Core;

namespace Game.TileMatch
{
    /// <summary>
    /// Service responsible for calculating level difficulty parameters (total objects, unique type counts, stack density)
    /// based on the level index and curves defined in GameConfig.
    /// Handles breath level drops to offer players a relaxation step.
    /// </summary>
    public static class DifficultyService
    {
        /// <summary>
        /// Struct containing calculated level design variables.
        /// </summary>
        public struct Parameters
        {
            public int levelId;
            public int totalObjects; // Enforced to be a multiple of 3
            public int typeCount;    // Enforced to fit slot constraint and object counts
            public float stackDensity;
        }

        /// <summary>
        /// Computes difficulty parameters for the given level.
        /// </summary>
        public static Parameters GetParameters(int levelId, GameConfig config)
        {
            if (config == null)
            {
                // Core fallbacks if configuration is unavailable
                return new Parameters
                {
                    levelId = levelId,
                    totalObjects = 18,
                    typeCount = 3,
                    stackDensity = 1.0f
                };
            }

            // Retrieve raw parameters from AnimationCurves
            float rawObjects = config.totalObjectsCurve.Evaluate(levelId);
            float rawTypes = config.typeCountCurve.Evaluate(levelId);

            int totalObjects = Mathf.RoundToInt(rawObjects);
            int typeCount = Mathf.RoundToInt(rawTypes);

            // Breath/Easy Level check
            bool isBreathLevel = (levelId % config.breathLevelInterval == 0);
            if (isBreathLevel)
            {
                totalObjects = Mathf.RoundToInt(totalObjects * config.breathLevelMultiplier);
                typeCount = Mathf.Max(2, typeCount - config.breathLevelTypeReduction);
            }

            // Enforce constraints
            // 1. Total objects must be a multiple of 3
            totalObjects = Mathf.Max(6, (totalObjects / 3) * 3);

            // 2. Type count must be at least 2, and not exceed (totalObjects / 3)
            // (We need at least 3 tiles of each type)
            int maxPossibleTypes = totalObjects / 3;
            typeCount = Mathf.Clamp(typeCount, 2, Mathf.Min(12, maxPossibleTypes));

            // Stack density scales Y bounds and box size slightly based on object counts.
            // More objects -> slightly higher Y spawn range to let them stack nicely.
            float stackDensity = 1.0f + (totalObjects - 18) * 0.02f;

            return new Parameters
            {
                levelId = levelId,
                totalObjects = totalObjects,
                typeCount = typeCount,
                stackDensity = stackDensity
            };
        }
    }
}
