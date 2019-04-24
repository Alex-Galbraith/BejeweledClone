using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableElement : MonoBehaviour
{
    public GameObject toEnable;
    public void Enable() {
        toEnable.SetActive(true);
    }
    public void Disable() {
        toEnable.SetActive(false);
    }
}
