using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper.UI { 
    /// <summary>
    /// Score bar display with smoothly updating bare length effect.
    /// </summary>
    public class ScoreBar : MonoBehaviour
    {
        public RectTransform front;
        public RectTransform back;
        public IntReference scoreRef;
        public IntReference maxScoreRef;

        public float tweenTime = 0.5f;
        private int lastVal = 0;

        /// <summary>
        /// Score change callback
        /// </summary>
        private void OnChange(int before, int after) {
            StartCoroutine(TweenScore(tweenTime, lastVal, after));
            lastVal = after;
        }

        /// <summary>
        /// Smoothly update the score bar between two values.
        /// </summary>
        private IEnumerator TweenScore(float time, int before, int after) {
            float cTime = 0;
            while (cTime < time) {
                cTime += Time.unscaledDeltaTime;
                float frac = Mathf.Min(1, Mathf.Lerp(before, after, cTime / time) / (float)maxScoreRef.Value);
                front.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, frac * back.rect.width);
                yield return null;
            }

        }
        /// <summary>
        /// Register callbacks on creation.
        /// </summary>
        private void Awake() {
            if (scoreRef != null) {
                scoreRef.NotifyChange += OnChange;
            }
        }
        /// <summary>
        /// Deregister callback when destroyed
        /// </summary>
        private void OnDestroy() {
            scoreRef.NotifyChange -= OnChange;
        }
    }
}