using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>This class allows us to create a scriptable object that allows us to drag and drop paused as a reference. </para>
/// <para>The scriptable object can also be used to pause/unpause the game by calling <see cref="Pause"/> and <see cref="UnPause"/></para>
/// <para>There is also a NotifyChange event that can be subscribed to which is called when the value is modified.</para>
/// This technique is explained here: https://www.youtube.com/watch?v=raQ3iHhE_Kk
/// </summary>
[CreateAssetMenu(menuName = "Pause Asset")]
public class SetPaused : ScriptableObject {

    public event OnChange NotifyChange;
    public delegate void OnChange(bool paused);

    private float originalScale = 1;
    public void Pause() {
        if (!IsPaused)
            NotifyChange?.Invoke(true);
        IsPaused = true;
        if (Time.timeScale == 0) {
            return;
        }
        originalScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void UnPause() {
        Time.timeScale = originalScale;
        if (IsPaused)
            NotifyChange?.Invoke(true);
        IsPaused = false;
    }

    public bool IsPaused {
        get;
        private set;
    }
}
