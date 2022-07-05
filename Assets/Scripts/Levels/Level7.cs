using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level7 : MonoBehaviour {
    // private static m_Instance;
    private static List<PointReference> fixedPoints = new List<PointReference>();

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();
        PointReference p3 = new PointReference();
        PointReference p4 = new PointReference();
        PointReference p5 = new PointReference();
        PointReference p6 = new PointReference();
        PointReference p7 = new PointReference();
        PointReference p8 = new PointReference();
        List<Vehicle> vehicles = new List<Vehicle>();

        p1.SetFixed();
        p2.SetFixed();
        p3.SetFixed();
        p4.SetFixed();
        p5.SetFixed();
        p6.SetFixed();
        p7.SetFixed();
        p8.SetFixed();

        p1.SetPosition(new Vector3(-340, -60, 0));
        p2.SetPosition(new Vector3(-60, -10, 0));
        p3.SetPosition(new Vector3(50, -10, 0));
        p4.SetPosition(new Vector3(340, -50, 0));
        p5.SetPosition(new Vector3(-340, -130, 0));
        p6.SetPosition(new Vector3(-440, -60, 0));
        p7.SetPosition(new Vector3(430, -50, 0));
        p8.SetPosition(new Vector3(340, -130, 0));

        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        fixedPoints.Add(p3);
        fixedPoints.Add(p4);
        fixedPoints.Add(p5);
        fixedPoints.Add(p6);
        fixedPoints.Add(p7);
        fixedPoints.Add(p8);
        Levels.UpdateLevelData(7, fixedPoints, new List<SolidBarReference>());
    }
}
