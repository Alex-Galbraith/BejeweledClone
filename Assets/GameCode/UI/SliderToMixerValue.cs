using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TSwapper.UI {
    public class SliderToMixerValue : MonoBehaviour {
        public AudioMixer mixer;
        public string paramName;

        public Slider slider;

        public void SetFloat(float val) {
            mixer.SetFloat(paramName, val);
        }

        public void SetLogFloat(float val) {
            mixer.SetFloat(paramName, Mathf.Log(val));
        }

        private void Start() {
            mixer.GetFloat(paramName, out float v);
            slider.value = v;
        }
    }
}
