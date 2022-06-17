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
    public List<Checkpoint> checkpoints;
    private int checkpointCount, nextCheckpoint;
    private bool arrived, failed, waitingForHydraulic;
    private int duration = 2;

    public void Start() {
        checkpointCount = checkpoints.Count;
        Debug.Log(checkpointCount);
        nextCheckpoint = 0;
        waitingForHydraulic = false;
    }

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
        if (nextCheckpoint >= checkpointCount) {
            arrived = true;
        } else if (!waitingForHydraulic) {
            // Debug.Log("moving");
            if (ArrivedAtCheckpoint(checkpoints[nextCheckpoint]) && nextCheckpoint < checkpointCount) {
                Debug.Log("arrived at next");
                
                StartCoroutine(WaitForAWhile(duration));
                nextCheckpoint += 1;
            } else {
                Accelerate();
                UpdateWheels();
            }
        }
    }

    public bool Arrived() {
        return arrived;
    }

    // To detect if the car fall onto the water
    public bool Failed() {
        return failed;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "Water") {
            failed = true;
        }
    }

    public bool ArrivedAtCheckpoint(Checkpoint pt) {
        return pt.Arrived(transform.position);
    }

    private IEnumerator WaitForAWhile(int dur) {
        waitingForHydraulic = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(dur);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        waitingForHydraulic = false;
    }

    public bool IsWaiting() {
        return waitingForHydraulic;
    }
}
