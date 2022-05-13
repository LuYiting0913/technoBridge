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

    public void BackToMain() {
        SceneManager.LoadScene(0);
    }

    public void LoadLevel0() {
        //List<Point>
        Debug.Log("level0");
        LevelInitiator.InitLevel(Level0.GetFixedPoints());
        //Debug.Log(fixedPoints[0]);
        SceneManager.LoadScene(1);
    }

    public void Go() {
        Debug.Log("Go");
        SceneInitiator.InitScene(PointManager.GetAllPos(), SolidBarManager.GetAll());
        SceneManager.LoadScene(2);
    }
}   
