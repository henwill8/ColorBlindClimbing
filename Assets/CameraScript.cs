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

    public void GetHighlightColor()
    {
        Debug.Log("Getting pixels");
        Color[] pixels = backCam.GetPixels(backCam.width / 4, backCam.height / 4, backCam.width / 2, backCam.height / 2);

        int totalPixels = pixels.Length;
        int coloredPixels = 0;

        int[] hues = new int[pixels.Length];
        int[] saturations = new int[pixels.Length];
        int[] values = new int[pixels.Length];

        foreach (var pixel in pixels)
        {
            float h, s, v;
            Color.RGBToHSV(pixel, out h, out s, out v);

            // if(s > 0.25f && v < 0.8f) {
                hues[coloredPixels] = (int)(h * 360);
                saturations[coloredPixels] = (int)(s * 100);
                values[coloredPixels] = (int)(v * 100);
                coloredPixels++;
            // }
        }

        int[] hueOccurrencesCounted = ArrayManager.IntValueCounter(hues, 360);
        Tuple<int, int> hueBounds = ArrayManager.GetBoundsOfHighestDensityValues(hueOccurrencesCounted, 3.5f);

        Shader.SetGlobalFloat("_MinimumHue", hueBounds.Item1);
        Shader.SetGlobalFloat("_MaximumHue", hueBounds.Item2);

        float removePercentage = 0.005f;

        int[] satInBoundValues = ArrayManager.RemoveOutOfBoundValues(saturations, hues, hueBounds.Item1, hueBounds.Item2);
        Tuple<int, int> satBounds = ArrayManager.GetBoundsFromPercentRange(satInBoundValues, removePercentage, 1 - removePercentage);

        Shader.SetGlobalFloat("_MinimumSaturation", satBounds.Item1);
        Shader.SetGlobalFloat("_MaximumSaturation", satBounds.Item2);

        int[] valInBoundValues = ArrayManager.RemoveOutOfBoundValues(values, hues, hueBounds.Item1, hueBounds.Item2);
        Tuple<int, int> valBounds = ArrayManager.GetBoundsFromPercentRange(valInBoundValues, removePercentage, 1 - removePercentage);

        Shader.SetGlobalFloat("_MinimumValue", valBounds.Item1);
        Shader.SetGlobalFloat("_MaximumValue", valBounds.Item2);
        
        rawImage.material.shader = customShader;

        Debug.Log("Done!");
    }
}
