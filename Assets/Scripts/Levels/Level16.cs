using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level16 : MonoBehaviour {
    // private static m_Instance;
    private static List<PointReference> fixedPoints = new List<PointReference>();

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();
        PointReference p3 = new PointReference();
        PointReference p4 = new PointReference();
        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p2.SetFixed();
        p3.SetFixed();
        p4.SetFixed();       
        
        p1.SetPosition(new Vector3(-340, 0, 0));
        p2.SetPosition(new Vector3(300, 0, 0));
        p3.SetPosition(new Vector3(-340, -90, 0));
        p4.SetPosition(new Vector3(300, -90, 0));    

        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        fixedPoints.Add(p3);
        fixedPoints.Add(p4);
        
        Levels.UpdateLevelData(16, fixedPoints, new List<SolidBarReference>());
    }
}
