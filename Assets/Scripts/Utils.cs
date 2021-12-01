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

    static public int KeepInCircularRange(int value, int min, int max)
    {
        int safety = 0;
        while((value > max || value < min) && safety < 100) {
            if(value > max) value -= max - min + 1;
            if(value < min) value += max - min + 1;
            
            safety++;
        }
        if(safety > 99) Debug.Log("INFINITE LOOP STOPPED AT KEEPINCICULARRANGE()");
        return value;
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
