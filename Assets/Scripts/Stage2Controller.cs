using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Stage2Controller : MonoBehaviour {
    private bool isPaused, displayStress, ended;
    private float playSpeed;
    public GameObject playSpeedSlider;
    public GameObject canvas;
    public Transform vehicleParent;//, hydraulicParent;


    public Transform splitPointParent, hydraulicParent;
    public Transform onlineDistributionBars;
    private Transform pointParent;
    
    // public List<VehicleController> allVehicles = new List<VehicleController>();
    public int currentBatch;
    public bool animating = false;

    private static int totalCost, level, budget;
    public int star = 3;


    public List<VehicleController> VehicleBatch1;  
    public List<VehicleController> VehicleBatch2;  
    public List<VehicleController> VehicleBatch3;  
    public List<VehicleController> VehicleBatch4;  
    private List<List<VehicleController>> batches = new List<List<VehicleController>>();
    
    private float startAnimationTime, animationDuration;
    
    // public AudioManager audioManager;

    public List<Animatable> AnimatableBatch1;
    public List<Animatable> AnimatableBatch2;
    public List<Animatable> AnimatableBatch3;
    public List<Animatable> AnimatableBatch4;
    private List<List<Animatable>> animatableBatches = new List<List<Animatable>>();
    public List<bool> hydraulicAfterAnimation;

    public void Start() {
        playSpeed = 2f;
        currentBatch = 0;
        batches.Add(VehicleBatch1);
        batches.Add(VehicleBatch2);
        batches.Add(VehicleBatch3);
        batches.Add(VehicleBatch4);
        animatableBatches.Add(AnimatableBatch1);
        animatableBatches.Add(AnimatableBatch2);
        animatableBatches.Add(AnimatableBatch3);
        animatableBatches.Add(AnimatableBatch4);
        InitVehicleDelegates();
        pointParent = GameObject.Find("PointParent").transform;
        OnRestarted();
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
        // for (int i = 0; i < vehicleParent.childCount; i++) {
        //     VehicleRestarted += vehicleParent.GetChild(i).GetComponent<VehicleController>().OnRestarted;
        // }
    }

    public static void SetTotalCost(int l, int c, int bud) {
        level = l;
        totalCost = c;
        budget = bud;
    }

    public void SomethingBroken() {
        star = 2;
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
        if (batches[currentBatch].Count == 0) return true;
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
        Debug.Log("start waiting for hydraulics");
        yield return new WaitForSeconds(15);
        Debug.Log("Reconnecting");
        ReconnectPoint(); 
        Debug.Log("playing animation");
        foreach (Animatable a in animatableBatches[currentBatch]) {
            Debug.Log(a);
            a.StartAnimation();
        }
        Debug.Log("start waitng for animation");
        yield return new WaitForSeconds(20);  
        Debug.Log("end waiting");

    	if (hydraulicAfterAnimation[currentBatch]) {
            OnActivated();
            Debug.Log("start waiting for hydraulics after animation");
            yield return new WaitForSeconds(15); 
        }

        currentBatch += 1; 
        InitVehicleDelegates();
        OnRestarted();
        animating = false;
         
    }

    private IEnumerator WaitForAWhile(int sec) {
        yield return new WaitForSeconds(sec);
    }

    private void DisplayPass(int star) {
        Debug.Log("pass");
        GameObject popup = canvas.transform.GetChild(3).gameObject;
        popup.SetActive(true);
        Transform starParent = popup.transform.GetChild(3);
        for (int i = 0; i < 3; i++) {
            starParent.GetChild(i).GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
        }
        for (int i = 0; i < star; i++) {
            starParent.GetChild(i).GetComponent<Image>().color = new Color(1,230f/255,0);
        }
        string s = "";
        if (star == 1) {
            s = "Over Budget and Over 100% Stress!";
        } else if (star == 2) {
            if (totalCost > budget) {
                s = "Over Budget!";
            } else {
                s = "Over 100% Stress!";
            }
        } else {
            s = "Well Done!";
        }
        popup.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = s;
        UpdateDistribution();
    }

    private void UpdateDistribution() {
        int intervalNum = 12;
        int[] countInInterval = new int[intervalNum];
        int maxCount = 0;
        float interval = ((float) budget * 2) / intervalNum;

        List<int> allScores = GlobalData.GetGlobalData(level);
        foreach (int score in allScores) {
            Debug.Log(score);
            int temp = (int) (((float) score) / interval);
            Debug.Log(temp);
            if (temp < intervalNum) {
                countInInterval[temp] += 1;
                if (maxCount < countInInterval[temp]) maxCount = countInInterval[temp];
            }
            
        }

        for (int i = 0; i < 12; i++) {
            float ratio = ((float) countInInterval[i]) / maxCount;
            onlineDistributionBars.GetChild(i).localScale = new Vector3(1, ratio, 1);
        }

        int pos = (int) (((float) totalCost) / budget / 2 * 120 - 60);
        onlineDistributionBars.GetChild(12).localPosition = new Vector3(pos, 0, 0);
        onlineDistributionBars.GetChild(13).GetComponent<TMPro.TextMeshProUGUI>().text = MoneyToString(totalCost);
    }

    private string MoneyToString(int c) {
        string s = "$";
        if (c < 1000) {
            s += c;
        } else {
            s += c / 1000 + ",";
            int r = c % 1000;
            if (r < 100) s += "0";
            if (r < 10) s += "0";
            s += r;
        }
        return s;
    }


    public void Update() {
        if (!isPaused) Time.timeScale = playSpeed;

        if (!ended) {
            if (AllVehicleArrived()) {
                Debug.Log("all arrived");
                Debug.Log(totalCost);
                if (totalCost > budget) {
                    star = star == 2 ? 1 : 2;
                }
                // star = star - (totalCost > budget ? 1 : 0);
                // Levels.UpdateBestScore(level, totalCost, star);
                
                DisplayPass(star);
                GlobalData.AddLocalData(level.ToString(), totalCost, star);
                ended = true;
                
            } else if (AllVehicleWaiting(currentBatch) && !animating) {
                Debug.Log("all waiting");
                OnSplited();
                OnActivated();
                
                // StartCoroutine(WaitForAWhile(5));
                animating = true;
                StartCoroutine(PlayAnimation());
                // OnRestarted();
                // currentBatch += 1;
                
                
            } else if (AnyVehicleFailed()) {
                canvas.transform.GetChild(4).gameObject.SetActive(true);
                
            } 
        }
        // }
         
 
    }
}
