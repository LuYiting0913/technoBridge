using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicController : MonoBehaviour {
    private static float extendLimit = -1.5f;
    private static float contractLimit = 0.5f;

    private ConfigurableJoint controllJoint;

    public void ConvertToHydraulic() {
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


}
