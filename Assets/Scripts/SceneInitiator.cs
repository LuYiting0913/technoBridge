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

    //private static List<SolidBar3D> bar = new List<SolidBar3D>();

    public static List<PointReference>  pointToInit = new List<PointReference>();
    public static List<SolidBarReference> barToInit = new List<SolidBarReference>();
    public static List<Vehicle> vehicleToInit = new List<Vehicle>();

    private static List<Point> allPoints = new List<Point>();
    private static List<SolidBar> allBars = new List<SolidBar>();
    private static List<Vehicle> allVehicles = new List<Vehicle>();
    private static float scaleFactor = 10f;

    public void Start() {
        // render all points
        foreach (PointReference p in pointToInit) {
            for (int i = 0; i <= 1; i += 1) {
                Vector3 pos = p.GetPosition();
                pos.z += i * roadWidth;
                Point scaledTemplate = pointTemplate;
                scaledTemplate.transform.localScale = new Vector3(10, 10, 10);
                Point pointInstantiated = Instantiate(scaledTemplate, pos, Quaternion.identity, pointParent);
                
                pointInstantiated.InitRigidBody(p);
                pointInstantiated.UpdatePosition();

                allPoints.Add(pointInstantiated);
            }
        }    
        //store in asset manager
        AssetManager.Init(allPoints, new List<SolidBar>());

        foreach (SolidBarReference b in barToInit) {
            Debug.Log(b.GetMaterial());
            if (b.GetMaterial() != 0) {
                // for non-pavement barsm init twice
                for (int i = 0; i <= 1; i += 1) {
                    Vector3 headPos = b.GetHead3D() + new Vector3(0, 0, i * roadWidth);
                    Vector3 tailPos = b.GetTail3D() + new Vector3(0, 0, i * roadWidth);
                    Vector2 dir = b.GetDirection();
                    Vector3 midPoint = (headPos + tailPos) / 2;
                    float angle = Vector2.SignedAngle(Vector2.up, dir);
                    
                    GameObject scaledTemplate = MaterialManager.GetTemplate3D(b.GetMaterial());
                    scaledTemplate.transform.localScale = new Vector3(50, dir.magnitude / 2, 50);

                    SolidBar newBar = Instantiate(scaledTemplate, midPoint, 
                                                    Quaternion.Euler(new Vector3(0, 0, angle)), barParent).
                                                    GetComponent<SolidBar>();

                    newBar.SetMaterial(b.GetMaterial());
                    newBar.InitBarHead(AssetManager.GetPoint(headPos));
                    newBar.InitBarTail(AssetManager.GetPoint(tailPos));

                    allBars.Add(newBar);
                }
            } else {
                // for pavements
                Vector3 headPos = b.GetHead3D();
                Vector3 tailPos = b.GetTail3D();
                Vector2 dir = b.GetDirection();
                Vector3 midPoint = (headPos + tailPos) / 2 + new Vector3(0, 0, roadWidth / 2);
                float angle = Vector2.SignedAngle(Vector2.up, dir);      
                GameObject scaledTemplate = MaterialManager.GetTemplate3D(b.GetMaterial());
                //....
                scaledTemplate.transform.localScale = new Vector3(75, dir.magnitude, 300);

                Pavement newPave = Instantiate(scaledTemplate, midPoint, 
                                                Quaternion.Euler(new Vector3(0, 0, angle)), barParent).
                                                GetComponent<Pavement>();

                newPave.SetPosition(headPos, tailPos);
                newPave.InitPavementHinge(allPoints, roadWidth);
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
        pointToInit = Levels.GetPointData(level);
        barToInit = Levels.GetBarData(level);
        vehicleToInit = Levels.GetVehicleData(level);
        allPoints = new List<Point>();
        allBars = new List<SolidBar>();
    }

    public void Update() {
        foreach (SolidBar bar in allBars) {
            Debug.Log(bar.GetCurrentLoad());
            Color currentColor = bar.GetComponent<MeshRenderer>().material.color;
            bar.GetComponent<MeshRenderer>().material.color = bar.GetLoadColor(currentColor);
        }
    }

}
