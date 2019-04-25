using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSwapper.UI { 
    /// <summary>
    /// UI hook class for embedding a dynamic int within text. Uses an <see cref="IntReference"/> and listens for changes.
    /// </summary>
    [ExecuteInEditMode, RequireComponent(typeof(Text))]
    public class UpdateTextWithInt : MonoBehaviour
    {
        public IntReference reference;
        public Text toUpdate;
        public string prefix = "";
        public string suffix = "";

        /// <summary>
        /// Find our text component if it hasnt been set and register listeners.
        /// </summary>
        private void Awake() {
            if (toUpdate == null)
                toUpdate = GetComponent<Text>();
            if (reference != null) {
                reference.NotifyChange += OnChange;
            }
        }

        /// <summary>
        /// When destroyed, unsubscribe from events
        /// </summary>
        private void OnDestroy() {
            reference.NotifyChange -= OnChange;
        }

        /// <summary>
        /// Update callback
        /// </summary>
        private void OnChange(int before, int after) {
            toUpdate.text = prefix + after + suffix;
        }
    }
}