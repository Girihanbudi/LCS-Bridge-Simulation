using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSpawnerAuto : MonoBehaviour
{
    [System.Serializable]
    public class Indicator
    {
        public Image imageIcon;
        public Image imageBackground;
    }

    public InputField intervalInputMin;
    public InputField intervalInputMax;
    public InputField speedInputMin;
    public InputField speedInputMax;
    public InputField weightInputMin;
    public InputField weightInputMax;

    [Space]
    public float minIntervalLimit = 1.0f;
    public float maxIntervalLimit = 5.0f;
    public float minSpeedLimit = 15.0f;
    public float maxSpeedLimit = 100.0f;

    private const float speedDiffMax = 10.0f;
    private float currentIntervalTime;
    private float selectedIntervalTime;
    private float minInterval, maxInterval;
    private float minSpeed, maxSpeed;
    private int minWeight, maxWeight;
    public List<int> spawnPointChoice;

    public bool simulated = false;
    private bool usePoint1 = false, usePoint2 = false, usePoint3 = false;

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

    private void Start()
    {
        spawnPointChoice = new List<int>();

        intervalInputMin.text = minIntervalLimit.ToString();
        intervalInputMax.text = maxIntervalLimit.ToString();
        minInterval = minIntervalLimit;
        maxInterval = maxIntervalLimit;

        speedInputMin.text = 50.ToString();
        speedInputMax.text = 60.ToString();
        minSpeed = 50;
        maxSpeed = maxSpeedLimit;

        weightInputMin.text = carsWeightRange[0].minWeight.ToString();
        weightInputMax.text = carsWeightRange[2].maxWeight.ToString();
        minWeight = carsWeightRange[0].minWeight;
        maxWeight = carsWeightRange[2].maxWeight;

        AddOrRemovePoint(0);
        AddOrRemovePoint(1);
        AddOrRemovePoint(2);

        ChangeWeightIndicator();
    }

    private void Update()
    {
        if (!simulated) return;

        if(currentIntervalTime < selectedIntervalTime)
        {
            currentIntervalTime += Time.deltaTime;
        } 
        else
        {
            IntervalSpawn();
            currentIntervalTime = 0;
        }

    }

    public void OnMinIntervalChange()
    {
        if (intervalInputMin.text == "" || intervalInputMin.text == "." || intervalInputMin.text == "-") intervalInputMin.text = "0";
        minInterval = float.Parse(intervalInputMin.text.ToString());

        if (minInterval > maxInterval)
        {
            minInterval = maxInterval;
            intervalInputMin.text = minInterval.ToString();
        }

        if (minInterval < minIntervalLimit)
        {
            minInterval = minIntervalLimit;
            intervalInputMin.text = minInterval.ToString();
        }

        selectedIntervalTime = Random.Range(minInterval, maxInterval);
    }

    public void OnMaxIntervalChange()
    {
        if (intervalInputMax.text == "" || intervalInputMax.text == "." || intervalInputMax.text == "-") intervalInputMax.text = "0";
        maxInterval = float.Parse(intervalInputMax.text.ToString());

        if (maxInterval < minInterval) 
        {
            maxInterval = minInterval;
            intervalInputMax.text = maxInterval.ToString();
        }

        if (maxInterval > maxIntervalLimit)
        {
            maxInterval = maxIntervalLimit;
            intervalInputMax.text = maxInterval.ToString();
        }

        selectedIntervalTime = Random.Range(minInterval, maxInterval);
    }

    public void OnMinSpeedChange()
    {
        if(speedInputMin.text == "" || speedInputMin.text == "." || speedInputMin.text == "-") speedInputMin.text = "0";
        minSpeed = float.Parse(speedInputMin.text.ToString());

        if (minSpeed > maxSpeed) 
        {
            minSpeed = maxSpeed;
            speedInputMin.text = minSpeed.ToString();
        }

        if (maxSpeed - minSpeed > speedDiffMax)
        {
            maxSpeed = minSpeed + speedDiffMax;
            speedInputMin.text = minSpeed.ToString();
            speedInputMax.text = maxSpeed.ToString();
        }

        if (minSpeed < minSpeedLimit)
        {
            minSpeed = minSpeedLimit;
            speedInputMin.text = minSpeed.ToString();
        }        
    }

    public void OnMaxSpeedChange()
    {
        if (speedInputMax.text == "" || speedInputMax.text == "." || speedInputMax.text == "-") speedInputMax.text = "0";
        maxSpeed = float.Parse(speedInputMax.text.ToString());

        if (maxSpeed < minSpeed) 
        {
            maxSpeed = minSpeed;
            speedInputMax.text = maxSpeed.ToString();
        }

        if (maxSpeed - minSpeed > speedDiffMax) 
        {
            minSpeed = maxSpeed - speedDiffMax;
            speedInputMin.text = minSpeed.ToString();
            speedInputMax.text = maxSpeed.ToString();
        } 

        if (maxSpeed > maxSpeedLimit)
        {
            maxSpeed = maxSpeedLimit;
            speedInputMax.text = maxSpeed.ToString();
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

            case 2:
                usePoint3 = !usePoint3;
                if (usePoint3)
                {
                    spawnPointChoice.Add(point);
                    spawnIndicators[2].imageIcon.color = activeColor;
                }
                else
                {
                    spawnPointChoice.Remove(point);
                    spawnIndicators[2].imageIcon.color = deactivateColor;
                }
                break;
        }
    }

    public void TurnOnOffSimulate() 
    {
        simulated = !simulated;

        if (simulated)
        {
            simulateIndicator.color = simulateOn;
        } else
        {
            currentIntervalTime = selectedIntervalTime;
            simulateIndicator.color = simulateOff;
        }
    }


    void IntervalSpawn()
    {
        selectedIntervalTime = Random.Range(minInterval, maxInterval);
        // float currentSpeed = Random.Range(minSpeed, maxSpeed);
        float currentSpeed = minSpeed;
        int currentWeightChoice = Random.Range(minWeight, maxWeight);

        int spawnChoiceMax = spawnPointChoice.Count > 0 ? spawnPointChoice.Count : 0;
        int currentSpawnChoice = spawnPointChoice[Random.Range(0, spawnChoiceMax)];

        foreach (WeightRange range in carsWeightRange)
        {
            if (currentWeightChoice >= range.minWeight && currentWeightChoice <= range.maxWeight)
            {
                currentVehicle = range.vehicleObj;
                break;
            }
        }

        if (currentSpawnChoice == 2)
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
                    vehicleMotor.speed = currentSpeed;
                    vehicleMotor.weight = currentWeightChoice;
                }
            }
        }
        else
        {
            SaveSpawnVehicle saveSpawn = spawnPoints[currentSpawnChoice].GetComponent<SaveSpawnVehicle>();
            if (saveSpawn.saveSpawning)
            {
                GameObject vehicle = Instantiate(vehicles[currentVehicle],
                                        spawnPoints[currentSpawnChoice].transform.position,
                                        spawnPoints[currentSpawnChoice].transform.rotation) as GameObject;

                VehicleMotorStatic vehicleMotor = vehicle.GetComponent<VehicleMotorStatic>();
                vehicleMotor.enableMove = true;
                vehicleMotor.speed = currentSpeed;
                vehicleMotor.weight = currentWeightChoice;
            }
        }
    }
}
