using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

public class ColorBoundsHandler : MonoBehaviour
{
    static public int[] smoothedHues;
    static public int[] smoothedSaturations;
    static public int[] smoothedValues;

    static public int[] savedHuesArray;
    static public string savedHuesArrayFileName = "SavedHues.bin";

    // Start is called before the first frame update
    void Start()
    {
        if(File.Exists(Path.Combine(Application.persistentDataPath, savedHuesArrayFileName))) {
            savedHuesArray = FileUtils.ReadFileToIntArray(savedHuesArrayFileName);
        } else {
            savedHuesArray = new int[360];
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    static public void GetHighlightColor(WebCamTexture camera)
    {
        Debug.Log("GETTING HOLD COLOR BOUNDS");
        int lineLength = camera.width;
        int lines = camera.height;

        Color[] pixels = camera.GetPixels(0, 0, lineLength, lines);

        Vector2 centerPosition = new Vector2(camera.width / 2, camera.height / 2);
        float maxDistance = Vector2.Distance(Vector2.zero, centerPosition) / 2;

        ColorUtils.HSV[] hsvPixels = new ColorUtils.HSV[pixels.Length];
        int[] weightedHues = new int[360];
        int[] saturations = new int[101];
        int[] values = new int[101];

        for(int i = 0; i < pixels.Length; i++) {
            Vector2 pixelPosition = new Vector2(i % lineLength, i / lineLength);

            ColorUtils.HSV hsv;
            Color.RGBToHSV(pixels[i], out hsv.h, out hsv.s, out hsv.v);
            hsvPixels[i] = hsv;

            int weightAmount = (int)((1.0f - Math.Min(Vector2.Distance(pixelPosition, centerPosition) / maxDistance, 1)) * 10);

            float quadraticOne = (-1.0f/25.0f)*(float)Math.Pow(hsv.h * 360, 2)+25;
            float quadraticTwo = (-1.0f/25.0f)*(float)Math.Pow((hsv.h * 360)-359, 2)+25;
            float grayScaleFiltering = Math.Max(10, Math.Max(quadraticOne, quadraticTwo));

            if(!ColorUtils.ColorIsGrayScale(pixels[i], grayScaleFiltering)) weightedHues[(int)(hsv.h * 360)] += weightAmount;
        }
        
        // Get Hue Bounds
        smoothedHues = ArrayManager.SmoothIntArray(weightedHues, 10);
        int maxIndex = ArrayManager.GetHighestAverageIndex(smoothedHues);
        int[] normalizedArray = ArrayManager.NormalizeArray(smoothedHues, 1000);
        Tuple<int, int> hueBounds = ArrayManager.GetBoundsOfHighestDensityValues(ArrayManager.MergeArrays(normalizedArray, savedHuesArray, 1, 1, 10), 1.4f, 15, 10, true, 2, true, maxIndex);

        int[] processedArray = ArrayManager.ExtremifyArray(ArrayManager.RemoveValuesOutOfIndexRange(normalizedArray, hueBounds.Item1, hueBounds.Item2));
        // processedArray[hueBounds.Item1] /= 3;
        // processedArray[hueBounds.Item2] /= 3;
        savedHuesArray = ArrayManager.MergeArrays(savedHuesArray, processedArray, 1, 3, 20);
        FileUtils.IntArrayToFile(savedHuesArray, savedHuesArrayFileName);

        Shader.SetGlobalFloat("_GrayScaleSensitivity", Math.Max(12, Math.Max((-1.0f/300.0f)*(float)Math.Pow(maxIndex-20, 2)+35, (-1.0f/300.0f)*(float)Math.Pow(maxIndex-415, 2)+35)));
        Shader.SetGlobalFloat("_MinimumHue", hueBounds.Item1);
        Shader.SetGlobalFloat("_MaximumHue", hueBounds.Item2);

        // smoothedHues = ArrayManager.MergeArrays(normalizedArray, savedHuesArray, 1, 1, 10);//REMOVE THIS AFTER TESTING

        for(int i = 0; i < pixels.Length; i++) {
            Vector2 pixelPosition = new Vector2(i % lineLength, i / lineLength);
            int weightAmount = (int)((1.0f - Math.Min(Vector2.Distance(pixelPosition, centerPosition) / maxDistance, 1)) * 5);

            float quadraticOne = (-1.0f/25.0f)*(float)Math.Pow(hsvPixels[i].h * 360, 2)+25;
            float quadraticTwo = (-1.0f/25.0f)*(float)Math.Pow((hsvPixels[i].h * 360)-359, 2)+25;
            float grayScaleFiltering = Math.Max(10, Math.Max(quadraticOne, quadraticTwo));

            if(!ColorUtils.ColorIsGrayScale(pixels[i], grayScaleFiltering) && Utils.IsInCircularRange((int)(hsvPixels[i].h * 360), hueBounds.Item1, hueBounds.Item2)) {
                saturations[(int)(hsvPixels[i].s * 100)] += weightAmount;
                values[(int)(hsvPixels[i].v * 100)] += weightAmount;
            }
        }
        
        // Get Saturation Bounds
        smoothedSaturations = ArrayManager.SmoothIntArray(saturations, 10, false);
        Tuple<int, int> satBounds = ArrayManager.GetBoundsOfHighestDensityValues(smoothedSaturations, 5, 15, 10, false, 4, false);
        Shader.SetGlobalFloat("_MinimumSaturation", satBounds.Item1);
        Shader.SetGlobalFloat("_MaximumSaturation", satBounds.Item2);

        // Get Value Bounds
        smoothedValues = ArrayManager.SmoothIntArray(values, 10, false);
        Tuple<int, int> valBounds = ArrayManager.GetBoundsOfHighestDensityValues(smoothedValues, 5, 15, 10, false, 4, false);
        Shader.SetGlobalFloat("_MinimumValue", valBounds.Item1);
        Shader.SetGlobalFloat("_MaximumValue", valBounds.Item2);
    }

    static public void DeleteSavedHues()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, savedHuesArrayFileName));
        Array.Clear(savedHuesArray, 0, 360);
    }
}
