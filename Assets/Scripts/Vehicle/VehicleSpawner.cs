using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSpawner : MonoBehaviour
{
    public InputField speedInput;
    public InputField weightInput;
    public float speed;
    public float weight;
    public float minSpeed = 10.0f;
    public float maxSpeed = 100.0f;

    [System.Serializable]
    public struct WeightRange{
        public string name;
        public int vehicleObj;
        public int minWeight;
        public int maxWeight;
    }
    
    [Space]
    public List<WeightRange> carsWeightRange;
    public List<Button> vehicleButtons;

    [Space]
    public List<GameObject> vehicles;
    public int currentVehicle = 0;

    [Space]
    public List<GameObject> spawnPoints;
    public int currentSpawnPoint = 0;

    private float interval = 1f;
    private float currentInterval;

    private void Start()
    {
        speed = 50;
        speedInput.text = speed.ToString();
        weightInput.text = carsWeightRange[0].minWeight.ToString();
        currentInterval = 0;
    }

    private void Update()
    {
        if (currentInterval > 0)
            currentInterval -= Time.deltaTime;
    }

    public void OnSpeedChange()
    {
        if (speedInput.text == "" || speedInput.text == "." || speedInput.text == "-") speedInput.text = "0";
        speed = float.Parse(speedInput.text.ToString());

        if (speed < minSpeed)
        {
            speed = minSpeed;
            speedInput.text = speed.ToString();
        } else 
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
            speedInput.text = speed.ToString();
        }
    }

    public void OnWeightChange()
    {
        if (weightInput.text == "" || speedInput.text == "-") weightInput.text = "0";
        weight = float.Parse(weightInput.text);

        int smallestWeight = carsWeightRange[0].minWeight;
        if (weight < smallestWeight)
        {
            weight = smallestWeight;
            weightInput.text = weight.ToString();
        }

        int biggestWeight = carsWeightRange[carsWeightRange.Count - 1].maxWeight;
        if (weight > biggestWeight) 
        {
            weight = biggestWeight;
            weightInput.text = weight.ToString();
        }

        foreach (WeightRange range in carsWeightRange)
        {
            if(weight >= range.minWeight && weight <= range.maxWeight)
            {
                currentVehicle = range.vehicleObj;
                SetActiveUI vehicleUI = vehicleButtons[currentVehicle].GetComponent<SetActiveUI>();
                vehicleUI.ActivateCurrent();
                break;
            } 
        }
    }

    public void SetVehicle(int vehicleNumber)
    {
        currentVehicle = vehicleNumber;
    }

    public void SetSpawnPoint(int spawnPoint)
    {
        currentSpawnPoint = spawnPoint;
    }

    public void SpawnVechile()
    {
        if(currentInterval <= 0)
        {
            if (currentSpawnPoint == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    SaveSpawnVehicle saveSpawn = spawnPoints[i].GetComponent<SaveSpawnVehicle>();
                    if (saveSpawn.saveSpawning)
                    {

                        GameObject vehicle = Instantiate(vehicles[currentVehicle],
                                            spawnPoints[i].transform.position,
                                            spawnPoints[i].transform.rotation) as GameObject;

                        VehicleMotorStatic vehicleMotor = vehicle.GetComponent<VehicleMotorStatic>();
                        vehicleMotor.enableMove = true;
                        vehicleMotor.speed = speed;
                        vehicleMotor.weight = float.Parse(weightInput.text);
                    }
                }
            }
            else
            {
                SaveSpawnVehicle saveSpawn = spawnPoints[currentVehicle].GetComponent<SaveSpawnVehicle>();
                if (saveSpawn.saveSpawning)
                {
                    GameObject vehicle = Instantiate(vehicles[currentVehicle],
                                            spawnPoints[currentSpawnPoint].transform.position,
                                            spawnPoints[currentSpawnPoint].transform.rotation) as GameObject;

                    VehicleMotorStatic vehicleMotor = vehicle.GetComponent<VehicleMotorStatic>();
                    vehicleMotor.enableMove = true;
                    vehicleMotor.speed = speed;
                    vehicleMotor.weight = float.Parse(weightInput.text);
                }
            }

            currentInterval = interval;
        }
    }
}
