using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitiator : MonoBehaviour {
    public GameObject barTemplate;
    public Point pointTemplate;
    public Transform pointParent;
    public Transform barParent;
    public Transform vehicleParent;
    private int roadWidth = 100;
    private bool displayStress;

    private static int currentLevel;
    private static List<Point> allPoints = new List<Point>();
    private static List<SolidBar> allBars = new List<SolidBar>();
    private static List<Pavement> allPaves = new List<Pavement>();
    private static List<Vehicle> allVehicles = new List<Vehicle>();
    private static Vector3 backgroundPosition;
    private static float scale;

    public void Start() {
        List<PointReference> pointToInit = Levels.GetPointData(currentLevel);
        List<SolidBarReference> barToInit = Levels.GetBarData(currentLevel);
        List<Vehicle> vehicleToInit = Levels.GetVehicleData(currentLevel);
        Vector3 temp = Levels.GetBackgroundPosition(currentLevel);
        scale = Levels.GetBackgroundScale(currentLevel);
        backgroundPosition = new Vector3(temp.x, temp.y, 0);
        // render all points
        foreach (PointReference p in pointToInit) {
            for (int i = 0; i <= 1; i += 1) {
                InstantiatePoint(p, i);
            }
        }    
        //store in asset manager
        AssetManager.Init(allPoints, new List<SolidBar>());

        foreach (SolidBarReference b in barToInit) {
            if (b.GetMaterial() != 0) {
                // for non-pavement barsm init twice
                if (b.GetMaterial() < 3) {
                    for (int i = 0; i <= 1; i += 1) {
                        InstantiateBar(b, i);
                    }
                } else {
                    // ropes
                    for (int i = 0; i <= 1; i += 1) {
                        InstantiateRope(b, i);
                    }
                }
            } else {
                InstantiatePavement(b);
            }
        }

        AssetManager.Init(allPoints, allBars);

        foreach (Vehicle vehicle in vehicleToInit) {
            GameObject vehicleObject = vehicle.GetVehicle();
            VehicleController controller = Instantiate(vehicleObject, vehicle.GetPosition(), 
                                            Quaternion.Euler(new Vector3(0, 90, 0)), vehicleParent).GetComponent<VehicleController>();
            controller.SetCheckpoint(vehicle.GetCheckpoint());                                
        }
    }

    // transfer all the data from 2d UI
    public static void InitScene(int level) {
        currentLevel = level;
        allPoints = new List<Point>();
        allBars = new List<SolidBar>();
    }

    private void InstantiatePoint(PointReference point, int i) {
        Vector3 pos = point.GetPosition();
        pos.z += i * roadWidth;
        Point scaledTemplate = pointTemplate;
        scaledTemplate.transform.localScale = new Vector3(10, 5, 10);
        Point pointInstantiated = Instantiate(scaledTemplate, pos, Quaternion.Euler(90, 0, 0), pointParent);
        
        pointInstantiated.InitRigidBody(point);
        //pointInstantiated.UpdatePosition();

        allPoints.Add(pointInstantiated);
    }

    private void InstantiateBar(SolidBarReference bar, int i) {
        Vector3 headPosition = bar.GetHead3D() + new Vector3(0, 0, i * roadWidth);
        Vector3 tailPosition = bar.GetTail3D() + new Vector3(0, 0, i * roadWidth);
        Vector2 dir = bar.GetDirection();
        Vector3 midPoint = (headPosition + tailPosition) / 2;
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        
        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());
        scaledTemplate.transform.localScale = new Vector3(50, dir.magnitude / 2, 50);

        SolidBar newBar = Instantiate(scaledTemplate, midPoint, 
                            Quaternion.Euler(new Vector3(0, 0, angle)), barParent).GetComponent<SolidBar>();

        newBar.SetMaterial(bar.GetMaterial());
        newBar.SetBaseColor(newBar.GetComponent<MeshRenderer>().material.color);
        newBar.InitTemp(AssetManager.GetPoint(headPosition), AssetManager.GetPoint(tailPosition));
    	allBars.Add(newBar);
    }

    private void InstantiateRope(SolidBarReference bar, int i) {
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
            joint.anchor = new Vector3(0, -1, 0);
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
    	allBars.Add(newRope);
    }

    private void InstantiatePavement(SolidBarReference bar) {
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
        allPaves.Add(newPave);
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
                    // Transform piece1 = bar.transform.GetChild(0);
                    // Transform piece2 = bar.transform.GetChild(1);

                    // piece1.gameObject.SetActive(true);
                    // piece2.gameObject.SetActive(true);
                    // piece1.SetParent(barParent, true);
                    // piece2.SetParent(barParent, true);
                    // piece1.GetComponent<SolidBar>().InitTemp(bar.head, null);
                    // piece2.GetComponent<SolidBar>().InitTemp(null, bar.tail);
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
                    // Transform piece1 = bar.transform.GetChild(0);
                    // Transform piece2 = bar.transform.GetChild(1);

                    // piece1.gameObject.SetActive(true);
                    // piece2.gameObject.SetActive(true);
                    // piece1.SetParent(barParent, true);
                    // piece2.SetParent(barParent, true);
                    // piece1.GetComponent<SolidBar>().InitTemp(bar.head, null);
                    // piece2.GetComponent<SolidBar>().InitTemp(null, bar.tail);
                    pave.DisablePave();
                } else if (displayStress) {
                    // pave.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = pave.GetLoadColor();
                    pave.DisplayStress();
                } else {
                    // pave.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = pave.GetBaseColor();
                    pave.DisplayNormal();
                }
            }
        }
    }

    private Vector3 WorldToCanvas(Vector3 pos) {
        return (pos - backgroundPosition) / scale;
    }

}
