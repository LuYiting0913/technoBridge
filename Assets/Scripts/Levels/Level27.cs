using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level27 : MonoBehaviour {
    // private static m_Instance;
    private static List<PointReference> fixedPoints = new List<PointReference>();

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();
        PointReference p3 = new PointReference();
        PointReference p4 = new PointReference();
        // PointReference p5 = new PointReference();
        // PointReference p6 = new PointReference();
        // PointReference p7 = new PointReference();
        // PointReference p8 = new PointReference();
        // PointReference p9 = new PointReference();
        // PointReference p10 = new PointReference();
        // PointReference p5 = new PointReference();

        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p2.SetFixed(); 
        p3.SetFixed();
        p4.SetFixed(); 
        // p5.SetFixed();
        // p6.SetFixed();
        // p7.SetFixed();
        // p8.SetFixed();
        // p9.SetFixed();
        // p10.SetFixed();
        // 
        p1.SetPosition(new Vector3(-190, 40, 0));
        p2.SetPosition(new Vector3(-190, 100, 0));
        p3.SetPosition(new Vector3(200, -70, 0));
        p4.SetPosition(new Vector3(-250, 100, 0));
        // p5.SetPosition(new Vector3(280, -20, 0));
        // p6.SetPosition(new Vector3(280, 40, 0));
        // p7.SetPosition(new Vector3(280, 100, 0));
        // p8.SetPosition(new Vector3(280, 160, 0));
        // p9.SetPosition(new Vector3(100, 280, 0));
        // p10.SetPosition(new Vector3(-100, 280, 0));

        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        fixedPoints.Add(p3);
        fixedPoints.Add(p4);
        // fixedPoints.Add(p5);
        // fixedPoints.Add(p6);
        // fixedPoints.Add(p7);
        // fixedPoints.Add(p8);
        
        Levels.UpdateLevelData(27, fixedPoints, new List<SolidBarReference>());
    }
}
