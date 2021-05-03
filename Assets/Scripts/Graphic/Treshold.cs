using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treshold : MonoBehaviour
{
    public float cautionTreshold = 1.0f;
    public float dangerTreshold = 1.5f;

    [Space]
    public InputField cautionTresholdInput;
    public InputField dangerTresholdInput;

    private AccelerometerGraph accl;

    private void Start()
    {
        cautionTresholdInput.text = cautionTreshold.ToString();
        dangerTresholdInput.text = dangerTreshold.ToString();

        accl = transform.parent.parent.GetComponent<AccelerometerGraph>();
        
    }

    public void ChangeDangerTreshold()
    {
        dangerTreshold = float.Parse(dangerTresholdInput.text);
        accl.UpdateTreshold(accl.tresholdDanger1, accl.tresholdDangerText1, "Danger", dangerTreshold);
        accl.UpdateTreshold(accl.tresholdDanger2, accl.tresholdDangerText2, "Danger", -dangerTreshold);
    }

    public void ChangeCautionTreshold()
    {
        cautionTreshold = float.Parse(cautionTresholdInput.text);
        accl.UpdateTreshold(accl.tresholdCaution1, accl.tresholdCautionText1, "Caution", cautionTreshold);
        accl.UpdateTreshold(accl.tresholdCaution2, accl.tresholdCautionText2, "Caution", -cautionTreshold);
    }
}
