using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pause Asset")]
public class SetPaused : ScriptableObject
{
    private float originalScale = 1;
    public void Pause() {
        if (Time.timeScale == 0)
            return;
        originalScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void UnPause() {
        Time.timeScale = originalScale;
    }
}
