using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCollector : MonoBehaviour
{
    public List<GameObject> vehicles;

    // Start is called before the first frame update
    void Start()
    {
        vehicles = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            vehicles.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            vehicles.Remove(other.gameObject);
        }
    }
}
