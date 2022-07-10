using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

    public HydraulicController[] hydraulics;
    public Animatable animatable;
    public VehicleController vehicle;
    public Transform assetParent, leaderBoard;

    public int vehicleDriveTime = 3;
    public int hydraulicTime = 3;
    public int animatableTime = 3;

    private bool inAction = false;

    private void Start() {
        foreach (Transform invisibleRoad in transform.GetChild(3)) {
            invisibleRoad.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            invisibleRoad.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        }

        for (int i = 0; i < assetParent.GetChild(0).childCount; i++) {
            
            SolidBar bar = assetParent.GetChild(0).GetChild(i).GetComponent<SolidBar>();
            if (bar.GetMaterial() == 2 || bar.GetMaterial() == 5) {
                Vector3 headPosition = bar.head.transform.position;
                Vector3 tailPosition = bar.tail.transform.position;
                Vector2 dir = new Vector2(tailPosition.x - headPosition.x, tailPosition.y - headPosition.y);
                float angle = Vector2.SignedAngle(Vector2.up, dir);
                bar.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                bar.transform.position = (headPosition + tailPosition) / 2;
                bar.transform.localScale = new Vector3(50, dir.magnitude / 2, 50);
                bar.InitBarHead();
                bar.InitBarTail();
                // SolidBar.Instantiate3D(bar.head, bar.tail, bar.GetMaterial(), assetParent.GetChild(0));

                if (bar.GetMaterial() == 5) {
                    bar.GetComponent<HydraulicController>().ConvertToHydraulic(0.6f);
                }
            }

            
        }
    }

    private void ResetVehicle() {
        vehicle.transform.position = new Vector3(484, 377.4f,386.4f);
        vehicle.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        vehicle.InitVehicleStatus();
        animatable.transform.position = new Vector3(948, 217.75f, 1080);

    }

    private IEnumerator PlayAnimation() {
        inAction = true;
        animatable.animating = false;
        vehicle.Restart();
        Debug.Log("vehicle start");
        yield return new WaitForSeconds(vehicleDriveTime);
        // vehicle.Stop();
        foreach (HydraulicController hydraulic in hydraulics) {
            hydraulic.Activate();
        }
        Debug.Log("hydraulic activated");
        yield return new WaitForSeconds(hydraulicTime);
        animatable.StartAnimation();
        Debug.Log("animatable start");
        yield return new WaitForSeconds(animatableTime);
        animatable.EndAnimation();
        foreach (HydraulicController hydraulic in hydraulics) {
            hydraulic.Activate();
        }
        Debug.Log("hydraulic activated");
        yield return new WaitForSeconds(hydraulicTime);
        inAction = false;
        
    }

    private void FixedUpdate() {
        Time.timeScale = 3f;
        if (!inAction) {
            ResetVehicle();
            StartCoroutine(PlayAnimation());
        }
    }

    public void DisplayLeaderBoard() {
        int totalLevels = 30;
        int numOnPodium = 2;
        Transform nameParent = leaderBoard.GetChild(0);
        Transform scoreParent = leaderBoard.GetChild(1);
        Dictionary<string, int> levelCompleted = GlobalData.GetLevelCompleted();
        // Dictionary<string, int> sorted = new Dictionary<string, int>();
        int curNumOnPodium = 0;

        foreach (string s in levelCompleted.Keys) {
            Debug.Log(s);
            Debug.Log(levelCompleted[s]);
        }

        for (int i = totalLevels; i > 0; i--) {
            if (curNumOnPodium < numOnPodium) {
                foreach (string name in levelCompleted.Keys) {
                    if (curNumOnPodium < numOnPodium && levelCompleted[name] == i) {
                        nameParent.GetChild(curNumOnPodium).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = name;
                        scoreParent.GetChild(curNumOnPodium).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = i.ToString();
                        curNumOnPodium += 1;
                    } 
                }

            }
            
        }
        // List<string> names = new List<string>();
        // foreach (string s in levelCompleted.Keys) {
        //     names.Add(s);
        //     Debug.Log(s);
        // }

        // for (int i = 0; i < nameParent.childCount; i++) {
        //     nameParent.GetChild(i).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = names[i];
        //     scoreParent.GetChild(i).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = levelCompleted[names[i]].ToString();
        // }
    }
}
