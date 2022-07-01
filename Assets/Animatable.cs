using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animatable : MonoBehaviour {
    private float speed = 1f;
    private bool animating = false;

    public void StartAnimation() {
        animating = true;
    }

    public void FixedUpdate() {
        if (animating) {
            transform.position -= new Vector3(0, 0, speed); 
            if (transform.position.z < -100) animating = false; 
        }
    }
}
