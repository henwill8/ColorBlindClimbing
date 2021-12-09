using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorConversions
{
    // From https://bitbucket.org/hasullivan/color-space/src/master/ColorSpaceConversion/Conversion.cs
    public static Vector3 RGBToXYZ(Color color)
    {
        float[] xyz = new float[4];
        float[] rgb = new float[] { color.r, color.g, color.b };

        for (int i = 0; i < 3; i++)
        {
            if (rgb[i] > .04045f)
            {
                rgb[i] = (float)Math.Pow((rgb[i] + .055) / 1.055, 2.4);
            }
            else
            {
                rgb[i] = rgb[i] / 12.92f;
            }
        }

        xyz[0] = (rgb[0] * .412453f) + (rgb[1] * .357580f) + (rgb[2] * .180423f);
        xyz[1] = (rgb[0] * .212671f) + (rgb[1] * .715160f) + (rgb[2] * .072169f);
        xyz[2] = (rgb[0] * .019334f) + (rgb[1] * .119193f) + (rgb[2] * .950227f);

        return new Vector3(xyz[0], xyz[1], xyz[2]);
    }

    // Method from link was wrong, this is adjusted
    public static Vector3 XYZToLab(Vector3 color)
    {
        float[] lab = new float[3];
        float[] xyz = new float[3];
        float[] col = new float[] { color.x, color.y, color.z };

        xyz[0] = col[0] / 0.95047f;
        xyz[1] = col[1] / 1.0f;
        xyz[2] = col[2] / 1.08883f;

        for (int i = 0; i < 3; i++)
        {
            if (xyz[i] > .008856f)
            {
                xyz[i] = (float)Math.Pow(xyz[i], 1.0 / 3.0);
            }
            else
            {
                xyz[i] = ((xyz[i] * 903.3f) + 16.0f) / 116.0f;
            }
        }

        lab[0] = (116.0f * xyz[1]) - 16.0f;
        lab[1] = 500.0f * (xyz[0] - xyz[1]);
        lab[2] = 200.0f * (xyz[1] - xyz[2]);

        return new Vector3(lab[0], lab[1], lab[2]);
    }

    public static ColorUtils.LCH LabToLCH(Vector3 color)
    {
        ColorUtils.LCH lch = new ColorUtils.LCH();

        lch.l = color[0];
        lch.c = (float)Math.Sqrt((color[1] * color[1]) + (color[2] * color[2]));
        lch.h = (float)Math.Atan2(color[2], color[1]) * 180.0f/(float)System.Math.PI;
        if(lch.h < 0) lch.h += 360;

        return lch;
    }

    public static ColorUtils.LCH RGBToLCH(Color color)
    {
        return LabToLCH(XYZToLab(RGBToXYZ(color)));
    }
}
