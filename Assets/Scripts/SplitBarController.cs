using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitBarController : MonoBehaviour {
    // private static SplitBarController m_Instance;
    public int headOrTail;
    // 0 for head, 1 for tail

    // private void Awake() {
    //     if (m_Instance == null) {
    //         m_Instance = this;
    //         //DontDestroyOnLoad(m_Instance);
    //     } else if (m_Instance != this) {
    //         Destroy(m_Instance);
    //     }
    // }

    // public static SplitBarController GetInstance() {
    //     return m_Instance;
    // }

    // public void OnPressed(object source, Stage1Controller e) {
    //     RaycastHit2D hit = Physics2D.Raycast(e.startPoint, new Vector3(0, 0, 1));

    //     // if ()
    // }

    public void ToggleSplit() {
        transform.parent.GetComponent<SolidBar>().ToggleSplitParent(headOrTail);
    }


}
