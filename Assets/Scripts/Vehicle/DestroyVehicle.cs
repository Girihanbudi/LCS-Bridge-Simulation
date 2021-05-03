using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVehicle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DeletePoint")
        {
            Destroy(this.gameObject);
        }
    }
}
