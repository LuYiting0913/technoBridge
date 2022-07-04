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
        SceneManager.LoadScene(1);
    }

    public void SaveAndBackToMain(int level) {
        Levels.UpdateLevelData(level, AssetManager.GeneratePointReference(), AssetManager.GenerateBarReference());
        SceneManager.LoadScene(2);
    }

    public void LoadLevelMenu() {
        SceneManager.LoadScene(2);
    }

    public void LoadLevel(int level) {
        //List<Point>
        Debug.Log("level" + level);
        //Level0.LoadFixedPoints();
        //Debug.Log(Levels.GetPointData(0).Count);
        //Debug.Log(fixedPoints[0]);
        SceneManager.LoadScene(2 * level + 3);
    }

    public void LoadTutorial(int level) {
        SceneManager.LoadScene(3 + level * 2);
    }

    public void ClearAllTutorial() {
        for (int i = 1; i <= 4; i++) {
            Levels.ClearLevel(i);
        }
    }

    public void Go(int level) {
        Debug.Log("Go");
        Levels.UpdateLevelData(level, AssetManager.GeneratePointReference(), AssetManager.GenerateBarReference());
        Levels.UpdateBackground(level, AssetManager.GetBackgroundPosition(), AssetManager.GetBackgroundScale());
        SceneInitiator.InitScene(level);
        SceneManager.LoadScene(2 * level + 4);
    }
}   
