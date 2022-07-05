using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssetManager : MonoBehaviour {
    public static List<Point> allPoints = new List<Point>();
    private static List<SolidBar> allBars = new List<SolidBar>();
    private static int totalCost;
    private static double offsetDistance = 20.0;
    private static double snapDistance = 20.0;
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
            // Vector2 v1 = new Vector2(point.GetPosition().x, point.GetPosition().y);
            // Vector2 v2 = new Vecotr2(v.x, v.y);
            if ((point.GetPosition() - v).magnitude < offsetDistance) {
                return true;
            }
        }
        return false;
    }

    public static bool HasPointInWorld(Vector3 v) {
        foreach (Point point in allPoints) {
            // Vector2 v1 = new Vector2(point.GetWorldPosition().x, point.GetWorldPosition().y);
            // Vector2 v2 = new Vecotr2(v.x, v.y);
            if ((point.GetWorldPosition() - v).magnitude < offsetDistance) {
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

    public static Point GetPointInWorld(Vector3 v) {
        foreach (Point point in allPoints) {
            // Vector2 v1 = new Vector2(point.GetWorldPosition().x, point.GetWorldPosition().y);
            if ((point.GetWorldPosition() - v).magnitude < offsetDistance) {
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

    public static void UpdateCost(int c) {
        totalCost = c;
    }

    public static int GetTotalCost() {
        return totalCost;
    }
}
