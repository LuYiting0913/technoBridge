using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level18 : MonoBehaviour {
    // private static m_Instance;
    private static List<PointReference> fixedPoints = new List<PointReference>();

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();
        PointReference p3 = new PointReference();
        PointReference p4 = new PointReference();
        PointReference p5 = new PointReference();

        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p2.SetFixed(); 
        p3.SetFixed();
        p4.SetFixed(); 
        p5.SetFixed();
        
        p1.SetPosition(new Vector3(-340, -140, 0));
        p2.SetPosition(new Vector3(-20, -40, 0));
        p3.SetPosition(new Vector3(50, -40, 0));
        p4.SetPosition(new Vector3(-340, 40, 0));
        p5.SetPosition(new Vector3(320, -40, 0));

        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        fixedPoints.Add(p3);
        fixedPoints.Add(p4);
        fixedPoints.Add(p5);
        
        Levels.UpdateLevelData(18, fixedPoints, new List<SolidBarReference>());
    }
}
