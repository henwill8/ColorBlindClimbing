using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Utils
{
    static public bool IsInCircularRange(int value, int min, int max) {
        if(max >= min && (value >= min && value <= max)) {
            return true;
        } else if(min > max && (value >= min || value <= max)) {
            return true;
        }

        return false;
    }

    static public float EaseInOutExpo(float p) {
        if (p == 0.0 || p == 1.0) {
            return p;
        }

        if (p < 0.5f) {
            return 0.5f * Mathf.Pow(2, (20 * p) - 10);
        } else {
            return (-0.5f * Mathf.Pow(2, (-20 * p) + 10)) + 1;
        }
    }
}
