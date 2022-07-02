using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurveEditor : MonoBehaviour {

    private Vector3 head, tail, control;
    public LineRenderer renderer;
    private float segmentLength;
    private int numOfVert;
    public Transform controlPoint;
    private List<Vector3> points = new List<Vector3>();

    public List<Vector3> DrawTrace(Vector3 h, Vector3 t, int material, int lineType) {
        head = h;
        tail = t;
        control = (head + tail) / 2; 
        if (lineType == 1)  control += new Vector3(0, 200, 0);
        segmentLength = MaterialManager.GetMaxLength(material);
        Debug.Log(head);
        Debug.Log(tail);
        Debug.Log(control);
        numOfVert = (int) ((Math.PI * (head - tail).magnitude / 2) / segmentLength) - 1;
        Draw();
        return points;
    }

    public void Draw() {
        points = new List<Vector3>();
        points.Add(head);
        for (float ratio = 0; ratio <= 1; ratio += 1 / (float) numOfVert) {
            Vector3 tangent1 = Vector3.Lerp(head, control, ratio);
            Vector3 tangent2 = Vector3.Lerp(control, tail, ratio);
            Vector3 curve = Vector3.Lerp(tangent1, tangent2, ratio);

            points.Add(curve);
        }
        points.Add(tail);
        renderer.enabled = true;
        renderer.positionCount = numOfVert + 2;
        renderer.SetPositions(points.ToArray());
    }

    public List<Vector3> GetAllPoints() {
        return points;
    }

    public void Close() {
        renderer.enabled = false;
    }

    private void Update() {
        
    }

}
