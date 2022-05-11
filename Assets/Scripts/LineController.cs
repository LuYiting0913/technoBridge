using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour {
    private Transform[] points;
    private LineRenderer lr;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] points) {
        lr.positionCount = points.Length;
        this.points = points;
    }

    public void Update() {
        for (int i = 0; i < lr.positionCount; i++) {
            lr.SetPosition(i, this.points[i].position);
        }
    }
}
