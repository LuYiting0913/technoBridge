using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {
    public int force;

    public void Start() {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 20;
        rb.AddForce(new Vector3(1, 0, 0) * force, ForceMode.Force);
    }

    public void Update() {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(1, 0, 0) * force, ForceMode.Force);
    }
}
