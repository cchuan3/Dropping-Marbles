using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarControl : MonoBehaviour
{
    public Slider s;
	private float currVal = 0f;
    public float CurrVal {
        get {
            return currVal;
        }
        set {
            currVal = value;
            s.value = currVal;
        }
    }
}