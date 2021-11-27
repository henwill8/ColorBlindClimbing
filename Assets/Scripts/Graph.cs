using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph : MonoBehaviour
{
    public Material lineMaterial;

    public Vector2 size;

    public bool showSavedArray;

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

        int[] hues = ColorBoundsHandler.smoothedHues;
        if(showSavedArray) hues = ColorBoundsHandler.savedHuesArray;

        if(hues.Length == 0) return;
        
        int maxIndex = ArrayManager.GetHighestAverageIndex(ColorBoundsHandler.smoothedHues, 0);

        int hueMin = (int)Shader.GetGlobalFloat("_MinimumHue");
        int hueMax = (int)Shader.GetGlobalFloat("_MaximumHue");
        // Debug.Log("Min, MaxIndex, and Max: "+hueMin+" "+maxIndex+" "+hueMax);

        // int minShown = Utils.KeepInCircularRange(0, hues.Length-1, hueMin-25);
        // int maxShown = Utils.KeepInCircularRange(0, hues.Length-1, hueMax+25);
        int minShown = 0;
        int maxShown = 360;
        // Debug.Log("Shown Range: "+minShown+" "+maxShown);
        
        int valuesShown = maxShown - minShown;

        if(minShown > maxShown) {
            valuesShown = maxShown + (hues.Length-1 - minShown);
            maxShown += hues.Length;
        }

        // Debug.Log("Values Shown: "+valuesShown);
        
        float lineLength = size.x / valuesShown;
        float maxHeight = hues.Max();

        for(int i = minShown; i < maxShown; i++) {
            int circularRangeValue = Utils.KeepInCircularRange(0, hues.Length-1, i);

            Vector2 startPoint = new Vector2((i-minShown)*lineLength - size.x/2, size.y * (hues[circularRangeValue] / maxHeight) - size.y/2);
            Vector2 endPoint = new Vector2((i+1-minShown)*lineLength - size.x/2, size.y * (hues[Utils.KeepInCircularRange(0, hues.Length-1, i+1)] / maxHeight) - size.y/2);
            
            Lines.CreateLine(startPoint, endPoint, transform, Color.HSVToRGB((float)circularRangeValue/360f, 1, 1), lineMaterial, lineThickness);
            
            if(circularRangeValue == hueMin || circularRangeValue == hueMax || circularRangeValue == maxIndex) {
                Lines.CreateLine(new Vector2((i-minShown)*lineLength - size.x/2, -size.y/2), new Vector2((i-minShown)*lineLength - size.x/2, size.y/2), transform, new Color(1, 1, 1), lineMaterial, lineThickness);
            }
        }
    }
}
