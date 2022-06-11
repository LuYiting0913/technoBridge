using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage2Controller : MonoBehaviour {
    private bool isPaused, displayStress;
    private float playSpeed;
    public GameObject playSpeedSlider;
    public GameObject canvas;
    public Transform vehicleParent;

    public void Start() {
        playSpeed = 2f;
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
        playSpeed = s.value * 2;
        int percentage = (int) (playSpeed * 50);
        s.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = percentage + "%";
    }

    private bool AllVehicleArrived() {
        for (int i = 0; i < vehicleParent.childCount; i++) {
            if (!vehicleParent.GetChild(i).GetComponent<VehicleController>().arrived) return false; 
        }
        return true;
    }

    public void Update() {
        if (!isPaused) Time.timeScale = playSpeed;
        if (AllVehicleArrived()) {
            Debug.Log("all arrived");
            canvas.transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}
