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

    public void CreateGraph() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        int[] hues = cameraScript.hueOccurrencesCounted;
        if(hues.Length == 0) return;

        int hueMin = (int)Shader.GetGlobalFloat("_MinimumHue");
        int hueMax = (int)Shader.GetGlobalFloat("_MaximumHue");
        Debug.Log("Min and Max: "+hueMin+" "+hueMax);

        int minShown = ArrayManager.KeepInCircularRange(0, hues.Length-1, hueMin-25);
        int maxShown = ArrayManager.KeepInCircularRange(0, hues.Length-1, hueMax+25);
        Debug.Log("Shown Range: "+minShown+" "+maxShown);
        int valuesShown = maxShown - minShown;
        if(minShown > maxShown) {
            valuesShown = maxShown + (hues.Length-1 - minShown);
            maxShown += hues.Length;
        }
        Debug.Log("Values Shown: "+valuesShown);
        
        float lineLength = size.x / valuesShown;
        float maxHeight = hues.Max();

        for(int i = minShown; i < maxShown; i++) {
            Vector2 startPoint = new Vector2((i-minShown)*lineLength - size.x/2, size.y * (hues[ArrayManager.KeepInCircularRange(0, hues.Length-1, i)] / maxHeight) - size.y/2);
            Vector2 endPoint = new Vector2((i+1-minShown)*lineLength - size.x/2, size.y * (hues[ArrayManager.KeepInCircularRange(0, hues.Length-1, i+1)] / maxHeight) - size.y/2);
            CreateLine(startPoint, endPoint, transform, Color.HSVToRGB((float)i/360f, 1, 1));
            if(ArrayManager.KeepInCircularRange(0, hues.Length-1, i) == hueMin || ArrayManager.KeepInCircularRange(0, hues.Length-1, i) == hueMax) {
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
}
