using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInit : MonoBehaviour
{
    Camera thisCam;
    // Start is called before the first frame update
    void Start()
    {
        thisCam = GetComponent<Camera>();

        if (PlayerPrefs.HasKey(thisCam.name))
        {
            int fov = PlayerPrefs.GetInt(thisCam.name);
            thisCam.fieldOfView = fov;
        } else
        {
            thisCam.fieldOfView = 60;
        }
    }
}
