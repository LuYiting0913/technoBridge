using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurveEditor : MonoBehaviour {

    private Vector3 head, tail, control;
    public LineRenderer renderer;
    private float segmentLength;
    private int numOfVert;
    public Transform curveEditor;
    private List<Vector3> points = new List<Vector3>();

    public List<Vector3> DrawTrace(Vector3 h, Vector3 t, int material, int lineType) {
        head = h;
        tail = t;
        control = (head + tail) / 2; 
        if (lineType == 1) {
            control += new Vector3(0, 200, 0);
            ActivateCurveEditor();
        } else {
            DeactivateCurveEditor();
        }
        segmentLength = MaterialManager.GetMaxLength(material);
        segmentLength = segmentLength > 300 ? 100 : segmentLength;

        numOfVert = (int) ((Math.PI * (head - tail).magnitude / 2) / segmentLength) - 1;
        Draw();
        return points;
    }

    public void Draw() {
        points = new List<Vector3>();
        points.Add(head);
        for (float i = 1f; i < numOfVert; i++) {
            float ratio = i / ((float) numOfVert);
            Vector3 tangent1 = Vector3.Lerp(head, control, ratio);
            Vector3 tangent2 = Vector3.Lerp(control, tail, ratio);
            Vector3 curve = Vector3.Lerp(tangent1, tangent2, ratio);

            points.Add(curve);
        }
        // points.RemoveAt(points.Count - 1);
        points.Add(tail);
        renderer.enabled = true;
        renderer.startColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        renderer.endColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        renderer.positionCount = numOfVert + 1;
        
        renderer.SetPositions(points.ToArray());
        // return points;
    }

    private void ActivateCurveEditor() {
        curveEditor.gameObject.SetActive(true);
        curveEditor.GetChild(0).transform.position = control;
        LineRenderer controlLine = curveEditor.GetChild(1).GetComponent<LineRenderer>();
        List<Vector3> pts = new List<Vector3>() {head, control};
        controlLine.positionCount = 2;
        controlLine.startColor = new Color(0f, 0f, 0f, 0.6f);
        controlLine.endColor = new Color(0f, 0f, 0f, 0.6f);
        controlLine.SetPositions(pts.ToArray());
    }

    private void DeactivateCurveEditor() {
        curveEditor.gameObject.SetActive(false);
    }

    public List<Vector3> UpdateEditor(Vector2 newPos) {
        control = new Vector3(newPos.x, newPos.y, control.z);
        ActivateCurveEditor();
        Draw();
        return points;
    }

    public void Close() {
        renderer.enabled = false;
        head = Vector3.zero;
        tail = Vector3.zero;
        control = Vector3.zero;
        points = new List<Vector3>();
        DeactivateCurveEditor();
    }

    private void Update() {
        
    }

}
