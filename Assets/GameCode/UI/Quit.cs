using UnityEngine;
using System.Collections;
namespace TSwapper.UI { 
    public class Quit : MonoBehaviour {
        public void DoQuit() {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}