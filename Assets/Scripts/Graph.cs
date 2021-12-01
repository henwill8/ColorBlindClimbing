using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph : MonoBehaviour
{
    public Material lineMaterial;

    public Vector2 size;

    public int showSavedArray;

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

        int[] hues = ColorBoundsHandler.smoothedSaturations;
        if(showSavedArray == 1) hues = ColorBoundsHandler.smoothedValues;
        else if(showSavedArray == 2) hues = ColorBoundsHandler.smoothedHues;
        else if(showSavedArray == 3) hues = ColorBoundsHandler.savedHuesArray;

        if(hues.Length == 0) return;
        
        int maxIndex = ArrayManager.GetHighestAverageIndex(hues, 0);
        if(showSavedArray == 3) maxIndex = ArrayManager.GetHighestAverageIndex(ColorBoundsHandler.smoothedHues, 0);

        int hueMin = (int)Shader.GetGlobalFloat("_MinimumSaturation");
        int hueMax = (int)Shader.GetGlobalFloat("_MaximumSaturation");
        if(showSavedArray == 1) {
            hueMin = (int)Shader.GetGlobalFloat("_MinimumValue");
            hueMax = (int)Shader.GetGlobalFloat("_MaximumValue");
        } else if(showSavedArray > 1) {
            hueMin = (int)Shader.GetGlobalFloat("_MinimumHue");
            hueMax = (int)Shader.GetGlobalFloat("_MaximumHue");
        }
        // Debug.Log("Min, MaxIndex, and Max: "+hueMin+" "+maxIndex+" "+hueMax);

        // int minShown = Utils.KeepInCircularRange(0, hues.Length-1, hueMin-25);
        // int maxShown = Utils.KeepInCircularRange(0, hues.Length-1, hueMax+25);
        int minShown = 0;
        int maxShown = hues.Length;
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
            int circularRangeValue = Utils.KeepInCircularRange(i, 0, hues.Length-1);

            Vector2 startPoint = new Vector2((i-minShown)*lineLength - size.x/2, size.y * (hues[circularRangeValue] / maxHeight) - size.y/2);
            Vector2 endPoint = new Vector2((i+1-minShown)*lineLength - size.x/2, size.y * (hues[Utils.KeepInCircularRange(i+1, 0, hues.Length-1)] / maxHeight) - size.y/2);
            
            Color color = Color.HSVToRGB((float)ArrayManager.GetHighestAverageIndex(ColorBoundsHandler.smoothedHues, 0) / 360.0f, (float)circularRangeValue / 100.0f, 1);
            if(showSavedArray == 1) color = Color.HSVToRGB(0, 0, (float)circularRangeValue / 100.0f);
            else if(showSavedArray > 1) color = Color.HSVToRGB((float)circularRangeValue / 360.0f, 1, 1);

            if(i < maxShown-1) Lines.CreateLine(startPoint, endPoint, transform, color, lineMaterial, lineThickness);
            
            if(circularRangeValue == hueMin || circularRangeValue == hueMax || circularRangeValue == maxIndex) {
                Lines.CreateLine(new Vector2((i-minShown)*lineLength - size.x/2, -size.y/2), new Vector2((i-minShown)*lineLength - size.x/2, size.y/2), transform, new Color(1, 1, 1), lineMaterial, lineThickness);
            }
        }
    }
}
