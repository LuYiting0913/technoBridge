using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour {
    public WheelCollider frontLeft, frontRight;
    public WheelCollider rearLeft, rearRight;
    public Transform frontLeftWheel, frontRightWheel;
    public Transform rearLeftWheel, rearRightWheel;
    public float motor = 300;
    public float brake = 10000;
    public Checkpoint checkpoint;

    private void Accelerate() {
        rearLeft.motorTorque = motor;
        rearRight.motorTorque = motor;
        frontLeft.motorTorque = motor;
        frontRight.motorTorque = motor;
    }

    private void Brake() {

        rearLeft.motorTorque = 1;
        rearRight.motorTorque = 1;
        frontLeft.motorTorque = 1;
        frontRight.motorTorque = 1;
        rearLeft.brakeTorque = brake;
        rearRight.brakeTorque = brake;
        frontLeft.brakeTorque = brake;
        frontRight.brakeTorque = brake;
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
        if (checkpoint.Arrived(transform.position)) {
            Debug.Log("arrived");
            Brake();
        } else {
            Accelerate();
            UpdateWheels();
        }

    }

    public void SetCheckpoint(Checkpoint p) {
        checkpoint = p;
    }
}
