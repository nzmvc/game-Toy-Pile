using UnityEngine;

namespace Chassis.Juice
{
    /// <summary>
    /// The intensity level of physical haptic feedback.
    /// </summary>
    public enum HapticStrength
    {
        None,
        Light,
        Medium,
        Heavy
    }

    /// <summary>
    /// Configuration asset grouping visual, audio, and physical haptic parameters
    /// for triggering juicy feedback events (e.g. click, match, win, etc.).
    /// </summary>
    [CreateAssetMenu(fileName = "NewJuicePreset", menuName = "Chassis/Juice Preset")]
    public class JuicePreset : ScriptableObject
    {
        [Header("Visual Effects (Particles)")]
        [Tooltip("The particle system prefab to spawn.")]
        public GameObject particlePrefab;

        [Tooltip("How long in seconds before the particle is recycled back to pool.")]
        public float particleDuration = 2.0f;

        [Header("Audio Effects (SFX)")]
        [Tooltip("Audio clip to play.")]
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume = 1.0f;

        [Header("Physical Effects (Haptics)")]
        [Tooltip("The intensity of the haptic feedback.")]
        public HapticStrength hapticStrength = HapticStrength.None;
    }
}
