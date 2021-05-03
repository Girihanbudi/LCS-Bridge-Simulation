using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCurrentCamera : MonoBehaviour
{
    public Camera current;
    public List<GameObject> others;

    [Space]
    public Text camText;
    public Image camImg;
    public Image camBackground;

    Color deactivateColor;
    Color activeColor;

    [Space]
    public bool activateOnAwake;

    [Space]
    public Slider camFOV;

    [Space]
    public bool useKeyboard = true;
    public string keyCode;

    public void Start()
    {
        deactivateColor = new Color(255, 255, 255, 0.3f);
        activeColor = new Color(255, 255, 255, 255);

        if(activateOnAwake)
        {
            ActivateCurrent();
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(keyCode) && useKeyboard)
        {
            ActivateCurrent();
        }
    }

    public void ActivateCurrent()
    {
        current.gameObject.SetActive(true);
        camText.color = activeColor;
        camImg.color = activeColor;
        camBackground.enabled = false;

        foreach (GameObject cam in others)
        {
            SetCurrentCamera otherCamAtr = cam.GetComponent<SetCurrentCamera>();
            otherCamAtr.current.gameObject.SetActive(false);

            otherCamAtr.camText.color = deactivateColor;
            otherCamAtr.camImg.color = deactivateColor;
            otherCamAtr.camBackground.enabled = true;
        }

        camFOV.value = current.fieldOfView;
    }
}
