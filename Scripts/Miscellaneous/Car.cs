using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform carPosition;
   
    public WheelCollider frontRight;
    public WheelCollider frontLeft;
    public WheelCollider backRight;
    public WheelCollider backLeft;

    public Transform frontRightTransform;
    public Transform frontLeftTransform;
    public Transform backRightTransform;
    public Transform backLeftTransform;


    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;

    private float currAcceleration = 0f;
    private float currBreakForce = 0f;
    private float currTurnAngle = 0f;
    

    private void FixedUpdate(){

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currAcceleration = acceleration * 1.5f * Input.GetAxis("Vertical");
        }
        else
            currAcceleration = acceleration * Input.GetAxis("Vertical");
        {

        }

        //print(this.carPosition.rotation.eulerAngles);
        print(currAcceleration);
            
        if (Input.GetKey(KeyCode.Space)){
            currBreakForce = breakingForce;
            print(currBreakForce);
        }
        else{
            currBreakForce = 0;
        }

        if (Input.GetKey(KeyCode.V))
        {
            FixCarRotation();
        }

        frontRight.motorTorque = currAcceleration;
        frontLeft.motorTorque = currAcceleration;

        frontRight.brakeTorque = currBreakForce;
        frontLeft.brakeTorque = currBreakForce;
        backRight.brakeTorque = currBreakForce;
        backLeft.brakeTorque = currBreakForce;
        
        currTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currTurnAngle;
        frontRight.steerAngle = currTurnAngle;

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);
    }

    void UpdateWheel(WheelCollider wc, Transform t){
        Vector3 pos;
        Quaternion rot;

        wc.GetWorldPose(out pos, out rot);

        t.position = pos;
        t.rotation = rot;
        
    }

    void FixCarRotation()
    {
        Vector3 angles = carPosition.eulerAngles;
        angles.z = 0f;
        carPosition.eulerAngles = angles;
    }

}
