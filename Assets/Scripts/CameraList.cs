using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraList : MonoBehaviour
{
    public GameObject ButtonPrefab;

    public CameraScript cameraScript;

    public GameObject enablePermissionsText;
    public GameObject cameraText;

    bool created = false;

    // Start is called before the first frame update
    void Start()
    {
        cameraText.SetActive(false);
        enablePermissionsText.SetActive(false);

        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length > 0) CreateButtons(devices);
    }

    // Update is called once per frame
    void Update()
    {
        if(!created) {
            if(Time.realtimeSinceStartup < 10.0f) {
                WebCamDevice[] devices = WebCamTexture.devices;
            
                if(devices.Length > 0) CreateButtons(devices);
            } else {
                cameraText.SetActive(false);
                enablePermissionsText.SetActive(true);

                created = true;
            }
        }
    }

    void CreateButtons(WebCamDevice[] devices)
    {
        created = true;
        for(int i = 0; i < devices.Length; i++) {
            if(!devices[i].isFrontFacing && !Application.isEditor) {
                SelectCamera(devices[i]);
                return;
            }
            int value = i;
            GameObject newButton = Instantiate(ButtonPrefab);
            newButton.transform.SetParent(transform, false);
            newButton.name = devices[i].name;
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250 - (i*200));
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectCamera(devices[value]));

            GameObject buttonText = newButton.transform.GetChild(0).gameObject;
            buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = devices[i].name;

            cameraText.SetActive(true);
            enablePermissionsText.SetActive(false);
        }
    }

    public void SelectCamera(WebCamDevice device)
    {
        Debug.Log("Selected "+device.name);
        cameraScript.transform.parent.gameObject.SetActive(true);
        cameraScript.deviceName = device.name;
        gameObject.SetActive(false);
    }
}
