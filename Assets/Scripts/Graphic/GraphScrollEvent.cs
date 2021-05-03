using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphScrollEvent : MonoBehaviour
{
    public bool isMouseOverThis;
    public int scrollVal = 12;
    public int minVal = 12;
    public int maxVal = 96;

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isMouseOverThis)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            scrollVal += (int)(scroll * 10);
            if (scrollVal <= minVal) scrollVal = minVal;
            else if (scrollVal >= maxVal) scrollVal = maxVal;
        }
    }

    public void OnMouseEnter()
    {
        isMouseOverThis = true;    
    }

    public void OnMouseLeave()
    {
        isMouseOverThis = false;
    }
}
