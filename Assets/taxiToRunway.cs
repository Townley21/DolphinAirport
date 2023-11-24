using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taxiToRunway : MonoBehaviour
{
    public bool occupyingTaxiPath = true;
    public bool readyForTakeoff = false;
    float intialSpeed = 15f;
    float speed = 1.25f;
    float turnSpeed = 20f;
    float maxTurnAngle = 90f;
    float currentTurnAngle = 0f;
    float currentTurnAnglePositiveY = 0f;
    float maxTurnAnglePositiveY = 90f;
    bool isTurning = false;
    bool continueMoving = true;
    float angleToRotate = 0;
    bool hasReversed = false;
    public bool atQ2 = false;
    public bool atRunway = false;

    RunwayQueueSlot queueSlotTarget;

    // Start is called before the first frame update
    public bool getReadyForTakeoffStatus()
    {
        return readyForTakeoff;
    }

    public bool getQ2()
    {
        return atQ2;
    }

    public void setQueueSlotTarget(RunwayQueueSlot queueSlotTarget)
    {
        this.queueSlotTarget = queueSlotTarget;
    }

    // Update is called once per frame
    public void ExecuteMotion()
    {
        //terminal 1
        if (transform.position.z > 27 && transform.position.x > -15.5 && transform.position.y == 0)
        {
            isTurning = true;
        }

        //terminal 2
        if (transform.position.z > 22 && transform.position.x > -15.5 && transform.position.y == 0)
        {
            isTurning = true;
        }

        //terminal 3
        if (transform.position.z > 17 && transform.position.x > -15.5 && transform.position.y == 0)
        {
            isTurning = true;
        }

        //terminal 4
        if (transform.position.z > 12 && transform.position.x > -15.5 && transform.position.y == 0)
        {
            isTurning = true;
        }


        if (transform.position.x < -10 && transform.position.z < 5.8 && transform.position.y == 0)
        {
            isTurning = true;
            maxTurnAngle = 180f;
        }

        //STOP HERE FIRST
        if (transform.position.z < 2.5 && transform.position.x > -9.79 && transform.position.y == 0 && queueSlotTarget == RunwayQueueSlot.Q2)
        {
            atQ2 = true;
        }

        //HEAD OF RUNWAY QUEUE
        if (transform.position.z > 2 && transform.position.x > -5 && transform.position.y == 0)
        {
            atRunway = true;
            readyForTakeoff = true;
        }

        Debug.Log("isTurning: " + isTurning);

        if (isTurning)
            angleToRotate = -turnSpeed * Time.deltaTime;

        if (angleToRotate < 0) //Banking turn in Negative Y rotation
        {
            if (currentTurnAngle + angleToRotate <= -maxTurnAngle)
            {
                // clamp rotation angle to maxTurnAngle
                angleToRotate = -maxTurnAngle - currentTurnAngle;
                isTurning = false;
                hasReversed = true;
            }
            currentTurnAngle += angleToRotate;
        }

        //Debug.Log("atRunway in Script: " + atRunway);
        transform.Rotate(Vector3.up, angleToRotate);
        if (!hasReversed && continueMoving)
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        } else if (!atQ2 && queueSlotTarget == RunwayQueueSlot.Q2)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        else if (!atRunway && queueSlotTarget == RunwayQueueSlot.Q1)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }


    }
}
