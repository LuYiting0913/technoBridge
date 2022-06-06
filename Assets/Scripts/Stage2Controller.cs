using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage2Controller : MonoBehaviour {
    private bool isPaused;
    private float playSpeed;
    public GameObject playSpeedSlider;

    public void Start() {
        playSpeed = 1f;
    }

    public void TogglePause() {
        if (isPaused) {
            Resume();
        } else {
            Pause();
        }
    }

    public void Pause() {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void Resume() {
        isPaused = false;
        Time.timeScale = playSpeed; 
    }

    public void UpdatePlaySpeed() {
        Slider s = playSpeedSlider.GetComponent<Slider>();
        playSpeed = s.value;
        Debug.Log(s.value);
        int percentage = (int) (playSpeed * 100);
        s.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = percentage + "%";
    }

    public void Update() {
        if (!isPaused) Time.timeScale = playSpeed;
    }
}
