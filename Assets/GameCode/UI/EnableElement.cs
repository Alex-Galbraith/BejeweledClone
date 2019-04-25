using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper.UI {
    /// <summary>
    /// Enables or disables and element from script.
    /// </summary>
    public class EnableElement : MonoBehaviour {
        public GameObject toEnable;
        public void Enable() {
            toEnable.SetActive(true);
        }
        public void Disable() {
            toEnable.SetActive(false);
        }
    }
}
