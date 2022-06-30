using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitiator : MonoBehaviour {
    public GameObject barTemplate;
    public Point pointTemplate;
    public Point splitPointTemplate;
    public Transform pointParent, barParent, vehicleParent, hydraulicParent;
    public Transform splitPointParent;


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
                    if (b.GetMaterial() == 5) {
                        newBar.GetComponent<HydraulicController>().ConvertToHydraulic(b.GetHydraulicFactor());
                        allHydraulics.Add(newBar.GetComponent<HydraulicController>());
                    }
                    allBars.Add(newBar);
                }
            } else {
                allPaves.Add(InstantiatePavement(b));
            }           
        }

        AssetManager.Init(allPoints, allBars);
        transform.parent.GetComponent<Stage2Controller>().InitAllDelegates();
    }

    public static void InitScene(int level) {
        currentLevel = level;
        allPoints = new List<Point>();
        allBars = new List<SolidBar>();
    }


    private Pavement InstantiatePavement(SolidBarReference bar) {
        Vector3 headPosition = bar.GetHead3D();
        Vector3 tailPosition = bar.GetTail3D();
        Vector2 dir = bar.GetDirection();
        Vector3 midPoint = (headPosition + tailPosition) / 2 + new Vector3(0, 0, roadWidth / 2);
        float angle = Vector2.SignedAngle(Vector2.up, dir);      
        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());
        //....
        scaledTemplate.transform.localScale = new Vector3(75, dir.magnitude, 330);

        Pavement newPave = Instantiate(scaledTemplate, midPoint, 
                                        Quaternion.Euler(new Vector3(0, 0, angle)), barParent).
                                        GetComponent<Pavement>();

        newPave.SetPosition(headPosition, tailPosition);
        newPave.InitPavementHinge(bar, roadWidth);

        return newPave;
    }

    private void InitRopeJoint(ConfigurableJoint joint) {
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.axis = new Vector3(0, 0, 1); 
    }

    public void ToggleStressDisplay() {
        displayStress = !displayStress;
    }

    public void Update() {
        foreach (SolidBar bar in allBars) {
            if (bar != null && !bar.disabled) {
                if (bar.GetCurrentLoad() >= 1) {
                    Transform piece1 = bar.transform.GetChild(0);
                    Transform piece2 = bar.transform.GetChild(1);
                    ActivateBrokenPiece(piece1);
                    ActivateBrokenPiece(piece2);
                    bar.DisableBar();
                } else if (displayStress) {
                    bar.DisplayStress();
                } else {
                    bar.DisplayNormal();
                }
            }

        }

        foreach (Pavement pave in allPaves) {
            if (pave != null && !pave.disabled) {
                if (pave.GetCurrentLoad() >= 1) {
                    Transform piece1 = pave.transform.GetChild(2);
                    Transform piece2 = pave.transform.GetChild(3);
                    ActivateBrokenPiece(piece1);
                    ActivateBrokenPiece(piece2);
                    pave.DisablePave();
                } else if (displayStress) {
                    pave.DisplayStress();
                } else {
                    pave.DisplayNormal();
                }
            }
        }
    }

    private void ActivateBrokenPiece(Transform piece) {
        piece.gameObject.SetActive(true);
        piece.SetParent(barParent, true);
    }

    // public static void ActivateAllHydraulics() {
    //     foreach (HydraulicController hydraulic in allHydraulics) hydraulic.Activate();
    // }

    // public static void

}
