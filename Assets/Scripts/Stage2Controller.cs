using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class Stage2Controller : MonoBehaviour {
    private bool isPaused, displayStress;
    private float playSpeed;
    public GameObject playSpeedSlider;
    public GameObject canvas;
    public Transform vehicleParent;//, hydraulicParent;
    public Transform splitPointParent, hydraulicParent;
    // public List<VehicleController> allVehicles = new List<VehicleController>();

    public void Start() {
        playSpeed = 2f;
    }

    public delegate void HydraulicEventHandler(object source, Stage2Controller e);
    public event HydraulicEventHandler Activated;
    public event HydraulicEventHandler Splited;


    protected virtual void OnActivated() {
        if (Activated != null) Activated(this, this);
    }

    protected virtual void OnSplited() {
        if (Splited != null) Splited(this, this);
    }


    public void InitAllDelegates() {
        for (int i = 0; i < splitPointParent.childCount; i++) {
            Splited += splitPointParent.GetChild(i).GetComponent<SplitPointController>().OnSplited;
        }
        for (int i = 0; i < hydraulicParent.childCount; i++) {
            Activated += hydraulicParent.GetChild(i).GetComponent<HydraulicController>().OnActivated;
        }
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
            if (!vehicleParent.GetChild(i).GetComponent<VehicleController>().Arrived()) return false; 
        }
        return true;
    }

    private bool AllVehicleWaiting() {
        for (int i = 0; i < vehicleParent.childCount; i++) {
            if (!vehicleParent.GetChild(i).GetComponent<VehicleController>().IsWaiting()) return false; 
        }
        return true;
    }

    // To see if any vehicle failed
    private bool AnyVehicleFailed() {
        for (int i = 0; i < vehicleParent.childCount; i++) {
            if (vehicleParent.GetChild(i).GetComponent<VehicleController>().Failed()) return true; 
        }
        return false;
    }


    public void Update() {
        if (!isPaused) Time.timeScale = playSpeed;
                    // Debug.Log(AllVehicleWaiting());
        if (AllVehicleWaiting()) {
            // SceneInitiator.ActivateAllHydraulics();
            OnSplited();
            OnActivated();
        } else if (AnyVehicleFailed()) {
            canvas.transform.GetChild(4).gameObject.SetActive(true);
        } else if (AllVehicleArrived()) {
            // Debug.Log("all arrived");
            canvas.transform.GetChild(3).gameObject.SetActive(true);
        }
    }
}
