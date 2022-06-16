using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour{
    private static int threshold = 30;
    public bool activateHydraulic;
    
    // public Vector3 position;

    // private Checkpoint(Vector3 pos) {
    //     position = pos;
    // }

    // public static Checkpoint Of(Vector3 pos) {
    //     return new Checkpoint(pos);
    // }

    public bool Arrived(Vector3 pos) {
        return (transform.position - pos).magnitude <= threshold;
    } 
}
