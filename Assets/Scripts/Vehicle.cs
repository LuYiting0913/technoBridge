using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Vehicle {
    private static Dictionary<int, string> vehicleTypes = new Dictionary<int, string>() {
        {1, "Prefab/Vehicles/Sedan"}
    };

    public int vehicleType = 1;
    private Vector3 position;
    private Checkpoint checkpoint;

    private Vehicle(int type, Vector3 pos, Vector3 checkpointPos) {
        vehicleType = type;
        position = pos;
        checkpoint = Checkpoint.Of(checkpointPos);
    }

    public static Vehicle Of(int type, Vector3 pos, Vector3 checkpointPos) {
        return new Vehicle(type, pos, checkpointPos);
    }

    public void SetPosition(Vector3 v) {
        position = v;
    }

    public Vector3 GetPosition() {
        return position;
    }

    public GameObject GetVehicle() {
        GameObject vehicle = Resources.Load<GameObject>(vehicleTypes[vehicleType]);
        vehicle.GetComponent<VehicleController>().SetCheckpoint(checkpoint);
        return vehicle;
    }

    public Checkpoint GetCheckpoint() {
        return checkpoint;
    }
}
