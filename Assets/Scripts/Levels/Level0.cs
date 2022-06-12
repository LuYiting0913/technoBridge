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
        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p1.SetPosition(new Vector3(-300, -60, 0));
        p2.SetFixed();
        p2.SetPosition(new Vector3(300, -10, 0));
        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        Levels.UpdateLevelData(0, fixedPoints, new List<SolidBarReference>());

        vehicles.Add(Vehicle.Of(1, new Vector3(-350, -40, 90), new Vector3(350,0, 90)));
        Levels.SetVehicleData(0, vehicles);

        Levels.SetBudget(0, budget);

    }

}
