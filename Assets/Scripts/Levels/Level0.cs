using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level0 : MonoBehaviour {
    private static List<PointReference> fixedPoints = new List<PointReference>();
    private static int budget = 50000;

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();
        PointReference p3 = new PointReference();
        PointReference p4 = new PointReference();
        PointReference p5 = new PointReference();
        PointReference p6 = new PointReference();
        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p2.SetFixed();
        p3.SetFixed();
        p4.SetFixed();
        p5.SetFixed();
        p6.SetFixed();
        p1.SetPosition(new Vector3(-300, -85, 0));
        p2.SetPosition(new Vector3(300, -10, 0));
        p3.SetPosition(new Vector3(-30, -50, 0));
        p4.SetPosition(new Vector3(-300, 15, 0));
        p5.SetPosition(new Vector3(300, 90, 0));
        p6.SetPosition(new Vector3(50, -50, 0));
        

        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        fixedPoints.Add(p3);
        fixedPoints.Add(p4);
        fixedPoints.Add(p5);
        fixedPoints.Add(p6);
        Levels.UpdateLevelData(0, fixedPoints, new List<SolidBarReference>());

        // vehicles.Add(Vehicle.Of(1, new Vector3(-350, -80, 90), new Vector3(350,0, 90)));
        // Levels.SetVehicleData(0, vehicles);


    }

}
