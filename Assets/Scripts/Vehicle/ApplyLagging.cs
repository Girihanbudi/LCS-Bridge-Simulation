using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLagging : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            VehicleMotorStatic vehicleMotor = other.GetComponent<VehicleMotorStatic>();
            vehicleMotor.hitLaggingPoint = true;
        }
    }
}
