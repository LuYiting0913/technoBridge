using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitBarController : MonoBehaviour {
    // private static SplitBarController m_Instance;
    public int headOrTail;
    // 0 for head, 1 for tail


    public void ToggleSplit() {
        transform.parent.GetComponent<SolidBar>().ToggleSplitParent(headOrTail);
    }


}
