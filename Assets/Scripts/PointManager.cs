using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointManager : MonoBehaviour {
    private static Dictionary<Vector2, Point> allPoints = new Dictionary<Vector2, Point>();
    private static double XOffset = 10.0;
    private static double YOffset = 10.0;

    public void Awake() {
        allPoints.Clear();
    }

    public static bool HasPoint(Vector2 v) {
        foreach (Vector2 key in allPoints.Keys) {
            if (Math.Abs(key.x - v.x) < XOffset && Math.Abs(key.y - v.y) < YOffset) {
                return true;
            }
        }
        return false;
    }

    public static Point GetPoint(Vector2 v) {
        foreach (Vector2 key in allPoints.Keys) {
            if (Math.Abs(key.x - v.x) < XOffset && Math.Abs(key.y - v.y) < YOffset) {
                return allPoints[key];
            }
        }
        return null;
    }

    public static void AddPoint(Vector2 v, Point p) {
        allPoints.Add(v, p);
    }

    public static List<Vector2> GetAllPos() {
        List<Vector2> all = new List<Vector2>();
        foreach (Point p in allPoints.Values) {
            all.Add(p.transform.position);
        }
        return all;
    }

    public static void DeletePoint(Point p) {
        allPoints.Remove(p.transform.position);
    }
}
