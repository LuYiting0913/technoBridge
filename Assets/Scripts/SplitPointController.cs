using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitPointController : MonoBehaviour {
    public bool isSplitPoint = false;
    // private Sprite normalPoint = Resources.Load("")

    public void ToggleSplit() {
        if (!isSplitPoint) {
            // Sprite normalPoint = 
            GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
            isSplitPoint = true;
        } else {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);
            isSplitPoint = false;
        }
    }

}
