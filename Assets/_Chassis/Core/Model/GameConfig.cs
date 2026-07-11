using UnityEngine;

namespace Chassis.Core
{
    /// <summary>
    /// ScriptableObject defining all runtime gameplay balancing and tuning parameters.
    /// Exposes configuration variables without hardcoding them in the scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Chassis/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Tile Match Mechanics")]
        [Tooltip("Fly/tween duration of a tile from the physics pile to its bar slot.")]
        public float tileFlyDuration = 0.35f;

        [Tooltip("Maximum slots in the match bar (usually 7).")]
        public int maxBarSlots = 7;

        [Tooltip("The number of slots filled that triggers the Near Miss warning.")]
        public int nearMissThreshold = 6;

        [Tooltip("Points awarded per 3-tile match.")]
        public int matchScore = 100;

        [Header("Shockwave Settings")]
        [Tooltip("The explosion/impulse force applied to neighboring physics objects when a match pops.")]
        public float shockwaveForce = 15f;

        [Tooltip("The radius of the shockwave blast.")]
        public float shockwaveRadius = 5f;

        [Tooltip("The upwards modifier for the explosion force to lift objects slightly.")]
        public float shockwaveUpwardsModifier = 0.5f;

        [Header("Physics Spawning Bounds")]
        [Tooltip("The range on the X-axis within which tiles can spawn randomly.")]
        public Vector2 spawnRangeX = new Vector2(-2f, 2f);

        [Tooltip("The height range on the Y-axis from which tiles will fall.")]
        public Vector2 spawnRangeY = new Vector2(4f, 8f);

        [Tooltip("The range on the Z-axis within which tiles can spawn randomly.")]
        public Vector2 spawnRangeZ = new Vector2(-2f, 2f);

        [Header("Difficulty Curves")]
        [Tooltip("Curve mapping level number (X) to total number of spawned objects (Y). Should be a multiple of 3.")]
        public AnimationCurve totalObjectsCurve = AnimationCurve.Linear(1, 18, 100, 63);

        [Tooltip("Curve mapping level number (X) to unique type counts (Y).")]
        public AnimationCurve typeCountCurve = AnimationCurve.Linear(1, 3, 100, 10);

        [Tooltip("Interval at which breath/easy levels occur (e.g. every 5 levels).")]
        public int breathLevelInterval = 5;

        [Tooltip("Multiplier applied to total objects on breath levels to drop difficulty (e.g. 0.6f).")]
        public float breathLevelMultiplier = 0.6f;

        [Tooltip("Flat amount of type count reduction on breath levels (e.g. 1).")]
        public int breathLevelTypeReduction = 1;
    }
}
