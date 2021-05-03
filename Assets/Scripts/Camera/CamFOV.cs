using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamFOV : MonoBehaviour
{
    Slider fovSlider;

    private void Start()
    {
        fovSlider = GetComponent<Slider>();
        fovSlider.value = Camera.main.fieldOfView;
    }

    public void CamFOVControl()
    {
        Camera currentCam = Camera.main;

        currentCam.fieldOfView = (int)fovSlider.value;
        PlayerPrefs.SetInt(currentCam.name, (int)fovSlider.value);

    }
}
