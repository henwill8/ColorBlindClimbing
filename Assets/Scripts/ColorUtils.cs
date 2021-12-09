using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorUtils
{
    public struct HSV {
        public float h;
        public float s;
        public float v;
    }

    public struct LCH {
        public float l;
        public float c;
        public float h;
    }

    static public float GetColorValueFromInt(Color pixel, int i)
    {
        if(i == 0) {
            return pixel.r;
        } else if(i == 1) {
            return pixel.g;
        } else {
            return pixel.b;
        }
    }

    static public bool ColorIsGrayScale(Color color, float sensitivity)
    {
        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 3; j++) {
                if(i == j) break;
                if(System.Math.Abs(GetColorValueFromInt(color, i) - GetColorValueFromInt(color, j)) > sensitivity/255.0f) return false;
            }
        }
        return true;
    }
}
