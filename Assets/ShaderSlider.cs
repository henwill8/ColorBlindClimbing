using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderSlider : MonoBehaviour
{
    public string valueName;

    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        Shader.SetGlobalFloat(valueName, slider.value);
    }

    void Update()
    {
        slider.value = Shader.GetGlobalFloat(valueName);
    }

    // Update is called once per frame
    public void UpdateSlider()
    {
        Shader.SetGlobalFloat(valueName, slider.value);
    }

}
