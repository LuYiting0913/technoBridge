using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animatable : MonoBehaviour {
    private float speed = 1f;
    private bool animating = false;
	public Checkpoint checkpoint;
	public bool arrived;

    public void StartAnimation() {
        animating = true;
    }
	
	public bool ArrivedAtCheckpoint(Checkpoint pt) {
        return pt.Arrived(transform.position);
    }

    public void FixedUpdate() {
		if (ArrivedAtCheckpoint(checkpoint)) {
			arrived = true;
		} else if (animating) {
            transform.position -= new Vector3(0, 0, speed); 
            if (transform.position.z < -400) animating = false; 
        }
    }
	
	public bool Arrived() {
		return arrived;
	}
}
