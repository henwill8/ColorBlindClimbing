using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorConversions
{
    // https://github.com/goagostudio/unity-colorutils/blob/master/ColorUtils.cs
    public static ColorHSL RGBtoHSL(Color color)
    {
        float max = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
        float min = Mathf.Min(Mathf.Min(color.r, color.g), color.b);

        float h;
        float s;
        float l = (max + min) / 2f;

        if (max == min)
        {
            h = s = 0;
        }
        else
        {
            float d = max - min;
            s = l > .5f ? d / (2f - max - min) : d / (max + min);

            if (max == color.r)
            {
                h = (color.g - color.b) / d + (color.g < color.b ? 6f : 0);
            }
            else if (max == color.g)
            {
                h = (color.b - color.r) / d + 2f;
            }
            else
            {
                h = (color.r - color.g) / d + 4f;
            }
            h /= 6;
        }

        return new ColorHSL(h, s, l);
    }

    private static float HUEtoRGB(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t*6 < 1f) return p + (q - p) * 6f * t;
        if (t*2 < 1f) return q;
        if (t*3 < 2f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }

    public static Color HSLtoRGB(ColorHSL color)// This is wrong
    {

        float r;
        float g;
        float b;

        if (color.s == 0)
        {
            r = g = b = color.l;
        }
        else
        {
            float q = color.l < .5f ? color.l * (1f + color.s) : color.l + color.s - color.l * color.s;
            float p = 2f * color.l - q;

            r = HUEtoRGB(p, q, color.h + 1f / 3f);
            g = HUEtoRGB(p, q, color.h);
            b = HUEtoRGB(p, q, color.h - 1f / 3f);
        }

        return new Color(r, g, b);
    }
}
