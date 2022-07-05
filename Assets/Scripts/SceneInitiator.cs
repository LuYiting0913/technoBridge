using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneInitiator : MonoBehaviour {
    public GameObject barTemplate;
    public Point pointTemplate;
    public Point splitPointTemplate;
    public Transform pointParent, barParent, vehicleParent, hydraulicParent;
    public Transform splitPointParent;
    public AudioManager audioManager;
    public Transform stressPercentageDisplay;


    private int roadWidth = 100;
    private bool displayStress;

    private static int currentLevel;
    private static List<Point> allPoints = new List<Point>();
    private static List<SolidBar> allBars = new List<SolidBar>();
    private static List<HydraulicController> allHydraulics = new List<HydraulicController>();
    private static List<Pavement> allPaves = new List<Pavement>();
    private static List<Vehicle> allVehicles = new List<Vehicle>();
    // private static Vector3 backgroundPosition;
    private static float scale;

    public void Start() {
        List<PointReference> pointToInit = Levels.GetPointData(currentLevel);
        List<SolidBarReference> barToInit = Levels.GetBarData(currentLevel);

        foreach (PointReference p in pointToInit) {
            for (int i = 0; i <= 1; i += 1) {
                allPoints.Add(Point.Instantiate3D(p, i * roadWidth, pointParent, splitPointParent));
            }
        }    
        AssetManager.Init(allPoints, new List<SolidBar>());


        foreach (SolidBarReference b in barToInit) {
            if (b.GetMaterial() != 0) {
                for (int i = 0; i <= 1; i += 1) {
                    SolidBar newBar = SolidBar.Instantiate3D(b, i * roadWidth, barParent, hydraulicParent);
                    if (newBar.GetMaterial() == 5) {
                        newBar.GetComponent<HydraulicController>().ConvertToHydraulic(b.GetHydraulicFactor());
                        allHydraulics.Add(newBar.GetComponent<HydraulicController>());
                    }
                    allBars.Add(newBar);
                }
            } else {
                allPaves.Add(Pavement.Instantiate3DPavement(b, roadWidth, barParent));
            }           
        }

        AssetManager.Init(allPoints, allBars);
        transform.parent.GetComponent<Stage2Controller>().InitAllDelegates();
    }

    public static void InitScene(int level) {
        currentLevel = level;
        allPoints = new List<Point>();
        allBars = new List<SolidBar>();
        // cost = totalCost;
    }

    private void UpdateStressPercentage(float f) {
        Transform ring = stressPercentageDisplay.GetChild(0).GetChild(0);
        Transform pointer = stressPercentageDisplay.GetChild(0).GetChild(1);
        Transform numberDisplay = stressPercentageDisplay.GetChild(1).GetChild(0);
        float angle = 90f - f * 180;
        Color color = GetLoadColor(f);
        ring.GetComponent<Image>().color = color;
        pointer.rotation = Quaternion.Euler(0, 0, angle);
        numberDisplay.GetComponent<TMPro.TextMeshProUGUI>().text = LoadToString(f);
    }

    private Color GetLoadColor(float load) {
        if (load < 0.5) {
            return new Color(load * 2, 1, 0);
        } else if (load < 1) {
            return new Color(1, 2 - load * 2, 0);
        } else {
            return new Color(1, 0, 0);
        }
    }

    private string LoadToString(float load) {
        int i = (int) (load * 100);
        int j = ((int) (load * 1000)) % 10;
        return i + "." + j + "%";
    }

    public void ToggleStressDisplay() {
        displayStress = !displayStress;
    }

    public void Update() {
        float highestLoad = 0f;
        foreach (SolidBar bar in allBars) {
            if (bar != null && !bar.disabled) {
                float load = bar.GetCurrentLoad();
                if (load > highestLoad) highestLoad = load;
                if (load >= 1) {
                    bar.Break();
                    audioManager.PlayBreakSound();
                } else if (displayStress) {
                    bar.DisplayStress();
                } else {
                    bar.DisplayNormal();
                }
            }

        }

        foreach (Pavement pave in allPaves) {
            if (pave != null && !pave.disabled) {
                float load = pave.GetCurrentLoad();
                if (load > highestLoad) highestLoad = load;
                if (load >= 1) {
                    pave.Break();
                    audioManager.PlayBreakSound();
                } else if (displayStress) {
                    pave.DisplayStress();
                } else {
                    pave.DisplayNormal();
                }
            }
        }

        if (highestLoad >= 1) {
            transform.parent.GetComponent<Stage2Controller>().SomethingBroken();
        }
        UpdateStressPercentage(highestLoad);
    }



}
