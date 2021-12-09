using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// https://github.com/goagostudio/unity-colorutils/blob/master/ColorUtils.cs
public class ColorHSL
{
    private float _h;
    private float _s;
    private float _l;

    public ColorHSL(float h = 0, float s = 0, float l = 0)
    {
        _h = h;
        _s = s;
        _l = l;
    }

    public float h
    {
        get { return _h; }
        set { _h = value; }
    }

    public float s
    {
        get { return _s; }
        set { _s = value; }
    }

    public float l {
        get { return _l; }
        set { _l = value; }
    }
}

public class ColorUtils
{
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
