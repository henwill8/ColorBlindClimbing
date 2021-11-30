using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevBuildManager : MonoBehaviour
{
    public bool isDevBuild;

    public GameObject[] DevObjects;
    public GameObject[] ReleaseObjects;

    // Start is called before the first frame update
    void Start()
    {
        if(isDevBuild) {
            for(int i = 0; i < ReleaseObjects.Length; i++) {
                Destroy(ReleaseObjects[i]);
            }
        } else {
            for(int i = 0; i < DevObjects.Length; i++) {
                Destroy(DevObjects[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
