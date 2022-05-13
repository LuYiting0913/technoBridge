using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInitiator : MonoBehaviour {
    private static List<Vector2> fixedPoints = new List<Vector2>();
    public Transform pointParent;
    public Point pointTemplate;

    public static void InitLevel(List<Vector2> points) {
        fixedPoints = points;
        Debug.Log("inited");
    }

    public void Start() {
        foreach (Vector2 v in fixedPoints) {
            Point p = Instantiate(pointTemplate, v, Quaternion.identity, pointParent).GetComponent<Point>();
            p.fixPoint();
            PointManager.AddPoint(v, p);
        }
         Debug.Log("loeaded");
    }
}
