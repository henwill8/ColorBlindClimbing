using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ColorBoundsHandler : MonoBehaviour
{
    static public float hueSensitivity = 2;
    
    static public Vector2 saturationRemoval = new Vector2(0.01f, 0.004f);
    static public Vector2 valueRemoval = new Vector2(0.004f, 0.005f);

    static public int[] hueOccurrencesCounted;

    // Start is called before the first frame update
    void Start()
    {
        Shader.SetGlobalFloat("_HueSensitivity", hueSensitivity);

        Shader.SetGlobalFloat("_SaturationMinRemoval", saturationRemoval.x);
        Shader.SetGlobalFloat("_SaturationMaxRemoval", saturationRemoval.y);

        Shader.SetGlobalFloat("_ValueMinRemoval", valueRemoval.x);
        Shader.SetGlobalFloat("_ValueMaxRemoval", valueRemoval.y);
    }

    // Update is called once per frame
    void Update()
    {
        hueSensitivity = Shader.GetGlobalFloat("_HueSensitivity");

        saturationRemoval.x = Shader.GetGlobalFloat("_SaturationMinRemoval");
        saturationRemoval.y = Shader.GetGlobalFloat("_SaturationMaxRemoval");

        valueRemoval.x = Shader.GetGlobalFloat("_ValueMinRemoval");
        valueRemoval.y = Shader.GetGlobalFloat("_ValueMaxRemoval");
    }
    
    static private Tuple<int, int> GetBoundsFromHue(int hue)
    {
        int[] colorBounds = {5, 35, 60, 150, 250, 335, 355};

        for(int i = 0; i < colorBounds.Length; i++) {
            int firstBound = colorBounds[i];
            int secondBound = colorBounds[ArrayManager.KeepInCircularRange(0, colorBounds.Length-1, i+1)];

            if((firstBound <= secondBound && (hue >= firstBound && hue <= secondBound)) || (firstBound >= secondBound && (hue >= firstBound || hue <= secondBound))) {
                return new Tuple<int, int>(firstBound, secondBound);
            }
        }

        return new Tuple<int, int>(0, 0);
    }

    static public float GetColorValueFromInt(Color pixel, int i) {
        if(i == 0) {
            return pixel.r;
        } else if(i == 1) {
            return pixel.g;
        } else {
            return pixel.b;
        }
    }

    static public bool ColorIsGrayScale(Color color, float sensitivity) {
        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 3; j++) {
                if(i == j) break;
                if(System.Math.Abs(GetColorValueFromInt(color, i) - GetColorValueFromInt(color, j)) > sensitivity/255.0f) return false;
            }
        }
        return true;
    }

    static public void GetHighlightColor(WebCamTexture camera)
    {
        Debug.Log("Getting pixels");
        Color[] pixels = camera.GetPixels(camera.width / 4, camera.height / 4, camera.width / 2, camera.height / 2);

        int totalPixels = pixels.Length;
        int coloredPixels = 0;

        int[] hues = new int[pixels.Length];
        int[] saturations = new int[pixels.Length];
        int[] values = new int[pixels.Length];

        float grayScaleSensitivity = Shader.GetGlobalFloat("_GrayScaleSensitivity");
        Debug.Log(grayScaleSensitivity);

        foreach (var pixel in pixels)
        {
            if(ColorIsGrayScale(pixel, grayScaleSensitivity)) break;

            float h, s, v;
            Color.RGBToHSV(pixel, out h, out s, out v);

            hues[coloredPixels] = (int)(h * 360);
            saturations[coloredPixels] = (int)(s * 100);
            values[coloredPixels] = (int)(v * 100);
            coloredPixels++;
        }

        hueOccurrencesCounted = ArrayManager.IntValueCounter(hues, 360);
        Tuple<int, int> hueBounds = ArrayManager.GetBoundsOfHighestDensityValues(hueOccurrencesCounted, hueSensitivity);
        // Tuple<int, int> hueBounds = GetBoundsFromHue(ArrayManager.GetHighestIndex(hueOccurrencesCounted));

        Shader.SetGlobalFloat("_MinimumHue", hueBounds.Item1);
        Shader.SetGlobalFloat("_MaximumHue", hueBounds.Item2);

        int[] satInBoundValues = ArrayManager.RemoveOutOfBoundValues(saturations, hues, hueBounds.Item1, hueBounds.Item2);
        Tuple<int, int> satBounds = ArrayManager.GetBoundsFromPercentRange(satInBoundValues, saturationRemoval.x, 1 - saturationRemoval.y);

        Shader.SetGlobalFloat("_MinimumSaturation", satBounds.Item1);
        Shader.SetGlobalFloat("_MaximumSaturation", satBounds.Item2);

        int[] valInBoundValues = ArrayManager.RemoveOutOfBoundValues(values, hues, hueBounds.Item1, hueBounds.Item2);
        Tuple<int, int> valBounds = ArrayManager.GetBoundsFromPercentRange(valInBoundValues, valueRemoval.x, 1 - valueRemoval.y);

        Shader.SetGlobalFloat("_MinimumValue", valBounds.Item1);
        Shader.SetGlobalFloat("_MaximumValue", valBounds.Item2);

        // Debug.Log("Done!");
    }
}
