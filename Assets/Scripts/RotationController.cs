using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour {

    public Vector3 axis = new Vector3(0, 0, 1);

    public float speed;
    void Update() {
        // Vector3 rot = new Vector3
        transform.Rotate(axis * speed * Time.deltaTime);
    }
}
