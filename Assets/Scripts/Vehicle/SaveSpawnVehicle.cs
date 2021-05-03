using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSpawnVehicle : MonoBehaviour
{
    public bool saveSpawning = true;

    public List<GameObject> vehicles;
    
    void Start()
    {
        vehicles = new List<GameObject>();
        saveSpawning = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            vehicles.Add(other.gameObject);
            saveSpawning = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            vehicles.Remove(other.gameObject);
        }

        if (vehicles.Count <= 0) saveSpawning = true;
    }
}
