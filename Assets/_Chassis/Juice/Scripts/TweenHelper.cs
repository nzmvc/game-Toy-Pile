using System.Collections;
using UnityEngine;

namespace Chassis.Juice
{
    /// <summary>
    /// Lightweight tween animation utilities implemented using Unity Coroutines.
    /// Eliminates external library dependencies like DOTween for basic casual juice.
    /// </summary>
    public static class TweenHelper
    {
        /// <summary>
        /// Animates the scale of a transform to bounce/punch out and return to normal.
        /// Useful for click/tap squashes.
        /// </summary>
        public static void PunchScale(MonoBehaviour runner, Transform target, Vector3 amount, float duration)
        {
            if (target == null || runner == null || !runner.gameObject.activeInHierarchy) return;
            runner.StartCoroutine(PunchScaleCoroutine(target, amount, duration));
        }

        private static IEnumerator PunchScaleCoroutine(Transform target, Vector3 amount, float duration)
        {
            Vector3 originalScale = target.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (target == null) yield break;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Sine curve that peaks and fades out
                float factor = Mathf.Sin(t * Mathf.PI) * (1f - t);
                target.localScale = originalScale + amount * factor;

                yield return null;
            }

            if (target != null)
            {
                target.localScale = originalScale;
            }
        }

        /// <summary>
        /// Shakes the local position of a transform.
        /// Useful for camera sarsıntı and UI alerts.
        /// </summary>
        public static void ShakePosition(MonoBehaviour runner, Transform target, float strength, float duration)
        {
            if (target == null || runner == null || !runner.gameObject.activeInHierarchy) return;
            runner.StartCoroutine(ShakePositionCoroutine(target, strength, duration));
        }

        private static IEnumerator ShakePositionCoroutine(Transform target, float strength, float duration)
        {
            Vector3 originalPos = target.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (target == null) yield break;

                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Fade strength as time passes
                float currentStrength = strength * (1f - t);
                Vector3 offset = Random.insideUnitSphere * currentStrength;
                target.localPosition = originalPos + offset;

                yield return null;
            }

            if (target != null)
            {
                target.localPosition = originalPos;
            }
        }
    }
}
