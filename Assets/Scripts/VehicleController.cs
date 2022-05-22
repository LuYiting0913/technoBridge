using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour {
    public WheelCollider frontLeft, frontRight;
    public WheelCollider rearLeft, rearRight;
    public Transform frontLeftWheel, frontRightWheel;
    public Transform rearLeftWheel, rearRightWheel;
    public float motor = 300;

    private void Accelerate() {
        rearLeft.motorTorque = motor;
        rearRight.motorTorque = motor;
        frontLeft.motorTorque = motor;
        frontRight.motorTorque = motor;
    }

    private void UpdateWheels() {
        UpdateWheel(frontLeft, frontLeftWheel);
        UpdateWheel(rearLeft, rearLeftWheel);
        UpdateWheel(frontRight, frontRightWheel);
        UpdateWheel(rearRight, rearRightWheel);
    }

    private void UpdateWheel(WheelCollider collider, Transform transform) {
        Vector3 position = transform.position;
        Quaternion quat = transform.rotation;
        quat = quat * Quaternion.Euler(new Vector3(0, 90, 0));

        collider.GetWorldPose(out position, out quat);

        transform.position = position;
        transform.rotation = quat;
    }

    public void FixedUpdate() {
        Accelerate();
        UpdateWheels();
    }
}
