using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssetManager : MonoBehaviour {
    private static List<Point> allPoints = new List<Point>();
    private static List<SolidBar> allBars = new List<SolidBar>();
    private static double offsetDistance = 30.0;
    private static double snapDistance = 30.0;
    private static float backgroundScale;
    private static Vector3 backgroundPosition;

    public static void Init(List<Point> points, List<SolidBar> bars) {
        allPoints.Clear();
        // assume the accepted points are all distinct
        foreach (Point p in points) {
            allPoints.Add(p);
        }
        allBars = bars;
    }

    public static void UpdateBackground(Vector3 v, float f) {
        backgroundPosition = v;
        backgroundScale = f;
    }

    public static Vector3 GetBackgroundPosition() {
        return backgroundPosition;
    }

    public static float GetBackgroundScale() {
        return backgroundScale;
    }

    public static List<SolidBarReference> GenerateBarReference() {
        List<SolidBarReference> reference = new List<SolidBarReference>();
        foreach (SolidBar bar in allBars) {
            SolidBarReference r = SolidBarReference.of(bar);
            reference.Add(r);
        }
        return reference;
    }

    public static List<PointReference> GeneratePointReference() {
        List<PointReference> reference = new List<PointReference>();
        foreach (Point point in allPoints) {
            PointReference r = PointReference.of(point, point.IsFixed());
            reference.Add(r);
        }
        return reference;
    }

    public static void UpdatePoints(List<Point> points) {
        allPoints = points;
    }

    public static bool HasPoint(Vector3 v) {
        foreach (Point point in allPoints) {
            if ((point.GetPosition() - v).magnitude < offsetDistance) {
                return true;
            }
        }
        return false;
    }

    public static Point GetPoint(Vector3 v) {
        foreach (Point point in allPoints) {
            if ((point.GetPosition() - v).magnitude < offsetDistance) {
                return point;
            }
        }
        return null;
    }

    public static bool HasSnap(Vector3 v) {
        foreach (Point point in allPoints) {
            if ((point.GetPosition() - v).magnitude < snapDistance) {
                return true;
            }
        }
        return false;
    }

    public static Point GetSnap(Vector3 v) {
        foreach (Point point in allPoints) {
            if ((point.GetPosition() - v).magnitude < snapDistance) {
                return point;
            }
        }
        return null;
    }

    public static void AddPoint(Point p) {
        if (!HasPoint(p.GetPosition())) {
            allPoints.Add(p);
        }
    }

    public static List<Vector3> GetAllPosition() {
        List<Vector3> all = new List<Vector3>();
        foreach (Point point in allPoints) {
            all.Add(point.GetPosition());
        }
        return all;
    }

    public static List<Point> GetAllPoints() {
        List<Point> all = new List<Point>();
        foreach (Point point in allPoints) {
            all.Add(point);
        }
        return all;
    }

    public static void DeletePoint(Point p) {
        allPoints.Remove(p);
    }

    public static void RemoveConnectedNull() {
        foreach (Point p in allPoints) p.RemoveConnectedNull();
    }

    public static void AddBar(SolidBar bar) {
        allBars.Add(bar);
    }

    public static bool HasBar(Point head, Point tail) {
        foreach (SolidBar bar in allBars) {
            if (head.Contain(bar.GetHead()) && tail.Contain(bar.GetTail()) ||
                head.Contain(bar.GetTail()) && tail.Contain(bar.GetHead())) {
                return true;
            }
        }
        return false;
    }

    public static List<SolidBar> GetAllBars() {
        return allBars;
    }

    public static void DeleteBar(SolidBar bar) {
        allBars.Remove(bar);
    }

    // private static Vector3 WorldToCanvas(Vector3 v) {
    //     Vector3 temp = (v - backgroundPosition) /  backgroundScale;
    //     return new Vector3(temp.x, temp.y, v.z);
    // }

    // private static Vector2 WorldToCanvas(Vector2 v) {
    //     Vector3 temp = (v - new Vector2(backgroundPosition.x, backgroundPosition.y)) /  backgroundScale;
    //     return new Vector2(temp.x, temp.y);
    // }
}
