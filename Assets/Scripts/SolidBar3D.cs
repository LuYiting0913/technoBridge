using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBar3D : MonoBehaviour {
    private Rigidbody head;
    private Rigidbody tail;
    public GameObject barTemplate;
    public HingeJoint headJoint;
    public HingeJoint tailJoint;
    //private int matrialType .....

    // init 3d bar at start
    public void InitSolidBar(Rigidbody headPoint, Rigidbody tailPoint) {
        head = headPoint;
        tail = tailPoint;
        headJoint.connectedBody = head;
        tailJoint.connectedBody = tail;
    }

    public Rigidbody GetHead() {
        return head;
    }  

    public Rigidbody GetTail() {
        return tail;
    }
}
