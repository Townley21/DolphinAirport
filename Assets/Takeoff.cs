using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Takeoff : MonoBehaviour
{
    // Start is called before the first frame update
    public bool occupyingRunway = true; //to know if runway is clear for sim
    public bool outOfScene = false;
    bool takingOff = false;
    bool executeLift = false;
    bool increaseAltitude = false;
    float angleLift;
    float targetAngleLift = 0.5f;
    float speed = 1.25f;
    float turnSpeed = 20f;
    float LiftSpeed = 0.05f;
    float maxTurnAngle = 90f;
    float takeOffAngle = 5f;
    float currentTurnAngle = 0f;
    float currentTurnAnglePositiveY = 0f;
    float maxTurnAnglePositiveY = 90f;
    bool isTurning = false;
    public bool atTerminal = false;
    float angleToRotate;
    public float acceleration = 1.0f;
    public float takeoffSpeed = 50.0f;
    private float currentSpeed = 1.25f;
    private float timeToTakeoff = 5.0f; // time it takes to reach takeoff speed
    private float elapsedTime = 0.0f;

    public bool getOutOfSceneStatus()
    {
        return outOfScene;
    }

    public bool getRunwayStatus()
    {
        return occupyingRunway;
    }
    // Update is called once per frame
    public void ExecuteMotion()
    {
        if (transform.position.z > 2 && transform.position.x > -3.5 && transform.position.y == 0)
        {
            isTurning = true;
        }

        if (transform.position.y > 4.5)
        {
            outOfScene = true;
        }
        
        // Debug.Log("angleToRotate: " + angleToRotate);

        if (isTurning)
        {
            angleToRotate = -turnSpeed * Time.deltaTime;
            if (angleToRotate < 0) //Banking turn in Negative Y rotation
            {

                if (currentTurnAngle + angleToRotate <= -maxTurnAngle)
                {
                    // clamp rotation angle to maxTurnAngle
                    angleToRotate = -maxTurnAngle - currentTurnAngle;
                    isTurning = false;
                    takingOff = true;
                }

                currentTurnAngle += angleToRotate;
            }
        }
        
        if (isTurning)
            transform.Rotate(Vector3.up, angleToRotate);

        // Accelerate the plane's speed over time
        if (takingOff && elapsedTime < timeToTakeoff)
        {
            currentSpeed += acceleration * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            speed = currentSpeed;
        }

        if(takingOff)
        {
            if(transform.position.x > 0 && transform.position. z > 35 && transform.position.y == 0)
            {
                executeLift = true;
                increaseAltitude = true;
            }
        }


        if (executeLift)
        {
            angleLift = -LiftSpeed * Time.deltaTime;
            if (angleLift < 0) //Banking turn in Negative Y rotation
            {

                if (angleLift + targetAngleLift <= -targetAngleLift)
                {
                    // clamp rotation angle to maxTurnAngle
                    angleToRotate = -maxTurnAngle - currentTurnAngle;
                    executeLift = false;
                }

                currentTurnAngle += angleToRotate;
            }

            transform.Rotate(-Vector3.right * angleLift);
            
        }

        if (increaseAltitude)
            transform.Translate(Vector3.up * 0.05f); //mega magic number, I got tired of writing only letters 0.0
        
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
