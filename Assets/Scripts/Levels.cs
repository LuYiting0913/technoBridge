using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Levels {
    private static Dictionary<int, List<PointReference>> pointData = new Dictionary<int, List<PointReference>>();
    private static Dictionary<int, List<SolidBarReference>> barData = new Dictionary<int, List<SolidBarReference>>();
    private static Dictionary<int, List<Vehicle>> vehicleData = new Dictionary<int, List<Vehicle>>();
    private static Dictionary<int, Vector3> backgroundPosition = new Dictionary<int, Vector3>();
    private static bool[] isInited = new bool[10];
    //private static Dictionary<int, List<Checkpoint>> checkpointData = new Dictionary<int, List<Checkpoint>>();

    public static List<PointReference> GetPointData(int level) {
        return pointData[level];
    }

    public static List<SolidBarReference> GetBarData(int level) {
        return barData[level];
    }

    public static void UpdateLevelData(int level, List<PointReference> newPoints, List<SolidBarReference> newBars) {
        Debug.Log("Updated to levels");
        isInited[level] = true;
        pointData[level] = newPoints;
        barData[level] = newBars;
    }

    public static void SetVehicleData(int level, List<Vehicle> vehicles) {
        vehicleData[level] = vehicles;
    }

    public static List<Vehicle> GetVehicleData(int level) {
        return vehicleData[level];
    }

    public static bool IsInited(int level) {
        return isInited[level];
    }

    public static void SetBackgroundPosition(int level, Vector3 v) {
        backgroundPosition[level] = v;
    }

    public static Vector3 GetBackgroundPosition(int level) {
        return backgroundPosition.ContainsKey(level) ? backgroundPosition[level] : new Vector3(0, 0, 105);
    }

}
