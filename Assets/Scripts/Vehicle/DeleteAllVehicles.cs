using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteAllVehicles : MonoBehaviour
{
    public SaveSpawnVehicle saveSpawn1;
    public SaveSpawnVehicle saveSpawn2;

    public void DeleteAllVehiclesInScene()
    {
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        foreach(GameObject vehicle in vehicles)
        {
            Destroy(vehicle);
        }

        saveSpawn1.vehicles.Clear();
        saveSpawn2.vehicles.Clear();
        saveSpawn1.saveSpawning = true;
        saveSpawn2.saveSpawning = true;
    }
}
