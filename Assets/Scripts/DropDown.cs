using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDown : MonoBehaviour {
    private bool isDroppedDown, isTransforming;
    private float increment;

    public void ToggleDropDown() {
        if (isDroppedDown) {
            isDroppedDown = false;
            isTransforming = true;
            increment = -0.1f;
            // transform.localScale = new Vector3(1, 0, 1);
        } else {
            // gameObject.SetActive(true);
            isDroppedDown = true;
            isTransforming = true;
            increment = 0.1f;
            // transform.localScale = new Vector3(1, 1, 1);
        }
    } 

    private void FixedUpdate() {
        if (isTransforming) {
            transform.localScale += new Vector3(0, increment, 0);
            if (transform.localScale.y > 1) {
                isTransforming = false;
                transform.localScale = new Vector3(1, 1, 1);
            } else if (transform.localScale.y < 0) {
                isTransforming = false;
                transform.localScale = new Vector3(1, 0, 1);
                // gameObject.SetActive(true);
            }
        }
        

    }

}
