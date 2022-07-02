using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtonController : MonoBehaviour {
    public int level;
    private bool isClicked = false;

    void Start() {
		if (Levels.GetLevelData().ContainsKey(level)) {
			GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
		}
	}

    private void OnMouseEnter() {
        transform.position += new Vector3(0, 10, 0);
        // GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
    }

    private void OnMouseExit() {
        transform.position -= new Vector3(0, 10, 0);
        // GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
    }

    private void OnMouseDown() {
        Debug.Log("clicked");    
        transform.parent.GetComponent<MainMenu>().LoadLevel(level);
    } 

}
