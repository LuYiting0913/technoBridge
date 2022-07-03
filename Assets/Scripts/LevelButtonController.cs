using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtonController : MonoBehaviour {
    public int level;
    // private LevelMenuController levelMenuController;
    private bool isClicked = false;
    private int clickCount;

    void Start() {
		if (Levels.GetLevelData().ContainsKey(level)) {
			GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
		}
        // levelMenuController = transform.parent.parent.GetComponent<LevelMenuController>();
	}

    // private void OnMouseEnter() {
    //     transform.position += new Vector3(0, 10, 0);
    //     // GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
    // }

    // private void OnMouseExit() {
    //     transform.position -= new Vector3(0, 10, 0);
    //     // GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
    // }

    private void OnMouseDown() {
        Debug.Log("clicked");  
        // clickCount += 1;
        // if (clickCount == 2) {
        //     transform.parent.GetComponent<MainMenu>().LoadLevel(level);
        // }  
        LevelMenuController.GetInstance().FirstClick(this);
        
    } 

    public void LoadThisLevel() {
        transform.parent.parent.GetComponent<MainMenu>().LoadLevel(level);
    }

    public void Rise() {
        transform.position += new Vector3(0, 6, 0);
    }

    public void Drop() {
        transform.position -= new Vector3(0, 6, 0);
    }

}
