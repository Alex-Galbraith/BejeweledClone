using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBar : MonoBehaviour
{
    public RectTransform front;
    public RectTransform back;
    public IntReference scoreRef;
    public IntReference maxScoreRef;

    private void OnChange(int before, int after) {
        float frac = Mathf.Min(1,((float)after) / (float)maxScoreRef.Value);
        front.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, frac * back.rect.width);
    }

    private void OnChangeMax(int before, int after) {
    }

    private void Awake() {
        if (scoreRef != null) {
            scoreRef.NotifyChange += OnChange;
        }
        if (maxScoreRef != null) {
            maxScoreRef.NotifyChange += OnChangeMax;
        }
    }

    private void OnDestroy() {
        scoreRef.NotifyChange -= OnChange;
        maxScoreRef.NotifyChange -= OnChangeMax;
    }
}
