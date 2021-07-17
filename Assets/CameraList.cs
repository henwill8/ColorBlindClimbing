using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraList : MonoBehaviour
{
    public GameObject ButtonPrefab;

    public CameraScript cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        for(int i = 0; i < devices.Length; i++) {
            int value = i;
            GameObject newButton = Instantiate(ButtonPrefab);
            newButton.transform.SetParent(transform, false);
            newButton.name = devices[i].name;
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250 - (i*200));
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectCamera(devices[value]));

            GameObject buttonText = newButton.transform.GetChild(0).gameObject;
            buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = devices[i].name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectCamera(WebCamDevice device) {
        Debug.Log("Selected "+device.name);
        cameraScript.transform.parent.gameObject.SetActive(true);
        cameraScript.deviceName = device.name;
        gameObject.SetActive(false);
    }
}
