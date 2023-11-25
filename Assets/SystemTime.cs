using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemTime : MonoBehaviour
{
    private float totalElapsedTime;
    private float startTime;
    private float simulationSpeed = 1.0f;
    public GameObject clock;
    public Text clockText;

    // Start is called before the first frame update
    public void Start()
    {
        if (clock == null)
        {
            Debug.Log("clock is null");
        }

        if (clockText == null)
        {
            Debug.Log("ClockText is null");
        }

        startTime = 8 * 3600;
        totalElapsedTime = 0f; // Initialize totalElapsedTime
    }

    // Update is called once per frame
    public void UpdateSimClock()
    {
        // Use Time.deltaTime to get the time passed since the last frame
        totalElapsedTime += Time.deltaTime * simulationSpeed;

        // Update the current time by adding the elapsed time
        float currentTime = startTime + totalElapsedTime;

        // Format the time as HH:mm:ss
        int hours = Mathf.FloorToInt(currentTime / 3600) % 24;
        int minutes = Mathf.FloorToInt((currentTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);

        clockText.text = formattedTime;
    }

    public void SetSimulationSpeed(float speed)
    {
        simulationSpeed = speed;
    }

    public void ResetSimulationTime()
    {
        totalElapsedTime = 0f;
        UpdateSimClock();
    }
    public float GetSystemTime()
    {
        return startTime + totalElapsedTime;
    }
}
