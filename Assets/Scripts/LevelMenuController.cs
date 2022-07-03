using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuController : MonoBehaviour {
    private static LevelMenuController m_Instance;

    public Transform infoPageParent;
    public Transform pointer;
    private LevelButtonController currentLevelButton;

    private void Awake() {
        if (m_Instance == null) {
            m_Instance = this;
            //DontDestroyOnLoad(m_Instance);
        } else if (m_Instance != this) {
            Destroy(m_Instance);
        }
    }

    public static LevelMenuController GetInstance() {
        return m_Instance;
    }

    public void FirstClick(LevelButtonController clickedButton) {
        if (clickedButton == currentLevelButton) {
            SecondClick();
        } else {
            clickedButton.Rise();
            if (currentLevelButton != null) currentLevelButton.Drop();
            currentLevelButton = clickedButton;
            pointer.position = currentLevelButton.transform.position + new Vector3(0, 13, 0);
        }

    }

    public void SecondClick() {
        currentLevelButton.LoadThisLevel();
    }

    



}
