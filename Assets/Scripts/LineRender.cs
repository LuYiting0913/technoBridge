using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRender : MonoBehaviour {
    [SerializeField] private Transform[] points;
    [SerializeField] private LineController line;

    public void Start() {
        line.SetUpLine(points);
    } 
}
