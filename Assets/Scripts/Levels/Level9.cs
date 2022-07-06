using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level9 : MonoBehaviour {
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
        
        p1.SetPosition(new Vector3(120, -130, 0));
        p2.SetPosition(new Vector3(-140, 90, 0));
        p3.SetPosition(new Vector3(-140, 180, 0));
        p4.SetPosition(new Vector3(120, -40, 0));      

        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        fixedPoints.Add(p3);
        fixedPoints.Add(p4);
        
        Levels.UpdateLevelData(9, fixedPoints, new List<SolidBarReference>());
    }
}
