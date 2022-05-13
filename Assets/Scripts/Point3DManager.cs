using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Point3DManager : MonoBehaviour {
    private static Dictionary<Vector3, Rigidbody> allPoints = new Dictionary<Vector3, Rigidbody>();
    private static double XOffset = 5.0;
    private static double YOffset = 5.0;

    public static Rigidbody GetPoint(Vector3 v) {
        foreach (Vector3 key in allPoints.Keys) {
            if (Math.Abs(key.x - v.x) < XOffset && Math.Abs(key.y - v.y) < YOffset) {
                return allPoints[key];
            }
        }
        return null;
    }

    public static void AddPoint(Vector3 v, Rigidbody obj) {
        allPoints.Add(v, obj);
    }
}
