using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas canvas;
    public GameObject queueTilePrefab;
    public Transform queueParent;
    public List<GameObject> tileList = new List<GameObject>();

    void Start()
    {
        // Get the reference to the canvas and the parent transform
        canvas = GameObject.Find("HUD").GetComponent<Canvas>();
        queueParent = canvas.transform.Find("queueParent"); // Create an empty GameObject in the Canvas to serve as the parent for tiles

        if (canvas == null)
            Debug.Log("canvas is null");
        else if (queueParent == null)
            Debug.Log("queueParent is null");
    }

    void setTileInfo(GameObject queueTile, Flight flight)
    {
        Text flightNumText = queueTile.transform.Find("FlightNum").GetComponent<Text>();
        Text directionText = queueTile.transform.Find("InOut").GetComponent<Text>();

        if (flight.dir == Direction.inbound)
        {
            Image planeIcon = queueTile.transform.Find("Icon").GetComponent<Image>();
            planeIcon.transform.Rotate(0, 0, -70);
        }

        flightNumText.text = flight.name;
        directionText.text = flight.dir.ToString();
    }

    public void enqueueTile(Flight flight)
    {
        //Debug.Log("SPAWNING TILE IN GUI...");

        GameObject queueTile = Instantiate(queueTilePrefab, queueParent);
        tileList.Add(queueTile);

        setTileInfo(queueTile, flight);

        HorizontalLayoutGroup layoutGroup = queueParent.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = false; // Disable child width control to center-align
        }

        // Calculate the total width of all tiles, including spacing
        float totalWidth = 0f;
        float spacing = 10f; // Adjust this value as needed to control the horizontal spacing

        foreach (GameObject tile in tileList)
        {
            totalWidth += tile.GetComponent<RectTransform>().rect.width + spacing;
        }

        // Calculate the horizontal offset to center-align the tiles
        float xOffset = totalWidth / 2f;

        // Set the position for each tile based on its index and the xOffset
        for (int i = 0; i < tileList.Count; i++)
        {
            RectTransform tileRectTransform = tileList[i].GetComponent<RectTransform>();
            Vector2 tilePosition = tileRectTransform.anchoredPosition;
            tilePosition.x = (i * (tileRectTransform.rect.width + spacing)) - xOffset;
            tileRectTransform.anchoredPosition = tilePosition;
        }

    }

    public void dequeueTile()
    {
        if (tileList.Count == 0)
        {
            Debug.Log("No tiles to dequeue.");
            return;
        }

        // Remove the first tile from the list
        GameObject firstTile = tileList[0];
        tileList.RemoveAt(0);

        // Destroy the GameObject associated with the dequeued tile
        Destroy(firstTile);

        // Calculate the total width of all remaining tiles, including spacing
        float totalWidth = 0f;
        float spacing = 10f; // Adjust this value as needed to control the horizontal spacing

        foreach (GameObject tile in tileList)
        {
            totalWidth += tile.GetComponent<RectTransform>().rect.width + spacing;
        }

        // Calculate the horizontal offset to center-align the remaining tiles
        float xOffset = totalWidth / 2f;

        // Set the position for each remaining tile based on its index and the xOffset
        for (int i = 0; i < tileList.Count; i++)
        {
            RectTransform tileRectTransform = tileList[i].GetComponent<RectTransform>();
            Vector2 tilePosition = tileRectTransform.anchoredPosition;
            tilePosition.x = (i * (tileRectTransform.rect.width + spacing)) - xOffset;
            tileRectTransform.anchoredPosition = tilePosition;
        }
    }

}
