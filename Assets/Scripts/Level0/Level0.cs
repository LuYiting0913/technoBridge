using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store all information abt this level
public class Level0 : MonoBehaviour {
    private static List<PointReference> fixedPoints = new List<PointReference>();

    public static void InitLevel() {
        PointReference p1 = new PointReference();
        PointReference p2 = new PointReference();

        p1.SetFixed();
        p1.SetPosition(new Vector3(-300, -60, 0));
        p2.SetFixed();
        p2.SetPosition(new Vector3(300, -10, 0));
        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        Levels.UpdateLevelData(0, fixedPoints, new List<SolidBarReference>());
    }

}
