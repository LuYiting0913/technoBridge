using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HydraulicController : MonoBehaviour {
    // private static float extendLimit = -1.5f;
    // private static float contractLimit = 0.5f;
    public float speed = 0.002f;
    public float target, f;
    public bool active;

    private ConfigurableJoint controllJoint;

    public void ConvertToHydraulic(float f) {

        SetAction(f);
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        Rigidbody pivot = joint.connectedBody;
        Point point = pivot.GetComponent<Point>();

        joint.connectedBody = null;
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        ConfigurableJoint newJoint = point.gameObject.AddComponent<ConfigurableJoint>();
        newJoint.connectedBody = GetComponent<Rigidbody>();
        newJoint.xMotion = ConfigurableJointMotion.Locked;
        newJoint.yMotion = ConfigurableJointMotion.Locked;
        newJoint.zMotion = ConfigurableJointMotion.Locked;
        newJoint.angularXMotion = ConfigurableJointMotion.Locked;
        newJoint.angularZMotion = ConfigurableJointMotion.Locked;

        // SoftJointLimit softJointLimit = new SoftJointLimit();
        // softJointLimit.limit = 0.5f;
        // newJoint.linearLimit = softJointLimit;
        // newJoint.linearLimit.limit = 2f;
        newJoint.anchor = new Vector3(0, 0, 0);
        newJoint.axis = new Vector3(0, 0, 1);
        newJoint.autoConfigureConnectedAnchor = false;
        newJoint.connectedAnchor = new Vector3(0, -1, 0);
        controllJoint = newJoint;
    }

    private void SetAction(float factor) {
        f = factor;
        // Debug.Log(factor);
        if (factor >= 1) {
            SetExtend(factor);
        } else {
            SetContract(factor);
        }
    }

    private void SetContract(float t) {
        speed = Math.Abs(speed);
        target = 1 - 2 * t;
    }

    private void SetExtend(float t) {
        speed = - Math.Abs(speed);
        target = 1 - 2 * t;
    }

    public void Activate() {
        active = true;
        // controllJoint.connectedAnchor += new Vector3(0, speed * 10, 0);
        controllJoint.xMotion = ConfigurableJointMotion.Limited;
        SoftJointLimit softJointLimit = new SoftJointLimit();
        softJointLimit.limit = 0.5f;
        controllJoint.linearLimit = softJointLimit;
        controllJoint.connectedAnchor += new Vector3(0, speed * 2, 0);
        controllJoint.xMotion = ConfigurableJointMotion.Locked;
        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayHydraulicSound3D();
    }

    public void Deactivate() {
        active = false;
    }

    private void FixedUpdate() {
        if (active) {
            Debug.Log("working");
            Vector3 v = controllJoint.connectedAnchor;
            // Debug.Log(controllJoint.connectedBody);
        
            if (speed > 0) {
                // contract
                if (v.y < target) {
                    controllJoint.connectedAnchor = new Vector3(v.x, v.y + speed, v.z);
                } else {
                    SwapDirection();
                }
            } else {
                // extend
                if (v.y > target) {
                    controllJoint.connectedAnchor = new Vector3(v.x, v.y + speed, v.z);
                } else {
                    SwapDirection();
                }
            }
        }
    }

    public void OnActivated(object source, Stage2Controller e) {
        Debug.Log("recieve activation");
        this.Activate();
    }

    private void SwapDirection() {
        // if (speed > 0) {
        //     // set to extend
        //     SetExtend(1f / f);
        // } else {
        //     SetContract(1f / f);
        // }
        speed = - speed;
        f = 1f / f;
        if (target != -1) {
            target = -1;
        } else {
            target = 1 - 2 * f; 
        }
        
        Deactivate();
    }



}
