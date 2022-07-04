using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public int currentMaterial;
	
	// For 2D scene;
	public void PlayBuildSound(int material) {
		switch (material) {
			case 0:
			case 1:
				transform.GetChild(0).GetComponent<AudioSource>().Play();
				break;
			case 2:
			case 5:
				transform.GetChild(1).GetComponent<AudioSource>().Play();
				break;
			case 3:
			case 4:
				transform.GetChild(2).GetComponent<AudioSource>().Play();
				break;
		}
	}

	public void PlayHydraulicSound() {
		transform.GetChild(3).GetComponent<AudioSource>().Play();
	}

	public void PlayDragSound()
	{
		transform.GetChild(4).GetComponent<AudioSource>().Play();
	}
	
	// For 3D scene;
	public void PlayBreakSound() {
		transform.GetChild(0).GetComponent<AudioSource>().Play();
	}

	public void PlayCarSound() {
		transform.GetChild(1).GetComponent<AudioSource>().Play();
	}

	public void StopCarSound() {
		transform.GetChild(1).GetComponent<AudioSource>().Stop();
	}

	public void PlayBoatSound() {
		transform.GetChild(2).GetComponent<AudioSource>().Play();
	}

	public void StopBoatSound() {
		transform.GetChild(2).GetComponent<AudioSource>().Stop();
	}

	public void PlayHydraulicSound3D() {
		transform.GetChild(3).GetComponent<AudioSource>().Play();
	}

}
