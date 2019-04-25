using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TSwapper.UI {
    /// <summary>
    /// UI hook for updating a mixer value to and from a <see cref="Slider"/>.
    /// </summary>
    public class SliderToMixerValue : MonoBehaviour {
        public AudioMixer mixer;
        public string paramName;

        public Slider slider;

        /// <summary>
        /// Set the float value of <see cref="paramName"/>.
        /// </summary>
        /// <param name="val"></param>
        public void SetFloat(float val) {
            mixer.SetFloat(paramName, val);
        }

        /// <summary>
        /// Load the float value of <see cref="paramName"/> to our <see cref="Slider"/>.
        /// </summary>
        public void LoadFloat() {
            mixer.GetFloat(paramName, out float v);
            slider.value = v;
        }
        
        private void Start() {
            LoadFloat();
        }
    }
}
