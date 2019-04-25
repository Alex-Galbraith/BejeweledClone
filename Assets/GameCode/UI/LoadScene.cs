using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper.UI {
    /// <summary>
    /// Script based scene loading with fade transition.
    /// </summary>
    public class LoadScene : MonoBehaviour {
        public int sceneID;
        public Image fade;
        public float transitionTime;
        public bool freezeTime = false;
        public Color from, to;
        public void ChangeScene() {
            if (freezeTime)
                Time.timeScale = 0;
            StartCoroutine(Transition());
        }
        /// <summary>
        /// Fades then loads scene.
        /// </summary>
        public IEnumerator Transition() {
            float cTime = 0;
            while (cTime < transitionTime) {
                cTime += Time.unscaledDeltaTime;
                fade.color = Color.Lerp(from, to, cTime / transitionTime);
                yield return null;
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);
            if (freezeTime)
                Time.timeScale = 1;
        }
    }
}
