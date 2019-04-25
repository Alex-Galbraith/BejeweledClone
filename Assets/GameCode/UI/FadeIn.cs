using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace TSwapper.UI {
    public class FadeIn : MonoBehaviour {
        public Image fade;
        public float transitionTime;
        public bool freezeTime = false;
        public bool playOnStart = true;
        public Color from, to;
        public void Start() {
            if (!playOnStart)
                return;
            if (freezeTime)
                Time.timeScale = 0;
            StartCoroutine(Transition());
        }

        public void Activate() {
            if (freezeTime)
                Time.timeScale = 0;
            StartCoroutine(Transition());
        }

        public IEnumerator Transition() {
            float cTime = 0;
            while (cTime < transitionTime) {
                cTime += Time.unscaledDeltaTime;
                fade.color = Color.Lerp(from, to, cTime / transitionTime);
                yield return null;
            }
            if (freezeTime)
                Time.timeScale = 1;
        }
    }
}
