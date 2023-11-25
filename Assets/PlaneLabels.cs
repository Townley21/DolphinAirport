using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneLabels : MonoBehaviour
{
    [SerializeField]
    Transform myPlaneTransform;
    [SerializeField]
    RectTransform myRectTransform;
    Camera mainCamera;

    //Swap to references to positions
    Camera camera1;
    Camera camera2;
    Camera camera3;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(myPlaneTransform.position);
        myRectTransform.anchoredPosition = screenPosition;

        Camera activeCamera = null;
        bool foundCamera = false;
        while (!foundCamera)
        {
            activeCamera = (Camera) FindObjectOfType(typeof(Camera));
            if (activeCamera.isActiveAndEnabled)
            {
                foundCamera = true;
                mainCamera = activeCamera;
            }
        }

    }
}