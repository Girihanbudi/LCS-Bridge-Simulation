using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public float multiplier = 0.25f;
    public float minTimeSpeed = 0.0f, maxTimeSpeed = 2.0f;

    public void DecreaseTimeSpeed()
    {
        if (Time.timeScale > minTimeSpeed) Time.timeScale -= multiplier;
        if (Time.timeScale < minTimeSpeed) Time.timeScale = minTimeSpeed;
    }

    public void IncreaseTimeSpeed()
    {
        if (Time.timeScale < maxTimeSpeed) Time.timeScale += multiplier;
        if(Time.timeScale > maxTimeSpeed) Time.timeScale = maxTimeSpeed;
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
    }

    public void Play()
    {
        Time.timeScale = 1.0f;
    }
}
