using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlightSchedule : MonoBehaviour
{
    public GameObject flightTilePrefab;
    public Transform gridContainer;
    public List<ScheduledFlight> originalList = new List<ScheduledFlight>();
    public List<ScheduledFlight> loadedFlights = new List<ScheduledFlight>();
    public Dictionary<string, GameObject> loadedTiles = new Dictionary<string, GameObject>();
    private int indexCount;
    public SimulationController simController;

    //string time, string city, string flightId, string terminal
    public void addTile(int timeIn, int timeOut, string city, string flightId, int terminal)
    {
        GameObject newFlight = Instantiate(flightTilePrefab, gridContainer);

        TextMeshProUGUI timeInText = newFlight.transform.Find("TimeIn").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timeOutText = newFlight.transform.Find("TimeOut").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI cityText = newFlight.transform.Find("cityLabel").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI flightIdText = newFlight.transform.Find("flightIdLabel").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI terminalText = newFlight.transform.Find("terminalLabel").GetComponent<TextMeshProUGUI>();

        
        

        timeInText.text = UnityTimeToClockTime(timeIn);
        timeOutText.text = UnityTimeToClockTime(timeOut);
        cityText.text = city;
        flightIdText.text = flightId;
        terminalText.text = terminal.ToString();
        newFlight.SetActive(true);
        loadedTiles.Add(flightId, newFlight);
    }

    private void RemoveTile(string flightId)
    {
        if (loadedTiles.ContainsKey(flightId))
        {
            Destroy(loadedTiles[flightId]);
            loadedTiles.Remove(flightId);
        }
    }

    public List<ScheduledFlight> GetLoadedFLights()
    {
        return loadedFlights;
    }

    public void NewDataSet(List<ScheduledFlight> newData)
    {
        originalList = newData;
        loadIntialTiles();
        simController.setDataImportedFlag(true);
    }

    public string UnityTimeToClockTime(int time)
    {
        int hours = Mathf.FloorToInt(time / 3600) % 24;
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        return formattedTime;
    }

    private void loadIntialTiles()
    {
        int counter = 0;
        indexCount = 0;
        if (loadedFlights != null)
        {
            loadedFlights.Clear();
        }
        
        if (loadedTiles != null)
        {
            loadedTiles.Clear();
        }
        

        foreach (ScheduledFlight flight in originalList)
        {
            if (counter == 40)
            {
                indexCount = counter;
                return;
            }
            loadedFlights.Add(flight);
            addTile(flight.TimeIn, flight.TimeOut, flight.City, flight.Name, flight.Terminal);
            counter++;
        }
    }

    public List<ScheduledFlight> removeFlightFromSchedule(string name)
    {
        foreach (ScheduledFlight flight in loadedFlights)
        {
            if (flight.Name.Equals(name))
            {
                loadedFlights.Remove(flight);
            }
        }
        RemoveTile(name);

        ScheduledFlight newFlight = originalList[indexCount];
        indexCount++;

        addTile(newFlight.TimeIn, newFlight.TimeOut, newFlight.City, newFlight.Name, newFlight.Terminal);
        loadedFlights.Add(originalList[indexCount]);
        return GetLoadedFLights();
    }
}
