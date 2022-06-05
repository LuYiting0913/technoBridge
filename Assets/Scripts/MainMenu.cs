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

    public void SaveAndBackToMain() {
        Levels.UpdateLevelData(0, AssetManager.GeneratePointReference(), AssetManager.GenerateBarReference());
        SceneManager.LoadScene(0);
    }

    public void LoadLevel0() {
        //List<Point>
        Debug.Log("level0");
        //Level0.LoadFixedPoints();
        //Debug.Log(Levels.GetPointData(0).Count);
        //Debug.Log(fixedPoints[0]);
        SceneManager.LoadScene(1);
    }

    public void Go() {
        Debug.Log("Go");
        Levels.UpdateLevelData(0, AssetManager.GeneratePointReference(), AssetManager.GenerateBarReference());
        Levels.UpdateBackground(0, AssetManager.GetBackgroundPosition(), AssetManager.GetBackgroundScale());
        SceneInitiator.InitScene(0);
        SceneManager.LoadScene(2);
    }
}   
