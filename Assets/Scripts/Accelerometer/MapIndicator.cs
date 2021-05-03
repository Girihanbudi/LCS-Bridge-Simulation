using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapIndicator : MonoBehaviour
{
    public enum Indicator { OK, Warning, Danger }
    public Indicator mapIndicator;
    
    public float timeLag = 1.0f;
    private float currentTimeLag;
    private Image imageIndicator;

    // Start is called before the first frame update
    void Start()
    {
        imageIndicator = GetComponent<Image>();
        currentTimeLag = timeLag;
    }

    private void Update()
    {
        currentTimeLag -= Time.deltaTime;
    }

    public void ChangeStatus(float value, Treshold graphTreshold)
    {
        if (value >= graphTreshold.dangerTreshold)
        {
            mapIndicator = Indicator.Danger;
            currentTimeLag = timeLag;
            imageIndicator.color = Color.red;
        }
        else if (value <= -graphTreshold.dangerTreshold)
        {
            mapIndicator = Indicator.Danger;
            currentTimeLag = timeLag;
            imageIndicator.color = Color.red;
        }
        else if (value >= graphTreshold.cautionTreshold)
        {
            mapIndicator = Indicator.Warning;
            currentTimeLag = timeLag;
            imageIndicator.color = Color.yellow;
        } 
        
        else if (value <= -graphTreshold.cautionTreshold)
        {
            mapIndicator = Indicator.Warning;
            currentTimeLag = timeLag;
            imageIndicator.color = Color.yellow;
        }
        else 
        {
            if (currentTimeLag <= 0)
            {
                mapIndicator = Indicator.OK;
                imageIndicator.color = Color.green;
            }
        }
    }


    
}
