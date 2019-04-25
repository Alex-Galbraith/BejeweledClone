using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper.UI {
    /// <summary>
    /// Smoothly updates score text by counting up to the goal value.
    /// </summary>
    [ExecuteInEditMode, RequireComponent(typeof(Text))]
    public class ScoreText : MonoBehaviour {
        public IntReference scoreRef, maxScoreRef;
        public Text toUpdate;
        public string divider = " / ";

        public float tweenTime = 0.5f;
        private int lastVal;

        /// <summary>
        /// Find our text component if it hasnt been set and register listeners.
        /// </summary>
        private void Awake() {
            if (toUpdate == null)
                toUpdate = GetComponent<Text>();
            if (scoreRef != null) {
                scoreRef.NotifyChange += OnScoreChange;
            }
        }

        /// <summary>
        /// Unregister from listeners when we are destroyed
        /// </summary>
        private void OnDestroy() {
            scoreRef.NotifyChange -= OnScoreChange;
        }

        /// <summary>
        /// Score change callback
        /// </summary>
        private void OnScoreChange(int before, int after) {
            StartCoroutine(TweenScore(tweenTime, lastVal, after));
            lastVal = after;
        }

        /// <summary>
        /// Smoothly count up to the correct score over time.
        /// </summary>
        private IEnumerator TweenScore(float time, int before, int after) {
            float cTime = 0;
            while (cTime < time) {
                cTime += Time.unscaledDeltaTime;
                toUpdate.text = Mathf.CeilToInt(Mathf.Lerp(before,after,cTime/time)) + divider + maxScoreRef.Value;
                yield return null;
            }
            
        }
    }
}