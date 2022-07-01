using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animatable : MonoBehaviour {
    private float speed = 1f;
    public bool animating = false;
	// public Checkpoint checkpoint;
	// public bool arrived;

    public void StartAnimation() {
        animating = true;
    }
	
    private void OnCollisionEnter(Collision other) {
        Debug.Log("hit sth");
    }

    public void FixedUpdate() {
		// if (ArrivedAtCheckpoint(checkpoint)) {
			// arrived = true;
		if (animating) {
            transform.position -= new Vector3(0, 0, speed); 
            if (transform.position.z < -700) animating = false; 
        }

        // DetectCollision()
    }
	
	// public bool Arrived() {
	// 	return arrived;
	// }
}
