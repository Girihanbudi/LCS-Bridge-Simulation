using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMotorStatic : MonoBehaviour
{
    public Transform breakLimitPos;
    public Transform rearLimitPos;

    public float speed;
    public float breakValue;
    public float weight;
    public float maxDistanceBreak = 20.0f;

    [Space]
    public bool useLagging = false;
    public bool hitLaggingPoint = false;
    public float currentLagTime;
    public float randomLagTime = 3.0f;
    public float currentLagInterval;
    public float randomLagInterval = 5.0f; 

    const float distanceForceStop = 1.0f;

    private bool onGround = false;
    private Rigidbody rb;

    public float nearestDistance;

    [Space]
    public VehicleCollector vehiclesCollector;
    [Space]
    public bool enableMove;

    private float moveInterval = 1.5f;

    public float acceleration;
    public float accelerationMultiplier;
    private float currentAccelerationMultiplier;

    public List<GameObject> wheels;
    private float currentWheelRotation;

    private void Start()
    {
        accelerationMultiplier = 2;
        currentAccelerationMultiplier = accelerationMultiplier;
        rb = GetComponent<Rigidbody>();
        moveInterval = 0.0f;

        currentLagInterval = 0.0f;
        currentLagTime = 0.0f;
    }

    private void Update()
    {
        if(vehiclesCollector.vehicles.Count <= 0)
        {
            acceleration += Time.deltaTime * currentAccelerationMultiplier;

            if (useLagging && hitLaggingPoint)
                Lagging();
        } else
        {
            if(acceleration > 0)
            {
                currentAccelerationMultiplier += Time.deltaTime;
                acceleration -= Time.deltaTime * currentAccelerationMultiplier * 3;
            } else
            {
                currentAccelerationMultiplier = accelerationMultiplier;
            }
        }

        if (onGround && enableMove)
        {
            Move();
            Break();
        }
    }

    private void RotateWheel()
    {
        Vector3 targetRotation = new Vector3(currentWheelRotation, 0, 0);

        foreach(GameObject wheel in wheels)
        {
            wheel.transform.eulerAngles = targetRotation;
        }
    }

    private void Move()
    {
        float finalSpeed;
        if (useLagging) finalSpeed = speed + acceleration - breakValue;
        else finalSpeed = speed + acceleration - breakValue;
        rb.velocity = (transform.forward * (finalSpeed));
        currentWheelRotation += finalSpeed;

        RotateWheel();
    }

    private void Break()
    {
        if(vehiclesCollector.vehicles.Count > 0)
        {
            if(moveInterval > 0)
            {
                moveInterval -= Time.deltaTime;
            }
            else
            {
                float lowestDistance = maxDistanceBreak;
                for (int i = 0; i < vehiclesCollector.vehicles.Count; i++)
                {
                    if(vehiclesCollector.vehicles[i] != null)
                    {
                        VehicleMotorStatic otherMotor = vehiclesCollector.vehicles[i].GetComponent<VehicleMotorStatic>();
                        Vector2 otherPoint = new Vector2(otherMotor.rearLimitPos.position.x, otherMotor.rearLimitPos.position.z);
                        Vector2 breakPoint = new Vector2(breakLimitPos.position.x, breakLimitPos.position.z);
                        float distance = Vector2.Distance(breakPoint, otherPoint);

                        if (i == 0) lowestDistance = distance;
                        else if (distance < lowestDistance) lowestDistance = distance;
                    }
                    else
                    {
                        vehiclesCollector.vehicles.Remove(vehiclesCollector.vehicles[i]);
                    }
                }

                nearestDistance = lowestDistance;
                if (nearestDistance < distanceForceStop)
                {
                    breakValue = speed;
                    moveInterval = 1.5f;
                }
                else
                {
                    breakValue = (speed * 0.9f) - (nearestDistance / maxDistanceBreak * speed);
                }

                if (breakValue < 0) breakValue = 0;
            }


        } 
        // IF STILL USE BREAK
        else
        {
            breakValue = 0;
            nearestDistance = maxDistanceBreak;
        }
    }

    private void Lagging()
    {
        
        if(currentLagInterval < randomLagInterval)
        {
            currentLagInterval += Time.deltaTime;
            return;
        } else
        {
            //currentLagInterval = Random.Range(randomLagInterval - (randomLagInterval * 0.3f), randomLagInterval + (randomLagInterval * 0.3f));
            

            if (currentLagTime < randomLagTime)
            {
                currentLagTime += Time.deltaTime;
                if (enableMove) enableMove = false;
            }
            else
            {
                currentLagInterval = 0;
                currentLagTime = 0;
                enableMove = true;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }

        if (collision.gameObject.CompareTag("Vehicle"))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
