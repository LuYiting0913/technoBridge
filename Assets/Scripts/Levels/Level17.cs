using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level17 : MonoBehaviour {
    // private static m_Instance;
    private static List<PointReference> fixedPoints = new List<PointReference>();

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();
        // PointReference p3 = new PointReference();

        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p2.SetFixed(); 
        // p3.SetFixed();
        
        p1.SetPosition(new Vector3(-270, -90, 0));
        p2.SetPosition(new Vector3(-270, -170, 0));
        // p3.SetPosition(new Vector3(270, -90, 0));

        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        // fixedPoints.Add(p3);
        
        Levels.UpdateLevelData(17, fixedPoints, new List<SolidBarReference>());
    }
}
