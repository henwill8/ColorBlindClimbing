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

    public string deviceName;
    
    public Shader customShader;

    void Start()
    {
        rawImage = GetComponent<RawImage>();

        if (backCam == null)
            backCam = new WebCamTexture(deviceName);

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
    }

    public void SelectColor() {
        ColorBoundsHandler.GetHighlightColor(backCam);

        rawImage.material.shader = customShader;
    }
}
