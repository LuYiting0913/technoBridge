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
    public bool arrived, failed, waitingForHydraulic;
    private int duration = 10;
    // public float boatSpeed = 0.5f;

    public void Start() {
        checkpointCount = checkpoints.Count;
        // Debug.Log(checkpointCount);
        nextCheckpoint = 0;
        waitingForHydraulic = false;
        // Restart();
    }

    public void InitVehicleStatus() {
        nextCheckpoint = 0; 
        waitingForHydraulic = false;
        // Accelerate();
        arrived = false;
    }

    private void Accelerate() {
        rearLeft.motorTorque = motor;
        rearRight.motorTorque = motor;
        frontLeft.motorTorque = motor;
        frontRight.motorTorque = motor;
        rearLeft.brakeTorque = 0;
        rearRight.brakeTorque = 0;
        frontLeft.brakeTorque = 0;
        frontRight.brakeTorque = 0;
    }

    private void Brake() {
        // rearLeft.motorTorque = 1;
        // rearRight.motorTorque = 1;
        // frontLeft.motorTorque = 1;
        // frontRight.motorTorque = 1;
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
        // Debug.Log(gameObject.name);
        if (nextCheckpoint >= checkpointCount) {
            arrived = true;
	    } else if (!waitingForHydraulic) {
            // Debug.Log("moving");
            if (ArrivedAtCheckpoint(checkpoints[nextCheckpoint]) && nextCheckpoint < checkpointCount) {
                Debug.Log("arrived at next");
                Stop();
                nextCheckpoint += 1;
            } else {
                // if (gameObject.name == "Boat") {
                //     gameObject.transform.position -= new Vector3(0, 0, boatSpeed);
                // } else {
                    // Debug.Log("accing"); 
                Accelerate();
                UpdateWheels();
                // }
            }
        }
    }

    public void OnRestarted(object source, Stage2Controller e) {
        // Debug.Log("restarted");
        Restart();
        Debug.Log("car sound played");
        // e.GetAudio().PlayCarSound();
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
        if (other.gameObject.name == "Water" && gameObject.name != "Boat") {
            failed = true;
        }
    }

    public bool ArrivedAtCheckpoint(Checkpoint pt) {
		bool arrivedResult = pt.Arrived(transform.position);
		if (arrivedResult) {
			pt.TurnOffVolume();
		}
        return arrivedResult;
    }

    public void Stop() {
        waitingForHydraulic = true;
        Brake();
        UpdateWheels();
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        GameObject.Find("AudioManager").GetComponent<AudioManager>().StopCarSound();

    }

    public void Restart() {
        // Accelerate();
        // UpdateWheels();
        waitingForHydraulic = false;
        // gameObject.GetComponent<Rigidbody>().isKinematic = false;
        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayCarSound();
    }

    // private IEnumerator WaitForAWhile(int dur) {
    //     waitingForHydraulic = true;
    //     gameObject.GetComponent<Rigidbody>().isKinematic = true;
    //     yield return new WaitForSeconds(dur);
    //     gameObject.GetComponent<Rigidbody>().isKinematic = false;
    //     waitingForHydraulic = false;
    // }

    public bool IsWaiting() {
        return waitingForHydraulic;
    }
}
