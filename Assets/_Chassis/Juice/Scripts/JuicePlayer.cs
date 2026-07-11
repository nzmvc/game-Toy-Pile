using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chassis.Core;

namespace Chassis.Juice
{
    /// <summary>
    /// Core visual-feedback skin controller overseeing particle pooling, audio pooling, haptic triggers,
    /// camera shake tweening, and pitch combo scaling.
    /// </summary>
    public class JuicePlayer : MonoBehaviour
    {
        public static JuicePlayer Instance { get; private set; }

        [Header("Default Presets")]
        [SerializeField] private JuicePreset clickPreset;
        [SerializeField] private JuicePreset matchPreset;
        [SerializeField] private JuicePreset nearMissPreset;
        [SerializeField] private JuicePreset winPreset;

        [Header("Audio Pool Settings")]
        [SerializeField] private int audioPoolSize = 12;

        private readonly Dictionary<GameObject, Queue<GameObject>> _particlePools = new Dictionary<GameObject, Queue<GameObject>>();
        private readonly List<AudioSource> _audioSources = new List<AudioSource>();

        private int _comboCount = 0;
        private float _lastMatchTime = 0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioPool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioPool()
        {
            for (int i = 0; i < audioPoolSize; i++)
            {
                GameObject go = new GameObject($"PooledAudioSource_{i}");
                go.transform.SetParent(transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _audioSources.Add(source);
            }
        }

        public void PlayClick(Vector3 position)
        {
            if (clickPreset != null)
            {
                PlayPreset(clickPreset, position, false);
            }
        }

        public void PlayMatch(Vector3 position)
        {
            if (matchPreset != null)
            {
                PlayPreset(matchPreset, position, true);
            }
        }

        public void PlayWin(Vector3 position)
        {
            if (winPreset != null)
            {
                PlayPreset(winPreset, position, false);
                _comboCount = 0; // reset combo on win
            }
        }

        public void PlayNearMiss(Vector3 position)
        {
            if (nearMissPreset != null)
            {
                PlayPreset(nearMissPreset, position, false);
            }
        }

        public void ShakeCamera(float strength, float duration)
        {
            if (Camera.main != null)
            {
                TweenHelper.ShakePosition(this, Camera.main.transform, strength, duration);
            }
        }

        private void PlayPreset(JuicePreset preset, Vector3 position, bool isMatch)
        {
            if (preset == null) return;

            // 1. Play Particles (Object Pooled)
            if (preset.particlePrefab != null)
            {
                SpawnPooledParticle(preset.particlePrefab, position, preset.particleDuration);
            }

            // 2. Play Audio (Audio Source Pooled with Pitch)
            if (preset.audioClip != null)
            {
                PlayAudio(preset.audioClip, preset.volume, isMatch);
            }

            // 3. Play Haptics
            if (preset.hapticStrength != HapticStrength.None)
            {
                TriggerHaptic(preset.hapticStrength);
            }
        }

        private void SpawnPooledParticle(GameObject prefab, Vector3 position, float duration)
        {
            if (!_particlePools.TryGetValue(prefab, out Queue<GameObject> pool))
            {
                pool = new Queue<GameObject>();
                _particlePools.Add(prefab, pool);
            }

            GameObject instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                if (instance != null)
                {
                    instance.transform.position = position;
                    instance.SetActive(true);
                }
                else
                {
                    instance = Instantiate(prefab, position, Quaternion.identity, transform);
                }
            }
            else
            {
                instance = Instantiate(prefab, position, Quaternion.identity, transform);
            }

            // Start coroutine to return to pool
            StartCoroutine(ReturnToPoolCoroutine(prefab, instance, duration));
        }

        private IEnumerator ReturnToPoolCoroutine(GameObject prefab, GameObject instance, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (instance != null)
            {
                instance.SetActive(false);
                if (_particlePools.TryGetValue(prefab, out Queue<GameObject> pool))
                {
                    pool.Enqueue(instance);
                }
            }
        }

        private void PlayAudio(AudioClip clip, float volume, bool isMatch)
        {
            AudioSource source = GetAvailableAudioSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = volume;

            float pitch = 1.0f;
            if (isMatch)
            {
                var config = ServiceLocator.Get<GameConfig>();
                float window = config != null ? config.comboWindow : 2.0f;
                float step = config != null ? config.comboPitchStep : 0.08f;
                float maxPitch = config != null ? config.comboMaxPitch : 1.5f;

                if (Time.time - _lastMatchTime < window)
                {
                    _comboCount++;
                }
                else
                {
                    _comboCount = 0;
                }

                _lastMatchTime = Time.time;
                pitch = 1.0f + (_comboCount * step);
                pitch = Mathf.Min(pitch, maxPitch);
            }
            else
            {
                // Simple pitch random variance (±5%)
                pitch = Random.Range(0.95f, 1.05f);
            }

            source.pitch = pitch;
            source.Play();
        }

        private AudioSource GetAvailableAudioSource()
        {
            for (int i = 0; i < _audioSources.Count; i++)
            {
                if (!_audioSources[i].isPlaying)
                {
                    return _audioSources[i];
                }
            }
            // If all are busy, steal the oldest one
            return _audioSources[0];
        }

        private void TriggerHaptic(HapticStrength strength)
        {
            bool hapticsEnabled = PlayerPrefs.GetInt("HapticsEnabled", 1) == 1;
            if (!hapticsEnabled) return;

            Debug.Log($"[JuicePlayer] Haptic Feedback Triggered: <b>{strength}</b>");

#if UNITY_ANDROID || UNITY_IOS
            if (strength == HapticStrength.Heavy)
            {
                Handheld.Vibrate(); // simple fallback for mobile heavy rumble
            }
#endif
        }
    }
}
