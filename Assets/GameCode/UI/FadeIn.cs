using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace TSwapper.UI {
    /// <summary>
    /// Fades an image in or out.
    /// </summary>
    public class FadeIn : MonoBehaviour {
        public Image fade;
        public float transitionTime;
        public bool playOnStart = true;
        public Color from, to;
        public void Start() {
            if (!playOnStart)
                return;
            StartCoroutine(Transition());
        }

        public void Activate() {
            StartCoroutine(Transition());
        }

        public IEnumerator Transition() {
            float cTime = 0;
            while (cTime < transitionTime) {
                cTime += Time.unscaledDeltaTime;
                fade.color = Color.Lerp(from, to, cTime / transitionTime);
                yield return null;
            }
        }
    }
}
