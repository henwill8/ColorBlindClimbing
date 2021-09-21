using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameObjectList
{
    public GameObject[] gameObjects;
}

public class UISwitcher : MonoBehaviour
{
    public GameObjectList[] objects;

    int index = -1;

    // Start is called before the first frame update
    void Start()
    {
        Button button = gameObject.GetComponent<Button>();
		button.onClick.AddListener(Switch);

        Switch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Switch() {
        index++;
        index = ArrayManager.KeepInCircularRange(0, objects.Length-1, index);
        Debug.Log("Switching to page "+(index+1));

        for(int i = 0; i < objects.Length; i++) {
            for(int j = 0; j < objects[i].gameObjects.Length; j++) {
                if(i == index) {
                    objects[i].gameObjects[j].SetActive(true);
                } else {
                    objects[i].gameObjects[j].SetActive(false);
                }
            }
        }
    }
}
