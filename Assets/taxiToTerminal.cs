using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class taxiToTerminal : MonoBehaviour
{
    public bool occupyingRunway = false; //to know if runway is clear for sim
    private bool hasExecuted = false;
    private bool isLanding = true;
    private float startTime;
    float targetSpeed = 1.25f;
    float intialSpeed = 4.5f;
    float speed;
    float turnSpeed = 20f;
    float maxTurnAngle = 90f;
    float currentTurnAngle = 0f;
    float currentTurnAnglePositiveY = 0f;
    float maxTurnAnglePositiveY = 90f;
    bool isTurning = false;
    public bool atTerminal = false;
    //float[3] waypoint = [-10f, -12f, -14f];

    //TERMINAL 1
    public bool terminal_1_avail = false;
    //TERMINAL 2
    public bool terminal_2_avail = false;
    //TERMINAL 3
    public bool terminal_3_avail = false;
    //TERMINAL 4
    public bool terminal_4_avail = false;
    // Start is called before the first frame update
    float speedFactor = 1.0f;
    float speedDelta;
    public bool getRunwayStatus()
    {
        return occupyingRunway;
    }

    // Used to be FixedUpdate, now it is ExecuteMotion
    public void ExecuteMotion()
    {
        speedDelta = Time.deltaTime * speedFactor;
        //Debug.Log("PLANE SPEED: " + speed);
        /* POSITIONS OF WHEN TO PERFORM TURNS
         */
        if (!hasExecuted)
        {
            startTime = Time.time;
            occupyingRunway = true;
            hasExecuted = true;
            speed = intialSpeed;
        }

        if (transform.position.z > 34.5 && transform.position.x == 0 && transform.position.y == 0)
        {
            isTurning = true;
            occupyingRunway = false;
            
        }
        else if (transform.position.x <= -4.5 && transform.position.z >= 38 && transform.position.y == 0)
        {
            isTurning = true;
            maxTurnAngle = 180f;
            
        }
        else if (( transform.position.x <= -8 && transform.position.z <= 34.9 && transform.position.y == 0 ) &&
                   terminal_1_avail)
        {
            isTurning = true;
            turnSpeed = -turnSpeed; //banking in positive direction need positive speed.
        }
        else if ((transform.position.x <= -8 && transform.position.z <= 29.9 && transform.position.y == 0) &&
                   terminal_2_avail)
        {
            isTurning = true;
            turnSpeed = -turnSpeed; //banking in positive direction need positive speed.
        }
        else if ((transform.position.x <= -8 && transform.position.z <= 24.9 && transform.position.y == 0) &&
                   terminal_3_avail)
        {
            isTurning = true;
            turnSpeed = -turnSpeed; //banking in positive direction need positive speed.
        }
        else if ((transform.position.x <= -8 && transform.position.z <= 19.9 && transform.position.y == 0) &&
                   terminal_4_avail)
        {
            isTurning = true;
            turnSpeed = -turnSpeed; //banking in positive direction need positive speed.
        }

        if (isLanding)
        {
            float descentRate = 0.45f; // Adjust the descent rate as needed.
            transform.Translate(Vector3.down * descentRate * speedDelta);

            float rotationSpeed = 0.5f; // Adjust the rotation speed as needed.
            float targetXRotation = 0.0f;

            // Calculate the rotation angle towards the target.
            float rotationAngle = rotationSpeed * speedDelta;

            if (Mathf.Abs(transform.rotation.eulerAngles.x - targetXRotation) < rotationAngle)
            {
                // If the rotation angle is close to the target, set it to the target value.
                transform.rotation = Quaternion.Euler(targetXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
            else
            {
                // Rotate across the X-axis.
                transform.Rotate(Vector3.right, rotationAngle);
            }

            float speedReductionFactor = 0.99f; // Adjust the speed reduction factor as needed.
            float speedReductionDelay = 600.0f; // Adjust the delay for the speed reduction curve.

            
            if (transform.position.y < 0.001)
            {
                // Ensure the Y position is exactly 0
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

                float timeElapsed = Time.time - startTime;
                float speedFactor = Mathf.Clamp01((timeElapsed / speedReductionDelay));
                speed = Mathf.Lerp(speed, targetSpeed, Mathf.Pow(speedFactor, speedReductionFactor));
            }
        }

        if (speed == 1.25)
        {
            isLanding = false;
        }



        //Debug.Log("isTurning: " + isTurning);

        if (!isTurning)
        {
            //Debug.Log("We should be going forward if you see me."); //NEW CURRENT SPEED FACTOR
            transform.Translate(Vector3.forward * speedDelta * speed);
        }
        else
        {
            float angleToRotate = -turnSpeed * speedDelta;
           // Debug.Log("angleToRotate: " + angleToRotate);

            if (angleToRotate < 0) //Banking turn in Negative Y rotation
            {
                
                if (currentTurnAngle + angleToRotate <= -maxTurnAngle)
                {
                    // clamp rotation angle to maxTurnAngle
                    angleToRotate = -maxTurnAngle - currentTurnAngle;
                    isTurning = false;
                }
                currentTurnAngle += angleToRotate;
            }
            else //Banking turn in positive Y rotation
            {
                // Banked turn to right
                if (currentTurnAnglePositiveY + angleToRotate >= maxTurnAnglePositiveY)
                {
                    angleToRotate = maxTurnAnglePositiveY - currentTurnAnglePositiveY;
                    isTurning = false;
                }
                currentTurnAnglePositiveY += angleToRotate;
            }

            //Check if Plane is at terminal
            //Debug.Log(atTerminal);
            if (transform.position.x <= -16)
                atTerminal = true;

            //Debug.Log("AtTerminal inScript: " + atTerminal);
            if (!atTerminal)
            {
                transform.Rotate(Vector3.up, angleToRotate);
                transform.Translate(Vector3.forward * speedDelta * speed);

                //Debug
                
                  //Debug.Log("maxTurnAngle: " + currentTurnAngle);
                 // Debug.Log("currentTurnAngle: " + currentTurnAngle);
                  //Debug.Log("currentTurnAngleY: " + currentTurnAnglePositiveY);
          
                  //Debug.Log("angleToRotate: " + angleToRotate);
                 
            }

        }
    }
}