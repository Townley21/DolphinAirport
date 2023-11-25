using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct Flight
{
    public GameObject plane;
    public string name;
    public taxiToTerminal goToTerminal;
    public Takeoff takeoff;
    public taxiToRunway goToRunway;
    public float nextBoardingTime;
    public int terminal;
    public Direction dir;
    public RunwayQueueSlot runwayQueueSlot;
}

public enum Direction
{
    inbound, outbound
}

public enum RunwayQueueSlot
{
    Q1, Q2, Full
}

public static class TimeControls
{
    private static float originalfixedDeltaTime;

    // Start is called before the first frame update
    public static void Initialize()
    {
        originalfixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    public static void AdjustTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = originalfixedDeltaTime * Time.timeScale;
        Debug.Log("Time scale changed to: " + newTimeScale);
    }
}


public class SimulationController : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;

    public SystemTime systemTime;
    public GameObject airplanePrefab;
    public UIManager uiManager;
    public FlightSchedule flightSchedule;

    bool runwayOccupied = false;
    bool atTerminal = false;
    public bool importedDataFlag = false;
    List<ScheduledFlight> loadedFlights;

    bool[] runwayQueueSlotAvail = new bool[] { true, true };
    bool[] terminalAvail = new bool[] { true, true, true, true };
    Queue<Flight> runwayQueue = new Queue<Flight>();

    float timeOverhead = 60.0f; //TO:DO MIN TRAVEL TIME TO FURTHEST TERMINAL
    float minSpawnInterval = 20.0f;
    float maxSpawnInterval = 25.0f;
    float nextSpawnTime;

    float minBoardingInterval = 25.0f;
    float maxBoardingInterval = 30.0f;
    float nextBoardingTime;


    List<Flight> taxiToTerminals = new List<Flight>();
    List<Flight> readyToDepart = new List<Flight>();
    List<Flight> taxiToRunway = new List<Flight>();
    List<Flight> readyForTakeoff = new List<Flight>();
    List<Flight> takeoffs = new List<Flight>();

    System.Random random = new System.Random();

    void Start()
    {
        //intialize random time for plane spawn
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = 0;
        TimeControls.Initialize();
        if (uiManager == null)
        {
            Debug.Log("UI MANAGER IS NULL");
        }

        if (systemTime == null)
        {
            Debug.Log("SYSTEM TIME IS NULL");
        }

        airplanePrefab = Resources.Load<GameObject>("Prefabs/Airplane");

        Debug.Log("Setup Complete.");
    }
 
    bool terminalIsAvail(int terminal)
    {
        return terminalAvail[terminal];
    }

    public void setDataImportedFlag(bool flag)
    {
        importedDataFlag = flag;
    }

    int findFirstAvailTerminal()
    {
        for (int i = 0; i < 4; i++)
        {
            if (terminalAvail[i])
                return i;
        }
        return -1;
    }

    bool anyTerminalIsOccupied()
    {
        for (int i = 0; i < terminalAvail.Length; i++)
        {
            if (!terminalAvail[i])
                return true;
        }
        return false;
    }

    Queue<Flight> sortRunwayQueue(Queue<Flight> unsortedQueue)
    {
        Queue<Flight> sortedQueue = new Queue<Flight>();
        List<Flight> outbound = new List<Flight>();
        List<Flight> inbound = new List<Flight>();
        while (unsortedQueue.Count != 0)
        {
            if (unsortedQueue.Peek().dir == Direction.outbound)
            {
                outbound.Add(unsortedQueue.Dequeue());
                uiManager.dequeueTile();
            }
            else
            {
                inbound.Add(unsortedQueue.Dequeue());
                uiManager.dequeueTile();
            }
        }

        foreach (Flight flight in outbound)
        {
            sortedQueue.Enqueue(flight);
            uiManager.enqueueTile(flight);
        }

        foreach(Flight flight in inbound)
        {
            sortedQueue.Enqueue(flight);
            uiManager.enqueueTile(flight);
        }

        return sortedQueue;
    }

    string GenerateRandomAlphanumeric(int length)
    {
        const string alphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numericChars = "0123456789";
        
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            if (i == 0)
            {
               int ranChar = random.Next(0, alphaChars.Length);
               result.Append(alphaChars[ranChar]);
            } else
            {
                int index = random.Next(0, numericChars.Length);
                result.Append(numericChars[index]);
            }
        }

        return result.ToString();
    }

    bool isRunwayClear()
    {
        foreach (Flight flight in taxiToTerminals)
        {
            if (flight.plane.GetComponent<taxiToTerminal>().getRunwayStatus())
            {
                Debug.Log("Landing Plane is occupying Runway");
                return true;
            }
        }

        foreach (Flight flight in takeoffs)
        {
            if (flight.plane.GetComponent<Takeoff>().getRunwayStatus())
            {
                Debug.Log("Departing Plane is occupying Runway");
                return true;
            }
        }
        Debug.Log("Runway Is Clear");
        return false;
    }

    RunwayQueueSlot getTaxiState()
    {
        if (runwayQueueSlotAvail[1])
        {
            return RunwayQueueSlot.Q2;
        }
        return RunwayQueueSlot.Full;
    }

    string PrintRunwayQueue()
    {
        string result = "Runway Queue: ";

        foreach (Flight flight in runwayQueue)
        {
            result += flight.name + " ";
        }
        return result;
    }

    void Update()
    {
        /**
         * 
         * USER IO
         * 
         */
        Debug.Log(PrintRunwayQueue());
        systemTime.UpdateSimClock();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TimeControls.AdjustTimeScale(1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TimeControls.AdjustTimeScale(2.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TimeControls.AdjustTimeScale(10.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TimeControls.AdjustTimeScale(30.0f);
        }

        if (Input.GetKeyDown("q"))
        {
            camera1.SetActive(true);
            camera2.SetActive(false);
            camera3.SetActive(false);
        }

        if (Input.GetKeyDown("w"))
        {
            camera1.SetActive(false);
            camera2.SetActive(true);
            camera3.SetActive(false);
        }
        
        if (Input.GetKeyDown("e"))
        {
            camera1.SetActive(false);
            camera2.SetActive(false);
            camera3.SetActive(true);
        }

        // DEBUGGING
        //if (takeoffs.Count < 1 && taxiToRunway.Count < 1 && taxiToTerminals.Count < 1 && readyToDepart.Count < 1)
        //    Debug.Log("No Scripts to Process");

        runwayQueue = sortRunwayQueue(runwayQueue);
        runwayOccupied = isRunwayClear();

        if (importedDataFlag)
        {
            loadedFlights = flightSchedule.GetLoadedFLights();
            importedDataFlag = false;
            systemTime.ResetSimulationTime();
            Debug.Log("Data Imported Fired At " + systemTime.GetSystemTime());
        }

        /**
         * ======= FLIGHT GENERATION =======
        */
        if (loadedFlights != null)
        {
            for (int i = 0; i < loadedFlights.Count; i++)
            {
                
                ScheduledFlight scheduledFlight = loadedFlights[i];
                
                if (i == 0)
                {
                    Debug.Log("Scheduled Flight: " + scheduledFlight.Name + "TimeIn: " + scheduledFlight.TimeIn + "SystemTime: " + systemTime.GetSystemTime());
                }
                if (systemTime.GetSystemTime() > scheduledFlight.TimeIn && !scheduledFlight.HasBeenQueued)
                {
                    Debug.Log("Enqueing scheduled Flight: " + scheduledFlight.Name);
                    Flight flight = new Flight();
                    flight.name = scheduledFlight.Name;
                    flight.dir = Direction.inbound;
                    flight.terminal = scheduledFlight.Terminal - 1;
                    flight.nextBoardingTime = scheduledFlight.TimeOut;

                    scheduledFlight.HasBeenQueued = true;
                    loadedFlights[i] = scheduledFlight;
                    runwayQueue.Enqueue(flight);
                    uiManager.enqueueTile(flight);
                }
            }
        }
        /**
         * ======= END FLIGHT GENERATION =======
        */




        /**
         * ======= HANDLE OPEN RUNWAY LOGIC =======
        */
        if (runwayOccupied == false && runwayQueue.Count > 0)
        {
            bool handled = false;
            
            if (runwayQueue.Peek().dir == Direction.outbound)
            {
                Debug.Log("HANDLING AN OUTBOUND FLIGHT");
                runwayOccupied = true;
                Flight outboundFlight = runwayQueue.Dequeue();
                takeoffs.Add(outboundFlight);
                readyForTakeoff.Remove(outboundFlight);
                uiManager.dequeueTile();
                handled = true;
                loadedFlights = flightSchedule.removeFlightFromSchedule(outboundFlight.name);
                Debug.Log("Flight: " + outboundFlight.name + " Cleared for takeoff");
            }

            //spawn incoming plane
             if (!handled && !runwayOccupied && runwayQueue.Peek().dir != Direction.outbound && terminalIsAvail(runwayQueue.Peek().terminal))
             {
                Flight spawningFlight = runwayQueue.Dequeue();
                Vector3 spawnPosition = new Vector3(0, 2.0f, -13);
                Quaternion spawnRotation = Quaternion.Euler(-3.5f, 0f, 0f);
                GameObject plane = Instantiate(airplanePrefab, spawnPosition, spawnRotation);

                plane.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(spawningFlight.name);
                taxiToTerminal goToTerminal = plane.AddComponent<taxiToTerminal>();
                taxiToRunway goToRunway = plane.AddComponent<taxiToRunway>();
                Takeoff takeoff = plane.AddComponent<Takeoff>();
                spawningFlight.plane = plane;
                spawningFlight.goToRunway = goToRunway;
                spawningFlight.takeoff = takeoff;

                int terminalSwitch = spawningFlight.terminal;
                if (terminalSwitch == 0)
                {
                    spawningFlight.terminal = 0;
                    goToTerminal.terminal_1_avail = true;
                    terminalAvail[0] = false;
                }
                else if (terminalSwitch == 1)
                {
                    spawningFlight.terminal = 1;
                    goToTerminal.terminal_2_avail = true;
                    terminalAvail[1] = false;
                }
                else if (terminalSwitch == 2)
                {
                    spawningFlight.terminal = 2;
                    goToTerminal.terminal_3_avail = true;
                    terminalAvail[2] = false;
                }
                else if (terminalSwitch == 3)
                {
                    spawningFlight.terminal = 3;
                    goToTerminal.terminal_4_avail = true;
                    terminalAvail[3] = false;
                }
                uiManager.dequeueTile();
                taxiToTerminals.Add(spawningFlight);
                runwayOccupied = true;
             }
        }
        /**
        * ======= END HANDLE OPEN RUNWAY LOGIC =======
        */

        /**
         * ======= Motion for runway -> terminal LOGIC =======
        */
        if (taxiToTerminals.Count > 0)
        {
            for (int i = 0; i < taxiToTerminals.Count; i++)
            {
                taxiToTerminals[i].plane.GetComponent<taxiToTerminal>().ExecuteMotion();
                
                if (taxiToTerminals[i].plane.GetComponent<taxiToTerminal>().atTerminal)
                {
                    readyToDepart.Add(taxiToTerminals[i]);
                    taxiToTerminals.Remove(taxiToTerminals[i]);
                    atTerminal = true;
                }
            }
        }
            /**
            * ======= END Motion for runway -> terminal LOGIC =======
            */

        if(readyToDepart.Count > 0)
        {
            for (int i = 0; i < readyToDepart.Count; i++)
            {
                if (systemTime.GetSystemTime() > readyToDepart[i].nextBoardingTime && getTaxiState() != RunwayQueueSlot.Full)
                {
                    Flight departingFlight = readyToDepart[i];
                    terminalAvail[readyToDepart[i].terminal] = true;
                    readyToDepart.Remove(readyToDepart[i]);

                    departingFlight.runwayQueueSlot = RunwayQueueSlot.Q2;
                    departingFlight.plane.GetComponent<taxiToRunway>().setQueueSlotTarget(departingFlight.runwayQueueSlot);
                    runwayQueueSlotAvail[1] = false;

                    taxiToRunway.Add(departingFlight);
                }
            }
        }



            //add to readyToDepart
        if(taxiToRunway.Count > 0)
        {
            for(int i = 0; i < taxiToRunway.Count; i++)
            {
                if (taxiToRunway[i].runwayQueueSlot == RunwayQueueSlot.Q2 && runwayQueueSlotAvail[0] && taxiToRunway[i].plane.GetComponent<taxiToRunway>().getQ2())
                {
                    Flight flight = taxiToRunway[i];
                    flight.plane.GetComponent<taxiToRunway>().setQueueSlotTarget(RunwayQueueSlot.Q1);
                    runwayQueueSlotAvail[0] = false;
                    runwayQueueSlotAvail[1] = true;
                    flight.runwayQueueSlot = RunwayQueueSlot.Q1;
                }

                Debug.Log("Flight: " + taxiToRunway[i].name + "Ready For Takeoff: " + taxiToRunway[i].plane.GetComponent<taxiToRunway>().getReadyForTakeoffStatus());
                taxiToRunway[i].plane.GetComponent<taxiToRunway>().ExecuteMotion();
                if(taxiToRunway[i].plane.GetComponent<taxiToRunway>().getReadyForTakeoffStatus())
                {
                    Debug.Log("Flight: " + taxiToRunway[i].name +" is marked ready for take off");
                    Flight arrivedFlight = taxiToRunway[i];
                    arrivedFlight.dir = Direction.outbound;
                    runwayQueue.Enqueue(arrivedFlight);
                    uiManager.enqueueTile(arrivedFlight);
                    readyForTakeoff.Add(arrivedFlight);
                    taxiToRunway.Remove(taxiToRunway[i]);

                }
            }
        }
            
        //Takeoffs -> destruction
        if (takeoffs.Count > 0)
        {
            for(int i = 0; i < takeoffs.Count; i++)
            {
                runwayQueueSlotAvail[0] = true;
                
                takeoffs[i].plane.GetComponent<Takeoff>().ExecuteMotion();
                
                if (takeoffs[i].plane.GetComponent<Takeoff>().outOfScene)
                {
                    loadedFlights = flightSchedule.removeFlightFromSchedule(takeoffs[i].name);
                    Debug.Log("Despawning Deperature");
                    Destroy(takeoffs[i].plane);
                    takeoffs.Remove(takeoffs[i]);
                }      
            }
                
        }
        
    }
}