using System;
using System.Collections;
using UnityEngine;

namespace Game.TileMatch
{
    /// <summary>
    /// Component attached to each matching item in the physics pile.
    /// Manages physical state, visual property initialization, and flying/tweening animations.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class TileObject : MonoBehaviour
    {
        public int TypeId { get; private set; }
        public bool IsCollected { get; private set; }

        private Rigidbody _rigidbody;
        private Collider _collider;
        private Renderer _renderer;
        private Coroutine _flyCoroutine;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// Initializes the tile's data, sets physics active, and applies custom color.
        /// </summary>
        public void Setup(int typeId, Color color)
        {
            TypeId = typeId;
            IsCollected = false;

            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            
            if (_collider != null)
            {
                _collider.enabled = true;
            }

            if (_renderer != null)
            {
                var block = new MaterialPropertyBlock();
                block.SetColor("_Color", color);
                _renderer.SetPropertyBlock(block);
            }
        }

        /// <summary>
        /// Disables physics and starts a coroutine to fly this tile into the bar slots.
        /// </summary>
        public void Collect(Transform targetSlot, float duration, Action onComplete)
        {
            if (IsCollected) return;
            IsCollected = true;

            // Turn off physics simulation and collision response
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = true;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            if (_collider != null)
            {
                _collider.enabled = false;
            }

            // Setup TrailRenderer dynamically for flight path feedback
            SetupTrail();

            if (_flyCoroutine != null)
            {
                StopCoroutine(_flyCoroutine);
            }

            _flyCoroutine = StartCoroutine(FlyToSlotCoroutine(targetSlot, duration, onComplete));
        }

        private void SetupTrail()
        {
            var trail = gameObject.GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = gameObject.AddComponent<TrailRenderer>();
            }

            trail.time = 0.25f;
            trail.startWidth = 0.2f;
            trail.endWidth = 0f;
            trail.autodestruct = false;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                trail.material = new Material(shader);
            }

            Color color = Color.white;
            if (_renderer != null)
            {
                var block = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(block);
                color = block.GetColor("_Color");
            }

            trail.startColor = color;
            trail.endColor = new Color(color.r, color.g, color.b, 0f);
            trail.enabled = true;
        }

        /// <summary>
        /// Smoothly moves the tile to a slot position (used when shifting in the bar).
        /// </summary>
        public void MoveToSlot(Transform targetSlot, float duration)
        {
            if (_flyCoroutine != null)
            {
                StopCoroutine(_flyCoroutine);
            }
            _flyCoroutine = StartCoroutine(FlyToSlotCoroutine(targetSlot, duration, null));
        }

        private IEnumerator FlyToSlotCoroutine(Transform targetSlot, float duration, Action onComplete)
        {
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Cubic ease-out fly animation
                float tEase = 1f - Mathf.Pow(1f - t, 3f);

                if (targetSlot != null)
                {
                    transform.position = Vector3.Lerp(startPosition, targetSlot.position, tEase);
                    transform.rotation = Quaternion.Slerp(startRotation, targetSlot.rotation, tEase);
                }
                else
                {
                    yield break;
                }

                yield return null;
            }

            if (targetSlot != null)
            {
                transform.position = targetSlot.position;
                transform.rotation = targetSlot.rotation;
            }

            // Disable trail renderer upon arrival
            var trail = GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.enabled = false;
            }

            _flyCoroutine = null;
            onComplete?.Invoke();
        }
    }
}
