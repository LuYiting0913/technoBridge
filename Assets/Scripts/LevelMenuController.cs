using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour {
    private static LevelMenuController m_Instance;

    public int ThemeNumber;
    public static int currentThemeNumber;
    private Camera cam;
    public int firstLevel;
    public Transform infoPageParent;
    public Transform pointer;
    private LevelButtonController currentLevelButton;

    private int camMovingSpeed, camMoved;
    public int themeGap = 1000;

    private void Awake() {
        if (m_Instance == null) {
            m_Instance = this;
            // DontDestroyOnLoad(m_Instance);
        } else if (m_Instance != this) {
            Destroy(m_Instance);
        }
    }

    private void Start() {
        Time.timeScale = 1f;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        // Debug.Log("loadded");
        // Debug.Log(currentThemeNumber);
        if(currentThemeNumber == 0) {
            currentThemeNumber = 1;
        } else {
            cam.transform.position -= new Vector3(themeGap, 0, 0) * (currentThemeNumber - 1); 
        }
    }

    // public void StartAtTheme(int theme) {
    //     Debug.Log("received theme update");
    //     currentThemeNumber = theme;
    //     // cam.transform.position -= new Vector3(themeGap, 0, 0) * (currentThemeNumber - 1);
    // }


    public static LevelMenuController GetInstance() {
        return m_Instance;
    }

    public void FirstClick(LevelButtonController clickedButton) {
        if (clickedButton == currentLevelButton) {
            SecondClick();
        } else {
            clickedButton.Rise();
            if (currentLevelButton != null) {
                currentLevelButton.Drop();
                HideLevelInfoPanel(currentLevelButton);
            }
            currentLevelButton = clickedButton;
            Debug.Log(currentLevelButton.transform.position);
            pointer.position = currentLevelButton.transform.position + new Vector3(0, 13, 0);
            ShowLevelInfoPanel(currentLevelButton);
        }

    }

    public void SecondClick() {
        currentLevelButton.LoadThisLevel();
    }

    private void HideLevelInfoPanel(LevelButtonController button) {
        infoPageParent.GetChild(button.level - firstLevel).gameObject.SetActive(false);
    }

    public void ShowLevelInfoPanel(LevelButtonController button) {
        infoPageParent.GetChild(button.level - firstLevel).gameObject.SetActive(true);
        int score = GlobalData.GetLocalData(button.level);
        int star = GlobalData.GetStarLevel(button.level);
        Debug.Log(star);
        Color grey = new Color(0.5f, 0.5f, 0.5f);
        Color yellow = new Color(1, 230f/255f, 30f/255f);
        Transform starParent = infoPageParent.GetChild(button.level - firstLevel).GetChild(3);
        for (int i = 0; i < 3; i++) {
            starParent.GetChild(i).GetComponent<Image>().color = i < star ? yellow : grey;
        }

        infoPageParent.GetChild(button.level - firstLevel).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = 
            "Best Score: $" + score;
 
    }

    public void NextTheme() {
        if (currentThemeNumber < ThemeNumber && camMovingSpeed == 0) {
            camMovingSpeed = -10;
            currentThemeNumber += 1;
        }
    }

    public void PreviousTheme() {
        if (currentThemeNumber > 1  && camMovingSpeed == 0) { 
            camMovingSpeed = 10;
            currentThemeNumber -= 1;
        }
    }

    private void FixedUpdate() {
        if (camMoved < themeGap) {
            cam.transform.position += new Vector3(camMovingSpeed, 0, 0);
            camMoved += camMovingSpeed > 0 ? camMovingSpeed : - camMovingSpeed;
        } else {
            camMovingSpeed = 0;
            camMoved = 0;
        }
    }

    



}
