using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper {
    [ExecuteInEditMode, RequireComponent(typeof(Text))]
    public class ScoreText : MonoBehaviour {
        public IntReference scoreRef, maxScoreRef;
        public Text toUpdate;
        public string divider = " / ";

        public float tweenTime = 0.5f;
        private int lastVal;

        private void Awake() {
            if (toUpdate == null)
                toUpdate = GetComponent<Text>();
            if (scoreRef != null) {
                scoreRef.NotifyChange += OnScoreChange;
            }
        }

        private void OnDestroy() {
            scoreRef.NotifyChange -= OnScoreChange;
        }

        private void OnScoreChange(int before, int after) {
            StartCoroutine(TweenScore(tweenTime, lastVal, after));
            lastVal = after;
        }

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