using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtonController : MonoBehaviour {
    public int level;
    // private LevelMenuController levelMenuController;
    private bool isClicked = false;
    private int clickCount;

    void Start() {
        
		if (Levels.IsLevelEdited(level) || GlobalData.GetLocalData(level) != 0) {
			GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
		}
        /*
        int star = GlobalData.GetStarLevel(level);
        if (GlobalData.GetLocalData(level) != 0) {
	        if (star == 3) {
		        GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
	        } else if (star == 2) {
		        GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
	        } else if (star == 1) {
		        GetComponent<MeshRenderer>().material.color = new Color(1, 230f/255f, 30f/255f);
	        }
        }*/
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
