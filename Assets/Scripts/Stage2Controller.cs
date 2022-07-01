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

    public List<VehicleController> VehicleBatch1;  
    public List<VehicleController> VehicleBatch2;  
    public List<VehicleController> VehicleBatch3;  
    public List<VehicleController> VehicleBatch4;  
    private List<List<VehicleController>> batches = new List<List<VehicleController>>();
    private int currentBatch;

    public void Start() {
        playSpeed = 2f;
        currentBatch = 0;
        batches.Add(VehicleBatch1);
        batches.Add(VehicleBatch2);
        batches.Add(VehicleBatch3);
        batches.Add(VehicleBatch4);
    }

    public delegate void HydraulicEventHandler(object source, Stage2Controller e);
    public event HydraulicEventHandler Activated;
    public event HydraulicEventHandler Splited;
    public event HydraulicEventHandler VehicleRestarted;


    protected virtual void OnActivated() {
        if (Activated != null) Activated(this, this);
    }

    protected virtual void OnSplited() {
        if (Splited != null) Splited(this, this);
    }

    protected virtual void OnRestarted() {
        if (VehicleRestarted != null) VehicleRestarted(this, this);
    }


    public void InitAllDelegates() {
        for (int i = 0; i < splitPointParent.childCount; i++) {
            Splited += splitPointParent.GetChild(i).GetComponent<SplitPointController>().OnSplited;
        }
        for (int i = 0; i < hydraulicParent.childCount; i++) {
            Activated += hydraulicParent.GetChild(i).GetComponent<HydraulicController>().OnActivated;
        }
        for (int i = 0; i < vehicleParent.childCount; i++) {
            VehicleRestarted += vehicleParent.GetChild(i).GetComponent<VehicleController>().OnRestarted;
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

    private bool AllVehicleWaiting(int batch) {
        foreach (VehicleController vc in batches[batch]) {
            if (!vc.IsWaiting()) return false; 
        }
        return true;
    }

    private void StartBatch(int batch) {
        foreach (VehicleController vc in batches[batch]) {
            vc.Restart();
        }
    }

    // To see if any vehicle failed
    private bool AnyVehicleFailed() {
        for (int i = 0; i < vehicleParent.childCount; i++) {
            if (vehicleParent.GetChild(i).GetComponent<VehicleController>().Failed()) return true; 
        }
        return false;
    }

    // private void PlayAnimation() {
        
    //     StartCoroutine(WaitForAWhile(3));
    // }

    private IEnumerator PlayAnimation() {
        Debug.Log("playing animation");
        yield return new WaitForSeconds(5);
        OnRestarted();
    }


    public void Update() {
        if (!isPaused) Time.timeScale = playSpeed;

        if (currentBatch > 3 || batches[currentBatch].Count == 0) {
            Debug.Log("all arrived");
            canvas.transform.GetChild(3).gameObject.SetActive(true);
        } else if (AllVehicleWaiting(currentBatch)) {
            Debug.Log("all waiting");
            OnSplited();
            OnActivated();
            // StartCoroutine(WaitForAWhile(3));
            StartCoroutine(PlayAnimation());
            currentBatch += 1;
            StartBatch(currentBatch);
        } else if (AnyVehicleFailed()) {
            canvas.transform.GetChild(4).gameObject.SetActive(true);
        }          
 
    }
}
