using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Lines
{
    static public GameObject CreateLine(Vector2 a, Vector2 b, Transform parent, Color color, Material material, float thickness) {
        GameObject go = new GameObject("line");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.material = material;
        image.color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        Vector2 dir = (b - a).normalized;
        float dist = Vector2.Distance(a, b);
        rt.sizeDelta = new Vector2(dist, thickness);
        rt.anchoredPosition = a + dir * dist * 0.5f;
        rt.localEulerAngles = new Vector3(0, 0, (float)System.Math.Atan2(dir.y, dir.x) * 57.29578f);

        return go;
    }
}
