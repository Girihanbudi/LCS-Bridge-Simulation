using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSpawnerTraffic : MonoBehaviour
{
    public enum TrafficMode { Light, Normal, Heavy }
    public TrafficMode currentTraffic;

    [System.Serializable]
    public class Indicator
    {
        public Image imageIcon;
        public Image imageBackground;
    }

    public float lengthBetweenInstance = 10.0f;
    public int maxSpawnInstance = 20;

    [Space]
    public Dropdown trafficModeDropDown;
    public InputField weightInputMin;
    public InputField weightInputMax;

    [Space]
    public float minSpeedLimit = 15.0f;
    public float maxSpeedLimit = 100.0f;

    private float minSpeed;
    private int minWeight, maxWeight;
    private List<int> spawnPointChoice;

    public bool simulated = false;
    private bool usePoint1 = false, usePoint2 = false;

    [Space]
    public Color deactivateColor = new Color(255, 255, 255, 0.3f);
    public Color activeColor = new Color(255, 255, 255, 255);

    [System.Serializable]
    public struct WeightRange
    {
        public string name;
        public int vehicleObj;
        public int minWeight;
        public int maxWeight;
    }

    [Space]
    public List<WeightRange> carsWeightRange;

    [Space]
    public List<GameObject> vehicles;
    public int currentVehicle = 0;

    [Space]
    public List<GameObject> spawnPoints;

    [Space]
    public List<Indicator> weightIndicators;
    public List<Indicator> spawnIndicators;

    [Space]
    public Image simulateIndicator;
    public Color simulateOn;
    public Color simulateOff;

    private bool firstTimeSimulate = true;
    public List<GameObject> westCollectionInstance;
    public List<GameObject> eastCollectionInstance;

    private float lagTime;
    private float lagInterval;

    private int countVehicleToLagging;
    private int currentCountVehicle;

    [Space]
    private DeleteAllVehicles deleteVehicles;

    private void Start()
    {
        spawnPointChoice = new List<int>();

        weightInputMin.text = carsWeightRange[0].minWeight.ToString();
        weightInputMax.text = carsWeightRange[2].maxWeight.ToString();
        minWeight = carsWeightRange[0].minWeight;
        maxWeight = carsWeightRange[2].maxWeight;

        AddOrRemovePoint(0);
        AddOrRemovePoint(1);
        AddOrRemovePoint(2);

        ChangeWeightIndicator();

        currentTraffic = TrafficMode.Heavy;
        trafficModeDropDown.value = (int)currentTraffic;
        ChangeTrafficMode(currentTraffic);

        westCollectionInstance = new List<GameObject>();
        eastCollectionInstance = new List<GameObject>();

        deleteVehicles = GameObject.FindWithTag("GameManager").GetComponent<DeleteAllVehicles>();
    }

    private void Update()
    {
        if (!simulated) return;

        if (firstTimeSimulate) InitSpawn();
        else
        {
            IntervalSpawn();
        }
    }

    public void OnTrafficModeChange()
    {
        currentTraffic = (TrafficMode)trafficModeDropDown.value;
        ChangeTrafficMode(currentTraffic);
    }

    public void ChangeTrafficMode(TrafficMode mode)
    {
        if(mode == TrafficMode.Light)
        {
            minSpeed = 25;
            maxSpawnInstance = 5;
            lengthBetweenInstance = 60;
            lagInterval = 5.0f;
            lagTime = 4.0f;
            countVehicleToLagging = 6;
        } 
        else if (mode == TrafficMode.Normal) 
        {
            minSpeed = 20;
            maxSpawnInstance = 8;
            lengthBetweenInstance = 50;
            lagInterval = 3.0f;
            lagTime = 5.0f;
            countVehicleToLagging = 5;
        } 
        else if (mode == TrafficMode.Heavy) 
        {
            minSpeed = 15;
            maxSpawnInstance = 10;
            lengthBetweenInstance = 35;
            lagInterval = 1.0f;
            lagTime = 6.0f;
            countVehicleToLagging = 3;
        }
    }

    public void OnMinWeightChange()
    {
        if (weightInputMin.text == "" || weightInputMin.text == "-") weightInputMin.text = "0";
        minWeight = int.Parse(weightInputMin.text);

        if (weightInputMax.text == "" || weightInputMax.text == "-") weightInputMax.text = "0";
        maxWeight = int.Parse(weightInputMax.text);

        if (minWeight > maxWeight) 
        {
            minWeight = maxWeight;
            weightInputMin.text = minWeight.ToString();
        }

        int smallestWeight = carsWeightRange[0].minWeight;
        if (minWeight < smallestWeight)
        {
            minWeight = smallestWeight;
            weightInputMin.text = minWeight.ToString();
        }

        int biggestWeight = carsWeightRange[carsWeightRange.Count - 1].maxWeight;
        if (minWeight > biggestWeight)
        {
            minWeight = biggestWeight;
            weightInputMax.text = minWeight.ToString();
        }

        ChangeWeightIndicator();
    }

    public void OnMaxWeightChange()
    {
        if (weightInputMin.text == "" || weightInputMin.text == "-") weightInputMin.text = "0";
        minWeight = int.Parse(weightInputMin.text);

        if (weightInputMax.text == "" || weightInputMax.text == "-") weightInputMax.text = "0";
        maxWeight = int.Parse(weightInputMax.text);

        if (maxWeight < minWeight) 
        {
            maxWeight = minWeight;
            weightInputMax.text = maxWeight.ToString();
        }
        
        int smallestWeight = carsWeightRange[0].minWeight;
        if (maxWeight < smallestWeight)
        {
            maxWeight = smallestWeight;
            weightInputMin.text = maxWeight.ToString();
        }

        int biggestWeight = carsWeightRange[carsWeightRange.Count - 1].maxWeight;
        if (maxWeight > biggestWeight)
        {
            maxWeight = biggestWeight;
            weightInputMax.text = maxWeight.ToString();
        }

        ChangeWeightIndicator();
    }

    private void ChangeWeightIndicator()
    {
        int i = 0;
        foreach (WeightRange carWeightRange in carsWeightRange)
        {
            // IS SELECTED
            if ((carWeightRange.minWeight >= minWeight && carWeightRange.minWeight <= maxWeight) ||
                (carWeightRange.maxWeight >= minWeight && carWeightRange.maxWeight <= maxWeight))
            {
                weightIndicators[i].imageIcon.color = activeColor;
                weightIndicators[i].imageBackground.color = new Color(0, 0, 0, 0.1f);
            } 
            // IS NOT SELECTED
            else
            {
                weightIndicators[i].imageIcon.color = deactivateColor;
                weightIndicators[i].imageBackground.color = new Color(0, 0, 0, 0.75f);
            }
            i++;
        }
    }

    public void AddOrRemovePoint(int point)
    {
        bool isCurrentPoint;

        switch(point){
            case 0:
                usePoint1 = !usePoint1;
                if (usePoint1)
                {
                    spawnPointChoice.Add(point);
                    spawnIndicators[0].imageIcon.color = activeColor;
                }
                else
                {
                    spawnPointChoice.Remove(point);
                    spawnIndicators[0].imageIcon.color = deactivateColor;
                }
                break;

            case 1:
                usePoint2 = !usePoint2;
                if (usePoint2)
                {
                    spawnPointChoice.Add(point);
                    spawnIndicators[1].imageIcon.color = activeColor;
                }
                else
                {
                    spawnPointChoice.Remove(point);
                    spawnIndicators[1].imageIcon.color = deactivateColor;
                }
                break;
        }
    }

    public void TurnOnOffSimulate() 
    {
        simulated = !simulated;

        if (simulated)
        {
            deleteVehicles.DeleteAllVehiclesInScene();

            westCollectionInstance = westCollectionInstance.Where(item => item != null).ToList();
            eastCollectionInstance = eastCollectionInstance.Where(item => item != null).ToList();

            if (westCollectionInstance.Count > 0 || eastCollectionInstance.Count > 0)
            {    
                simulated = false;
                return;
            }
            simulateIndicator.color = simulateOn;
            firstTimeSimulate = true;
        } else
        {
            deleteVehicles.DeleteAllVehiclesInScene();
            westCollectionInstance = new List<GameObject>();
            eastCollectionInstance = new List<GameObject>();
            simulateIndicator.color = simulateOff;
        }
    }

    void InitSpawn()
    {
        firstTimeSimulate = false;

        GameObject prevInstantiated = null;

        foreach (int sp in spawnPointChoice)
        {
            int direction = 1;
            if (sp == 1) direction = -1;

            for (int i = 0; i < maxSpawnInstance; i++)
            {
                // Choice vehicle By random weight
                int currentWeightChoice = Random.Range(minWeight, maxWeight);

                foreach (WeightRange range in carsWeightRange)
                {
                    if (currentWeightChoice >= range.minWeight && currentWeightChoice <= range.maxWeight)
                    {
                        currentVehicle = range.vehicleObj;
                        break;
                    }
                }


                GameObject vehicle = null;
                if (i == 0)
                {
                    vehicle = Instantiate(vehicles[currentVehicle],
                                        spawnPoints[sp].transform.position,
                                        spawnPoints[sp].transform.rotation) as GameObject;

                    prevInstantiated = vehicle;
                } else
                {
                    VehicleMotorStatic prevVehicleMotor = prevInstantiated.GetComponent<VehicleMotorStatic>();

                    Vector3 spawnPoint = new Vector3(spawnPoints[sp].transform.position.x, spawnPoints[sp].transform.position.y, prevVehicleMotor.breakLimitPos.position.z + (lengthBetweenInstance * direction));

                    vehicle = Instantiate(vehicles[currentVehicle],
                                         spawnPoint,
                                         spawnPoints[sp].transform.rotation) as GameObject;

                    prevInstantiated = vehicle;
                }

                VehicleMotorStatic vehicleMotor = vehicle.GetComponent<VehicleMotorStatic>();
                vehicleMotor.enableMove = true;
                vehicleMotor.speed = minSpeed;
                vehicleMotor.weight = currentWeightChoice;
                vehicleMotor.randomLagInterval = lagInterval;
                vehicleMotor.randomLagTime = lagTime;
                vehicleMotor.useLagging = true;

                if (sp == 0) westCollectionInstance.Add(vehicle);
                else if (sp == 1) eastCollectionInstance.Add(vehicle);
            }
        }
    }

    void IntervalSpawn()
    {
        //if (westCollectionInstance.Count >= maxSpawnInstance || eastCollectionInstance.Count >= maxSpawnInstance)
        //{
            westCollectionInstance = westCollectionInstance.Where(item => item != null).ToList();
            eastCollectionInstance = eastCollectionInstance.Where(item => item != null).ToList();

        //   return;
        //}

        // float currentSpeed = Random.Range(minSpeed, maxSpeed);
        float currentSpeed = minSpeed;
        int currentWeightChoice = Random.Range(minWeight, maxWeight);

        foreach (WeightRange range in carsWeightRange)
        {
            if (currentWeightChoice >= range.minWeight && currentWeightChoice <= range.maxWeight)
            {
                currentVehicle = range.vehicleObj;
                break;
            }
        }

        int spawnIndex = 0;
        foreach (int i in spawnPointChoice)
        {
            SaveSpawnVehicle saveSpawn = spawnPoints[i].GetComponent<SaveSpawnVehicle>();
            if (saveSpawn.saveSpawning)
            {
                GameObject vehicle = Instantiate(vehicles[currentVehicle],
                                    spawnPoints[i].transform.position,
                                    spawnPoints[i].transform.rotation) as GameObject;

                VehicleMotorStatic vehicleMotor = vehicle.GetComponent<VehicleMotorStatic>();
                vehicleMotor.enableMove = true;
                vehicleMotor.speed = currentSpeed;
                vehicleMotor.weight = currentWeightChoice;
                vehicleMotor.randomLagInterval = lagInterval;
                vehicleMotor.randomLagTime = lagTime;
                vehicleMotor.useLagging = true;

                if (i == 0) westCollectionInstance.Add(vehicle);
                else eastCollectionInstance.Add(vehicle);
            }
            spawnIndex++;
        }

    }
}
