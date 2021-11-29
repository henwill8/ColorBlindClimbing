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

    public Material lineMaterial;

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
            Vector2 screenSize = transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta;

            if(backCam.videoRotationAngle % 180 == 0) {
                rt.sizeDelta = new Vector2(screenSize.x, screenSize.x * aspectRatio);
            } else if((backCam.videoRotationAngle + 90) % 180 == 0) {
                aspectRatio = (float)backCam.width / (float)backCam.height;
                rt.sizeDelta = new Vector2(screenSize.x * aspectRatio, screenSize.x);
            }

            // Vector2 pos1 = new Vector2(-(rt.sizeDelta.x / 4), -(rt.sizeDelta.y / 4));
            // Vector2 pos2 = new Vector2(-(rt.sizeDelta.x / 4), rt.sizeDelta.y / 4);
            // Vector2 pos3 = new Vector2(rt.sizeDelta.x / 4, rt.sizeDelta.y / 4);
            // Vector2 pos4 = new Vector2(rt.sizeDelta.x / 4, -(rt.sizeDelta.y / 4));

            // Lines.CreateLine(pos1, pos2, transform.parent, new Color(1, 1, 1), lineMaterial, 7);
            // Lines.CreateLine(pos2, pos3, transform.parent, new Color(1, 1, 1), lineMaterial, 7);
            // Lines.CreateLine(pos3, pos4, transform.parent, new Color(1, 1, 1), lineMaterial, 7);
            // Lines.CreateLine(pos4, pos1, transform.parent, new Color(1, 1, 1), lineMaterial, 7);

            rawImage.enabled = true;
        }
    }

    public void SelectColor() {
        ColorBoundsHandler.GetHighlightColor(backCam);

        rawImage.material.shader = customShader;
    }
}
