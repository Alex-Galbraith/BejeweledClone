using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper {
    [ExecuteInEditMode, RequireComponent(typeof(Text))]
    public class ScoreText : MonoBehaviour {
        public IntReference scoreRef, maxScoreRef;
        public Text toUpdate;
        public string divider = " / ";

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
            StartCoroutine(TweenScore(0.5f, before, after));
        }

        private IEnumerator TweenScore(float time, int before, int after) {
            float cTime = 0;
            while (cTime < time) {
                cTime += Time.deltaTime;
                toUpdate.text = Mathf.CeilToInt(Mathf.Lerp(before,after,cTime/time)) + divider + maxScoreRef.Value;
                yield return null;
            }
            
        }
    }
}