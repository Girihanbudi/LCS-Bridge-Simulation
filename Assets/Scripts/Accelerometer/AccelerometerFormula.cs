using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AccelerometerFormula : MonoBehaviour
{
    public TextReader rGO;
    public float calculateFrame = 10;

    private List<float> rValues;
    private float r;    // Random Vibration value

    private List<GameObject> vehicles;
    private int currentRIndex = 0;
    private float distanceToOuter;

    // Start is called before the first frame update
    void Start()
    {
        vehicles = new List<GameObject>();
        rValues = rGO.GetComponent<TextReader>().floatValues;

        distanceToOuter = GetComponent<SphereCollider>().radius;
        distanceToOuter *= 10;
    }

    private void Update()
    {
        r = rValues[currentRIndex];
        currentRIndex++;
        if (currentRIndex > rValues.Count - 1)
        {
            currentRIndex = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Vehicle"))
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

    public (float r, List<float>) CalculateVibration()
    {        
        List<float> vibrationData = new List<float>();
        for(int i=0; i < calculateFrame; i++)
        {
            vehicles = vehicles.Where(item => item != null).ToList();
            if (vehicles.Count > 0)
            {
                int seletectedIndex = Random.Range(0, vehicles.Count - 1);
                
                if (vehicles[seletectedIndex] != null)
                {
                    float distance = Vector3.Distance(vehicles[seletectedIndex].transform.position, transform.position);
                    float w = vehicles[seletectedIndex].GetComponent<VehicleMotorStatic>().weight;

                    if (distance > distanceToOuter) distance = distanceToOuter;

                    float d = distance / distanceToOuter * 100;
                    float calculation = r * (1 + (d / 100)) * (1 + (w / 16000));

                    vibrationData.Add(calculation);
                }
                else
                {
                    vibrationData.Add(0);
                }
            }
            else
            {
                vibrationData.Add(0);
            }
        }
        return (r, vibrationData);
    }
}
