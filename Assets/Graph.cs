using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Graph : MonoBehaviour
{
    public CameraScript cameraScript;
    public Material lineMaterial;

    public Vector2 size;

    public float lineThickness;

    // Start is called before the first frame update
    void OnEnable()
    {
        CreateGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateGraph() {//Make narrowing system based on bounds so that graph is easier to view
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        int[] hues = cameraScript.hueOccurrencesCounted;
        int hueMin = (int)Shader.GetGlobalFloat("_MinimumHue");
        int hueMax = (int)Shader.GetGlobalFloat("_MaximumHue");

        int minShown = System.Math.Max(hueMin-25, 0);
        int maxShown = System.Math.Min(hueMax+25, hues.Length-1);
        int valuesShown = maxShown - minShown;
        
        float lineLength = size.x / valuesShown;
        float maxHeight = hues.Max();

        for(int i = minShown; i < maxShown; i++) {
            Vector2 startPoint = new Vector2((i-minShown)*lineLength - size.x/2, size.y * (hues[i] / maxHeight) - size.y/2);
            Vector2 endPoint = new Vector2((i+1-minShown)*lineLength - size.x/2, size.y * (hues[i+1] / maxHeight) - size.y/2);
            CreateLine(startPoint, endPoint, transform, Color.HSVToRGB((float)i/360f, 1, 1));
            if(i == hueMin || i == hueMax) {
                CreateLine(new Vector2((i-minShown)*lineLength - size.x/2, -size.y/2), new Vector2((i-minShown)*lineLength - size.x/2, size.y/2), transform, new Color(0, 0, 0));
            }
        }
    }

    void CreateLine(Vector2 a, Vector2 b, Transform parent, Color color) {
        GameObject go = new GameObject("line");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.material = lineMaterial;
        image.color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        Vector2 dir = (b - a).normalized;
        float dist = Vector2.Distance(a, b);
        rt.sizeDelta = new Vector2(dist, lineThickness);
        rt.anchoredPosition = a + dir * dist * 0.5f;
        rt.localEulerAngles = new Vector3(0, 0, (float)System.Math.Atan2(dir.y, dir.x) * 57.29578f);
    }

    // float getXPos(int i) {
    //     int offset = 9;
    //     if(!npsLinesEnabled) offset = 20;
    //     float pointDistance = (graphData.size.x - convertToGraphSizeX(offset)) / float(graphPoints.size()-1);
    //     return i*pointDistance + convertToGraphSizeX(graphOffset.x);
    // }

    // float getYPos(int i) {
    //     return ((float)graphPoints[i].second / (float)maxNoteCount) * graphData.size.y + convertToGraphSizeY(graphOffset.y);
    // }
}
