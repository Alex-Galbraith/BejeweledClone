using UnityEngine;
using System.Collections;
namespace TSwapper.UI { 
    /// <summary>
    /// UI hook for quitting the game
    /// </summary>
    public class Quit : MonoBehaviour {
        public void DoQuit() {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}