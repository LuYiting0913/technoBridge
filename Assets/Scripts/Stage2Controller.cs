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
    private Transform pointParent;
    // public List<VehicleController> allVehicles = new List<VehicleController>();

    public List<VehicleController> VehicleBatch1;  
    public List<VehicleController> VehicleBatch2;  
    // public List<VehicleController> VehicleBatch3;  
    // public List<VehicleController> VehicleBatch4;  
    private List<List<VehicleController>> batches = new List<List<VehicleController>>();
    public int currentBatch;
    private float startAnimationTime, animationDuration;
    private bool animating = false;
    // public AudioManager audioManager;

    public List<Animatable> AnimatableBatch1;
    public List<Animatable> AnimatableBatch2;
    private List<List<Animatable>> animatableBatches = new List<List<Animatable>>();

    public void Start() {
        playSpeed = 2f;
        currentBatch = 0;
        batches.Add(VehicleBatch1);
        batches.Add(VehicleBatch2);
        // batches.Add(VehicleBatch3);
        // batches.Add(VehicleBatch4);
        animatableBatches.Add(AnimatableBatch1);
        animatableBatches.Add(AnimatableBatch2);
        pointParent = GameObject.Find("PointParent").transform;
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

    private void InitVehicleDelegates() {
        VehicleRestarted = null;
        foreach (VehicleController vc in batches[currentBatch]) VehicleRestarted += vc.OnRestarted;
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
        // if (GameObject.Find("Boat") != null && !GameObject.Find("Boat").GetComponent<Animatable>().Arrived()) {
        //     return false;
        // }
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
            Debug.Log("Current vehicle is " + vc.gameObject.name + " in batch " + batch);
            vc.Restart();
        }
    }

    private bool AnyVehicleFailed() {
        for (int i = 0; i < vehicleParent.childCount; i++) {
            if (vehicleParent.GetChild(i).GetComponent<VehicleController>().Failed()) return true; 
        }
        return false;
    }
    
    private void ReconnectPoint() {
        int threshold = 10;
        Debug.Log("reconnecting");
        for (int i = 0; i < pointParent.childCount; i++) {
            Transform splitPoint = pointParent.GetChild(i);
            for (int j = 0; j < pointParent.childCount; j++) {
                Transform point = pointParent.GetChild(j);
                if (i != j && (splitPoint.position - point.position).magnitude < threshold) {
                    Debug.Log("reconnected");
                    FixedJoint joint = splitPoint.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = point.GetComponent<Rigidbody>();
                    splitPoint.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
                }
            }
        }
    }

    private IEnumerator PlayAnimation() {
        Debug.Log("enter");
        yield return new WaitForSeconds(8);
        Debug.Log("Reconnecting");
        ReconnectPoint(); 
        Debug.Log("playing animation");
        foreach (Animatable a in animatableBatches[currentBatch]) {
            Debug.Log(a);
            a.StartAnimation();
        }
        Debug.Log("start waitng");
        yield return new WaitForSeconds(10);  
        Debug.Log("end waiting");
        currentBatch += 1; 
        InitVehicleDelegates();
        OnRestarted();
         
    }

    private IEnumerator WaitForAWhile(int sec) {
        yield return new WaitForSeconds(sec);
    }


    public void Update() {
        if (!isPaused) Time.timeScale = playSpeed;
        // if (!Animating() && ) Debug.Log("animna");
        // if(!Animating()) { 
        // if (currentBatch > 1 || batches[currentBatch].Count == 0) {
        if (AllVehicleArrived()) {
            Debug.Log("all arrived");
            canvas.transform.GetChild(3).gameObject.SetActive(true);
        } else if (AllVehicleWaiting(currentBatch) && !animating) {
            Debug.Log("all waiting");
            OnSplited();
            OnActivated();
            
            // StartCoroutine(WaitForAWhile(5));
            animating = true;
            StartCoroutine(PlayAnimation());
            // OnRestarted();
            
            
        } else if (AnyVehicleFailed()) {
            canvas.transform.GetChild(4).gameObject.SetActive(true);
        } 
        // }
         
 
    }
}
