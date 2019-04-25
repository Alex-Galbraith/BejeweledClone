using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper.UI { 
    [ExecuteInEditMode, RequireComponent(typeof(Text))]
    public class UpdateTextWithInt : MonoBehaviour
    {
        public IntReference reference;
        public Text toUpdate;
        public string prefix = "";
        public string suffix = "";

        private void Awake() {
            if (toUpdate == null)
                toUpdate = GetComponent<Text>();
            if (reference != null) {
                reference.NotifyChange += OnChange;
            }
        }

        private void OnDestroy() {
            reference.NotifyChange -= OnChange;
        }

        private void OnChange(int before, int after) {
            toUpdate.text = prefix + after + suffix;
        }
    }
}