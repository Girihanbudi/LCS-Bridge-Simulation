using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetView : MonoBehaviour
{
    public List<GameObject> cams; 

    public void ResetActiveCam()
    {
        foreach(GameObject cam in cams)
        {
            if (cam.activeSelf)
            {
                cam.GetComponent<MouseLook>().ResetRotation();
            }
        }
    }
}
