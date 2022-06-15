using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HydraulicController : MonoBehaviour {
    // private static float extendLimit = -1.5f;
    // private static float contractLimit = 0.5f;
    private float speed = 0.002f;
    private float target;
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
        newJoint.anchor = new Vector3(0, 0, 0);
        newJoint.axis = new Vector3(0, 0, 1);
        newJoint.autoConfigureConnectedAnchor = false;
        newJoint.connectedAnchor = new Vector3(0, -1, 0);
        controllJoint = newJoint;
    }

    private void SetAction(float factor) {
        Debug.Log(factor);
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
    }

    public void Deactivate() {
        active = false;
    }

    private void FixedUpdate() {
        if (active) {
            Vector3 v = controllJoint.connectedAnchor;
            if (speed > 0) {
                // contract
                if (v.y < target) controllJoint.connectedAnchor = new Vector3(v.x, v.y + speed, v.z);
            } else {
                // extend
                if (v.y > target) controllJoint.connectedAnchor = new Vector3(v.x, v.y + speed, v.z);
            }
        }
    }


}
