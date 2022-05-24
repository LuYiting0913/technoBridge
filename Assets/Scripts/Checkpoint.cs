using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint {
    private static int threshold = 20;
    public Vector3 position;

    private Checkpoint(Vector3 pos) {
        position = pos;
    }

    public static Checkpoint Of(Vector3 pos) {
        return new Checkpoint(pos);
    }

    public bool Arrived(Vector3 pos) {
        return (position - pos).magnitude <= threshold;
    } 
}
