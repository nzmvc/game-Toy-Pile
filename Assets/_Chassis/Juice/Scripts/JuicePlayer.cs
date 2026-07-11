using UnityEngine;

namespace Chassis.Juice
{
    /// <summary>
    /// Mock manager managing particle plays, SFX triggers, and haptic engine feedback.
    /// Exposes lightweight visual-feedback triggers.
    /// </summary>
    public class JuicePlayer : MonoBehaviour
    {
        public static JuicePlayer Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySfx(string clipName)
        {
            Debug.Log($"[JuicePlayer] Playing Audio SFX: <b>{clipName}</b>");
        }

        public void PlayVibration(string strength)
        {
            Debug.Log($"[JuicePlayer] Triggering Haptic: <b>{strength} vibration</b>");
        }

        public void PlayParticle(string presetName, Vector3 position)
        {
            Debug.Log($"[JuicePlayer] Spawning Particle preset <b>{presetName}</b> at position: {position}");
        }
    }
}
