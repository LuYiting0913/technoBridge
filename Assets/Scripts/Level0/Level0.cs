using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0 : MonoBehaviour {
    private static List<Vector2> fixedPoints = new List<Vector2>();

    public static List<Vector2> GetFixedPoints() {
        Vector2 p1 = new Vector2(-300, -60);
        Vector2 p2 = new Vector2(300,-10);
        fixedPoints.Add(p1);
        fixedPoints.Add(p2);
        return fixedPoints;
    }
}
