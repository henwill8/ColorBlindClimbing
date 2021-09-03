using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour
{
    public Slider slider;

    TMPro.TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float value = ((float)((int)(slider.value * 1000)))/1000;
        text.text = value.ToString();
    }
}
