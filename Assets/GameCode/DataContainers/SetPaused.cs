using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper {
    [CreateAssetMenu(menuName = "Pause Asset")]
    public class SetPaused : ScriptableObject {
        private float originalScale = 1;
        public void Pause() {
            IsPaused = true;
            if (Time.timeScale == 0) {
                return;
            }
            originalScale = Time.timeScale;
            Time.timeScale = 0;
        }

        public void UnPause() {
            Time.timeScale = originalScale;
            IsPaused = false;
        }

        public bool IsPaused {
            get;
            private set;
        }
    }
}
