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
        // List<Vehicle> vehicleToInit = Levels.GetVehicleData(currentLevel);
        // Vector3 temp = Levels.GetBackgroundPosition(currentLevel);
        // scale = Levels.GetBackgroundScale(currentLevel);
        // backgroundPosition = new Vector3(temp.x, temp.y, 0);
        // render all points
        foreach (PointReference p in pointToInit) {
            for (int i = 0; i <= 1; i += 1) {
                // if (p.IsSplit()) {
                //     InstantiateSplit(p, i);
                // } else {
                    InstantiatePoint(p, i, p.IsSplit());
                // }
            }
        }    
        //store in asset manager
        AssetManager.Init(allPoints, new List<SolidBar>());

// use oop here
        foreach (SolidBarReference b in barToInit) {
            if (b.GetMaterial() != 0) {
                // for non-pavement barsm init twice
                if (b.GetMaterial() < 3) {
                    for (int i = 0; i <= 1; i += 1) {
                        allBars.Add(InstantiateBar(b, i, barParent));
                    }
                } else if (b.GetMaterial() < 5) {
                    // ropes
                    for (int i = 0; i <= 1; i += 1) {
                        allBars.Add(InstantiateRope(b, i));
                    }
                } else {
                    for (int i = 0; i <= 1; i += 1) {
                        SolidBar bar = InstantiateBar(b, i, hydraulicParent);
                        bar.GetComponent<HydraulicController>().ConvertToHydraulic(b.GetHydraulicFactor());
                        allBars.Add(bar);
                        allHydraulics.Add(bar.GetComponent<HydraulicController>());
                    }
                }
            } else {
                allPaves.Add(InstantiatePavement(b));
            }           
        }

        AssetManager.Init(allPoints, allBars);
        transform.parent.GetComponent<Stage2Controller>().InitAllDelegates();
    }

    // transfer all the data from 2d UI
    public static void InitScene(int level) {
        currentLevel = level;
        allPoints = new List<Point>();
        allBars = new List<SolidBar>();
    }

    private void InstantiatePoint(PointReference point, int i, bool isSplit) {
        Vector3 pos = point.GetPosition();
        pos.z += i * roadWidth;
        Point scaledTemplate = isSplit ? splitPointTemplate : pointTemplate;
        Transform parent = isSplit ? splitPointParent : pointParent;
        scaledTemplate.transform.localScale = new Vector3(10, 5, 10);
        Point pointInstantiated = Instantiate(scaledTemplate, pos, Quaternion.Euler(90, 0, 0), parent);
        
        if (!isSplit) pointInstantiated.InitRigidBody(point);
        //pointInstantiated.UpdatePosition();

        allPoints.Add(pointInstantiated);
    }

    // private void InstantiateSplit(PointReference point, int i) {
    //     Vector3 pos = point.GetPosition();
    //     pos.z += i * roadWidth;
    //     Point scaledTemplate = splitPointTemplate;
    //     scaledTemplate.transform.localScale = new Vector3(10, 5, 10);
    //     GameObject parentInstantiated = Instantiate(scaledTemplate, pos, Quaternion.Euler(90, 0, 0), splitPointParent).gameObject;    
    //     // pointInstantiated.InitRigidBody(point);

    //     allPoints.Add(parentInstantiated.GetComponent<Point>());

    // }

    private SolidBar InstantiateBar(SolidBarReference bar, int i, Transform parent) {
        Vector3 headPosition = bar.GetHead3D() + new Vector3(0, 0, i * roadWidth);
        Vector3 tailPosition = bar.GetTail3D() + new Vector3(0, 0, i * roadWidth);
        Vector2 dir = bar.GetDirection();
        Vector3 midPoint = (headPosition + tailPosition) / 2;
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        
        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());
        scaledTemplate.transform.localScale = new Vector3(50, dir.magnitude / 2, 50);

        SolidBar newBar = Instantiate(scaledTemplate, midPoint,  
                            Quaternion.Euler(new Vector3(0, 0, angle)), parent).GetComponent<SolidBar>();

        newBar.SetMaterial(bar.GetMaterial());
        newBar.SetBaseColor(newBar.GetComponent<MeshRenderer>().material.color);

        Point head = bar.GetHeadSplit(AssetManager.GetPoint(headPosition));
        Point tail = bar.GetTailSplit(AssetManager.GetPoint(tailPosition));

        newBar.InitTemp(head, tail);
    	// allBars.Add(newBar);
        return newBar;
    }


    private SolidBar InstantiateRope(SolidBarReference bar, int i) {
        GameObject ropeParent = new GameObject("RopeParent");
        ropeParent.transform.parent = barParent;
        SolidBar newRope = ropeParent.AddComponent<SolidBar>();
        
        int maxPerSegment = 20;
        Vector3 headPosition = bar.GetHead3D() + new Vector3(0, 0, i * roadWidth);
        Vector3 tailPosition = bar.GetTail3D() + new Vector3(0, 0, i * roadWidth);
        Vector3 dir = tailPosition - headPosition;
        float l = dir.magnitude;
        int numberOfSegments = (int) (l / maxPerSegment);

        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());
        scaledTemplate.transform.localScale = new Vector3(10, l / numberOfSegments / 1.9f, 10);
        Quaternion rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, bar.GetDirection()));
        
        GameObject previousSegment = AssetManager.GetPoint(headPosition).gameObject;
        for (int j = 1; j <= numberOfSegments; j++) {
            Vector3 tempHead = headPosition + ((float) (j - 1) / numberOfSegments) * dir; 
            Vector3 tempTail = headPosition + ((float) j / numberOfSegments) * dir;
            Vector3 tempPos = (tempHead + tempTail) / 2;
            SolidBar b = Instantiate(scaledTemplate, tempPos, rotation, ropeParent.transform).GetComponent<SolidBar>();
            ConfigurableJoint joint = b.gameObject.GetComponent<ConfigurableJoint>();
            joint.connectedBody = previousSegment.GetComponent<Rigidbody>();
            joint.anchor = new Vector3(0, -0.8f, 0);
            InitRopeJoint(joint); 
            previousSegment = b.gameObject;        
        }
        ConfigurableJoint jt = previousSegment.AddComponent<ConfigurableJoint>();
        jt.connectedBody = AssetManager.GetPoint(tailPosition).gameObject.GetComponent<Rigidbody>();
        jt.anchor = new Vector3(0, 1, 0);
        InitRopeJoint(jt); 

        newRope.SetMaterial(bar.GetMaterial());
        newRope.SetBaseColor(previousSegment.GetComponent<MeshRenderer>().material.color);
        newRope.SetR(AssetManager.GetPoint(headPosition), AssetManager.GetPoint(tailPosition));
    	// allBars.Add(newRope);
        return newRope;
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
        newPave.InitPavementHinge(allPoints, roadWidth);
        // allPaves.Add(newPave);
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
