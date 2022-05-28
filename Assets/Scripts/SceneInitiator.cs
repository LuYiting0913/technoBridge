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
    // public static List<PointReference>  pointToInit = new List<PointReference>();
    // public static List<SolidBarReference> barToInit = new List<SolidBarReference>();
    // public static List<Vehicle> vehicleToInit = new List<Vehicle>();

    private static int currentLevel;
    private static List<Point> allPoints = new List<Point>();
    private static List<SolidBar> allBars = new List<SolidBar>();
    private static List<Vehicle> allVehicles = new List<Vehicle>();
    private static float scaleFactor = 10f;

    public void Start() {
        List<PointReference> pointToInit = Levels.GetPointData(currentLevel);
        List<SolidBarReference> barToInit = Levels.GetBarData(currentLevel);
        List<Vehicle> vehicleToInit = Levels.GetVehicleData(currentLevel);

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
                for (int i = 0; i <= 1; i += 1) {
                    InstantiateBar(b, i);
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
        scaledTemplate.transform.localScale = new Vector3(10, 10, 10);
        Point pointInstantiated = Instantiate(scaledTemplate, pos, Quaternion.identity, pointParent);
        
        pointInstantiated.InitRigidBody(point);
        pointInstantiated.UpdatePosition();

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
        newBar.InitBarHead(AssetManager.GetPoint(headPosition));
        newBar.InitBarTail(AssetManager.GetPoint(tailPosition));
    	allBars.Add(newBar);
    }

    private void InstantiatePavement(SolidBarReference bar) {
        Vector3 headPosition = bar.GetHead3D();
        Vector3 tailPosition = bar.GetTail3D();
        Vector2 dir = bar.GetDirection();
        Vector3 midPoint = (headPosition + tailPosition) / 2 + new Vector3(0, 0, roadWidth / 2);
        float angle = Vector2.SignedAngle(Vector2.up, dir);      
        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());
        //....
        scaledTemplate.transform.localScale = new Vector3(75, dir.magnitude, 300);

        Pavement newPave = Instantiate(scaledTemplate, midPoint, 
                                        Quaternion.Euler(new Vector3(0, 0, angle)), barParent).
                                        GetComponent<Pavement>();

        newPave.SetPosition(headPosition, tailPosition);
        newPave.InitPavementHinge(allPoints, roadWidth);
    }

    public void Update() {
        foreach (SolidBar bar in allBars) {
            Debug.Log(bar.GetCurrentLoad());
            if (bar.GetCurrentLoad() >= 1) {
                Debug.Log(bar.transform.GetChild(0).transform.position);
                //bar.gameObject.SetActive(false);
                bar.transform.GetChild(0).gameObject.SetActive(true);
                bar.transform.GetChild(1).gameObject.SetActive(true);
                // Destroy(bar.transform.gameObject);
            } 
            Color currentColor = bar.GetComponent<MeshRenderer>().material.color;
            bar.GetComponent<MeshRenderer>().material.color = bar.GetLoadColor(currentColor);

        }
    }

}
