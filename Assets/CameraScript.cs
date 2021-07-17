using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CameraScript : MonoBehaviour
{

    static WebCamTexture backCam;

    RawImage rawImage;

    public Shader customShader;

    public string deviceName;

    public Slider minimumSlider;
    public Slider maximumSlider;

    void Start()
    {
        rawImage = GetComponent<RawImage>();

        if (backCam == null)
            backCam = new WebCamTexture(deviceName);

        rawImage.material.mainTexture = backCam;
        rawImage.material.shader = Shader.Find("Standard");
        rawImage.texture = backCam;

        rawImage.material.SetColor("_Color", new Color(1, 1, 1, 1));

        rawImage.enabled = false;

        if (!backCam.isPlaying)
            backCam.Play();

    }

    void Update()
    {
        if(backCam.width > 100 && !rawImage.enabled) {
            transform.rotation = Quaternion.AngleAxis(-backCam.videoRotationAngle, Vector3.forward);

            float aspectRatio = (float)backCam.height / (float)backCam.width;
            
            Debug.Log("Camera Resolution is "+backCam.width+"x"+backCam.height);
            Debug.Log("Camera Aspect Ratio is "+aspectRatio);

            RectTransform rt = GetComponent<RectTransform>();
            float screenWidth = transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;
            if(backCam.videoRotationAngle % 180 == 0) {
                rt.sizeDelta = new Vector2(screenWidth, screenWidth * aspectRatio);
            } else if((backCam.videoRotationAngle + 90) % 180 == 0) {
                aspectRatio = (float)backCam.width / (float)backCam.height;
                rt.sizeDelta = new Vector2(screenWidth * aspectRatio, screenWidth);
            }

            rawImage.enabled = true;
        }

        // if(Time.time > 10) {
            // rawImage.material.shader = customShader;
        // }
    }

    public int KeepInCircularRange(int min, int max, int value)
    {
        while(value > max || value < min) {
            if(value > max) value -= max - min + 1;
            if(value < min) value += max - min + 1;
        }
        return value;
    }

    public int[] IntValueCounter(int[] inputArray, int length, int values)
    {
        int[] array = new int[values];

        for(int i = 0; i < length; i++) {
            array[inputArray[i]]++;
        }

        return array;
    }

    public Tuple<int, int> GetBoundsOfHighestDensityValues(int[] array)
    {
        int[] bounds = new int[2];
        int highestValue = 0;
        int highestIndex = 0;

        for(int i = 0; i < array.Length; i++) {
            int count = 0;
            for(int j = 0; j < 11; j++) {
                count += array[KeepInCircularRange(0, array.Length-1, i-j+5)];
            }
            if(count > highestValue) {
                highestValue = count;
                highestIndex = i;
            }
        }

        for(int i = 0; i < 2; i++) {
            int iterations = 0;
            int lowest = array.Max();

            while(iterations < 100) {
                if(i == 0) {
                    iterations--;
                } else {
                    iterations++;
                }

                int indexValue = KeepInCircularRange(0, array.Length-1, highestIndex + iterations);

                if(array[indexValue] < lowest || lowest == array.Max()) {
                    bounds[i] = indexValue;
                    lowest = array[indexValue];
                }

                if(array[indexValue] > array[bounds[i]] * 1.5f) break;
            }
        }

        Debug.Log("Max Index: "+highestIndex+", Min: "+bounds[0]+", Max: "+bounds[1]);

        return new Tuple<int, int>(bounds[0], bounds[1]);
    }

    public void GetHighlightColor()
    {
        Debug.Log("Getting pixels");
        Color[] pixels = backCam.GetPixels(backCam.width / 4, backCam.height / 4, backCam.width / 2, backCam.height / 2);

        int totalPixels = pixels.Length;
        int coloredPixels = 0;

        int[] hues = new int[pixels.Length];

        foreach (var pixel in pixels)
        {
            float h, s, v;
            Color.RGBToHSV(pixel, out h, out s, out v);

            // if(s > 0.25f && v < 0.8f) {
                hues[coloredPixels] = (int)(h * 360);
                coloredPixels++;
            // }
        }

        int[] occurrencesCounted = IntValueCounter(hues, coloredPixels, 360);

        Tuple<int, int> bounds = GetBoundsOfHighestDensityValues(occurrencesCounted);

        Shader.SetGlobalFloat("_MinimumHue", bounds.Item1);
        Shader.SetGlobalFloat("_MaximumHue", bounds.Item2);
        
        rawImage.material.shader = customShader;

        Debug.Log("Done!");
    }

    void ApplyShader()
    {

    }
}
