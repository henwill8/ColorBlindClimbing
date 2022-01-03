using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

public class ColorBoundsHandler : MonoBehaviour
{
    static public int[] smoothedHues;
    static public int[] smoothedLuminosity;

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

        ColorHSL[] hslPixels = new ColorHSL[pixels.Length];
        int[] weightedHues = new int[360];
        int[] luminosity = new int[101];

        float grayScaleSensitivity = 0.2f;

        for(int i = 0; i < pixels.Length; i++) {
            Vector2 pixelPosition = new Vector2(i % lineLength, i / lineLength);

            ColorHSL hsl = ColorConversions.RGBtoHSL(pixels[i]);
            hslPixels[i] = hsl;

            int weightAmount = (int)((1.0f - Math.Min(Vector2.Distance(pixelPosition, centerPosition) / maxDistance, 1)) * 10);

            if(hsl.s > grayScaleSensitivity) weightedHues[(int)(hsl.h * 360)] += weightAmount;
        }
        
        // Get Hue Bounds
        smoothedHues = ArrayManager.SmoothIntArray(weightedHues, 10);
        int maxIndex = ArrayManager.GetHighestAverageIndex(smoothedHues);
        int[] normalizedArray = ArrayManager.NormalizeArray(smoothedHues, 1000);
        Tuple<int, int> hueBounds = ArrayManager.GetBoundsOfHighestDensityValues(ArrayManager.MergeArrays(normalizedArray, savedHuesArray, 1, 1, 10), 1.4f, 15, 10, true, 2, true, maxIndex);

        int[] processedArray = ArrayManager.ExtremifyArray(ArrayManager.RemoveValuesOutOfIndexRange(normalizedArray, hueBounds.Item1, hueBounds.Item2));
        savedHuesArray = ArrayManager.MergeArrays(savedHuesArray, processedArray, 1, 3, 20);
        FileUtils.IntArrayToFile(savedHuesArray, savedHuesArrayFileName);

        Shader.SetGlobalFloat("_MinimumHue", hueBounds.Item1);
        Shader.SetGlobalFloat("_MaximumHue", hueBounds.Item2);

        // smoothedHues = ArrayManager.MergeArrays(normalizedArray, savedHuesArray, 1, 1, 10);//REMOVE THIS AFTER TESTING

        for(int i = 0; i < pixels.Length; i++) {
            Vector2 pixelPosition = new Vector2(i % lineLength, i / lineLength);
            int weightAmount = (int)((1.0f - Math.Min(Vector2.Distance(pixelPosition, centerPosition) / maxDistance, 1)) * 5);

            if(hslPixels[i].s > grayScaleSensitivity && Utils.IsInCircularRange((int)(hslPixels[i].h * 360), hueBounds.Item1, hueBounds.Item2)) {
                luminosity[(int)(hslPixels[i].l * 100)] += weightAmount;
            }
        }
        
        // Get Saturation Bounds
        smoothedLuminosity = ArrayManager.SmoothIntArray(luminosity, 10, false);
        Tuple<int, int> luminosityBounds = ArrayManager.GetBoundsOfHighestDensityValues(smoothedLuminosity, 5, 15, 10, false, 4, false);
        Shader.SetGlobalFloat("_MinimumLuminosity", luminosityBounds.Item1);
        Shader.SetGlobalFloat("_MaximumLuminosity", luminosityBounds.Item2);
    }

    static public void DeleteSavedHues()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, savedHuesArrayFileName));
        Array.Clear(savedHuesArray, 0, 360);
    }
}
