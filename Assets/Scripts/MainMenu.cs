using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    
    public void PlayGame() {
        Debug.Log("play game");
    }

    public void Settings() {
        Debug.Log("go to settings");
    }

    public void Go() {
        Debug.Log("Go");
        SceneInitiator.InitScene(PointManager.GetAllPos(), SolidBarManager.GetAll());
        SceneManager.LoadScene(2);
    }
}   
