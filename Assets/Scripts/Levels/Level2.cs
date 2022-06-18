using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level2 : MonoBehaviour {
    // private static m_Instance;
    private static List<PointReference> fixedPoints = new List<PointReference>();

    // private void Awake() {
    //     if (m_Instance == null) {
    //         m_Instance = this;
    //         //DontDestroyOnLoad(m_Instance);
    //     } else if (m_Instance != this) {
    //         Destroy(m_Instance);
    //     }
    // }

    // public Level1 GetInstance() {
    //     return m_Instance;
    // }

    // private void Awake() {
    //     InitLevel();
    // }

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();
        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p1.SetPosition(new Vector3(-250, -15, 0));
        p2.SetFixed();
        p2.SetPosition(new Vector3(180, -15, 0));
        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        Levels.UpdateLevelData(2, fixedPoints, new List<SolidBarReference>());
        
	    // vehicles.Add(Vehicle.Of(2, new Vector3(-320, 0, 90), new Vector3(180,0, 90)));
        // Levels.SetVehicleData(2, vehicles);
        // Debug.Log(vehicles[0]);
        
	// vehicles.Add(Vehicle.Of(1, new Vector3(-150, 0, 90), new Vector3(150,0, 90)));
        // Levels.SetVehicleData(1, vehicles);
        //Debug.Log(vehicles[0]);
    }
}
