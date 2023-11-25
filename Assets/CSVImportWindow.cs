using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using System;

public struct ScheduledFlight
{
    public int TimeIn { get; set; }
    public int TimeOut { get; set; }
    public string City { get; set; }
    public string Name { get; set; }
    public int Terminal { get; set; }
    public bool HasBeenQueued { get; set; }
}

[RequireComponent(typeof(Button))]
public class CSVImportWindow : MonoBehaviour, IPointerDownHandler
{
    FlightSchedule fs;


    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        GameObject godPlane = GameObject.Find("GOD PLANE");
        fs = godPlane.GetComponent<FlightSchedule>();
        if (fs == null)
            Debug.Log("FS IS NULLL");
    }

    private void OnClick()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "csv", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url)
    {
        var loader = new WWW(url);
        yield return loader;
        ParseCSVFile(loader.text);
    }

    private List<ScheduledFlight> ParseCSVFile(string csv)
    {
        List<ScheduledFlight> flights = new List<ScheduledFlight>();
        string[] lines = csv.Split('\n');
        foreach (string line in lines)
        {
            string[] fields = line.Split(',');

            if (fields.Length >= 4)
            {
                try
                {
                    ScheduledFlight flight = new ScheduledFlight
                    {
                        TimeIn = int.Parse(fields[0]),
                        TimeOut = int.Parse(fields[1]),
                        City = fields[2],
                        Name = fields[3],
                        Terminal = int.Parse(fields[4]),
                        HasBeenQueued = false
                    };

                    // Add the flight to the list
                    flights.Add(flight);
                }
                catch (Exception e)
                {
                    // Handle parsing errors (e.g., invalid integer)
                    Debug.LogError($"Error parsing line: {line}. {e.Message}");
                }
            }
            else
            {
                // Handle lines with insufficient fields
                Debug.LogWarning($"Skipped line with insufficient fields: {line}");
            }
        }
        Debug.Log(flights);
        fs.NewDataSet(flights);
        return flights;
    }
}
