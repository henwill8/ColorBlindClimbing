using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

public class ColorBoundsHandler : MonoBehaviour
{
    static public float hueSensitivity = 2;
    
    static public Vector2 saturationRemoval = new Vector2(0.01f, 0.004f);
    static public Vector2 valueRemoval = new Vector2(0.004f, 0.005f);

    static public int[] hueOccurrencesCounted;

    static public int[] savedHuesArray;
    static public string savedHuesArrayFileName = "SavedHues.bin";

    // Start is called before the first frame update
    void Start()
    {
        Shader.SetGlobalFloat("_HueSensitivity", hueSensitivity);

        Shader.SetGlobalFloat("_SaturationMinRemoval", saturationRemoval.x);
        Shader.SetGlobalFloat("_SaturationMaxRemoval", saturationRemoval.y);

        Shader.SetGlobalFloat("_ValueMinRemoval", valueRemoval.x);
        Shader.SetGlobalFloat("_ValueMaxRemoval", valueRemoval.y);

        if(File.Exists(Path.Combine(Application.persistentDataPath, savedHuesArrayFileName))) {
            savedHuesArray = FileUtils.ReadFileToIntArray(savedHuesArrayFileName);
        } else {
            savedHuesArray = new int[360];
        }
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
            int secondBound = colorBounds[Utils.KeepInCircularRange(0, colorBounds.Length-1, i+1)];

            if(Utils.IsInCircularRange(hue, firstBound, secondBound)) {
                return new Tuple<int, int>(firstBound, secondBound);
            }
        }

        return new Tuple<int, int>(0, 0);
    }

    static public Tuple<int, int> GetBoundsFromPercentRange(int[] input, float lowerBoundPercent, float upperBoundPercent)
    {
        Array.Sort(input);

        int lowerIndex = (int)((input.Length-1) * lowerBoundPercent);
        int upperIndex = (int)((input.Length-1) * upperBoundPercent);

        int min = input[lowerIndex];
        int max = input[upperIndex];

        // Debug.Log("Median: "+input[(int)((input.Length-1) * 0.5f)]+", Min: "+min+", Lower Index: "+lowerIndex+", Max: "+max+", Upper Index: "+upperIndex+", Inputs Length: "+input.Length);

        return new Tuple<int, int>(min, max);
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
        int lineLength = camera.width;
        int lines = camera.height;

        Color[] pixels = camera.GetPixels(0, 0, lineLength, lines);

        Vector2 centerPosition = new Vector2(camera.width / 2, camera.height / 2);
        float maxDistance = Vector2.Distance(Vector2.zero, centerPosition) / 2;

        int coloredPixels = 0;

        int[] hues = new int[pixels.Length];
        int[] saturations = new int[pixels.Length];
        int[] values = new int[pixels.Length];

        int[] amplification = new int[360];

        for(int i = 0; i < pixels.Length; i++) {
            Vector2 pixelPosition = new Vector2(i % lineLength, i / lineLength);
            // Debug.Log(pixelPosition);

            float quadraticOne = (-1.0f/25.0f)*(float)Math.Pow(i, 2)+20;
            float quadraticTwo = (-1.0f/25.0f)*(float)Math.Pow(i-359, 2)+20;
            float grayScaleFiltering = Math.Max(5, Math.Max(quadraticOne, quadraticTwo));
            if(ColorIsGrayScale(pixels[i], grayScaleFiltering)) continue;

            float h, s, v;
            Color.RGBToHSV(pixels[i], out h, out s, out v);

            amplification[(int)(h * 360)] += (int)((1.0f - Math.Min(Vector2.Distance(pixelPosition, centerPosition) / maxDistance, 1)) * 4);//Lower for actual use?

            hues[coloredPixels] = (int)(h * 360);
            saturations[coloredPixels] = (int)(s * 100);
            values[coloredPixels] = (int)(v * 100);

            coloredPixels++;
        }

        Array.Resize(ref hues, coloredPixels);
        Array.Resize(ref saturations, coloredPixels);
        Array.Resize(ref values, coloredPixels);

        hueOccurrencesCounted = ArrayManager.SmoothIntArray(ArrayManager.AddArrays(amplification, ArrayManager.IntValueCounter(hues, 360)), 5);
        // int maxIndex = ArrayManager.FindTopOfNearestHill(hueOccurrencesCounted, ArrayManager.GetHighestAverageIndex(ArrayManager.AddArrays(ArrayManager.SmoothIntArray(amplification, 5), hueOccurrencesCounted)));
        int maxIndex = ArrayManager.GetHighestAverageIndex(hueOccurrencesCounted);
        int[] normalizedArray = ArrayManager.AddArrays(ArrayManager.NormalizeArray(hueOccurrencesCounted, 1000), savedHuesArray);
        Tuple<int, int> hueBounds = ArrayManager.GetBoundsOfHighestDensityValues(normalizedArray, hueSensitivity, maxIndex);

        int[] processedArray = ArrayManager.ExtremifyArray(ArrayManager.NormalizeArray(ArrayManager.RemoveValuesOutOfIndexRange(hueOccurrencesCounted, hueBounds.Item1, hueBounds.Item2), 1000), 0.5f);
        savedHuesArray = ArrayManager.MergeArrays(savedHuesArray, processedArray);
        FileUtils.IntArrayToFile(savedHuesArray, savedHuesArrayFileName);
        // Tuple<int, int> hueBounds = GetBoundsFromHue(ArrayManager.GetHighestIndex(hueOccurrencesCounted));

        Shader.SetGlobalFloat("_GrayScaleSensitivity", Math.Max(5, Math.Max((-1.0f/300.0f)*(float)Math.Pow(maxIndex-20, 2)+35, (-1.0f/300.0f)*(float)Math.Pow(maxIndex-415, 2)+35)));

        Shader.SetGlobalFloat("_MinimumHue", hueBounds.Item1);
        Shader.SetGlobalFloat("_MaximumHue", hueBounds.Item2);

        int[] satInBoundValues = ArrayManager.RemoveOutOfBoundValues(saturations, hues, hueBounds.Item1, hueBounds.Item2);
        Tuple<int, int> satBounds = GetBoundsFromPercentRange(satInBoundValues, saturationRemoval.x, 1 - saturationRemoval.y);

        Shader.SetGlobalFloat("_MinimumSaturation", satBounds.Item1);
        Shader.SetGlobalFloat("_MaximumSaturation", satBounds.Item2);

        int[] valInBoundValues = ArrayManager.RemoveOutOfBoundValues(values, hues, hueBounds.Item1, hueBounds.Item2);
        Tuple<int, int> valBounds = GetBoundsFromPercentRange(valInBoundValues, valueRemoval.x, 1 - valueRemoval.y);

        Shader.SetGlobalFloat("_MinimumValue", valBounds.Item1);
        Shader.SetGlobalFloat("_MaximumValue", valBounds.Item2);

        // Debug.Log("Done!");
    }

    static public void DeleteSavedHues()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, savedHuesArrayFileName));
        Array.Clear(savedHuesArray, 0, 360);
    }
}
