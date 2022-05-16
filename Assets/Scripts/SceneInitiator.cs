using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitiator : MonoBehaviour {
    public SolidBar barTemplate;
    public Point pointTemplate;
    public Transform pointParent;
    public Transform barParent;

    //private static List<SolidBar3D> bar = new List<SolidBar3D>();

    public static List<PointReference>  pointToInit = new List<PointReference>();
    public static List<SolidBarReference> barToInit = new List<SolidBarReference>();
    private static float scaleFactor = 10f;

    public void Awake() {
    }

    public void Start() {
        // temp stored all the points in a dictionary
        List<Point> allPoints = new List<Point>();
        // render all points
        foreach (PointReference p in pointToInit) {
            Debug.Log(p.GetPosition());
            Vector3 pos = p.GetPosition();
            Point scaledTemplate = pointTemplate;
            scaledTemplate.transform.localScale = new Vector3(10, 10, 10);

            Point pointInstantiated = Instantiate(scaledTemplate, pos, Quaternion.identity, pointParent);
            Rigidbody pointRb = pointInstantiated.GetComponent<Rigidbody>();
            pointRb.isKinematic = p.IsFixed();
            pointInstantiated.UpdatePosition();
            allPoints.Add(pointInstantiated);
        }    

        foreach (SolidBarReference b in barToInit) {
            Vector3 headPos = b.GetHead3D();
            Vector3 tailPos = b.GetTail3D();
            Vector2 dir = b.GetDirection();
            Vector3 midPoint = (headPos + tailPos) / 2;
            float angle = Vector2.SignedAngle(Vector2.up, dir);
            
            SolidBar scaledTemplate = barTemplate;
            scaledTemplate.transform.localScale = new Vector3(5, dir.magnitude / 2, 5);

            SolidBar newBar = Instantiate(scaledTemplate, midPoint, 
                                            Quaternion.Euler(new Vector3(0, 0, angle)), barParent).
                                            GetComponent<SolidBar>();

            // find head and tail point, not very efficient
            foreach (Point p in allPoints) {
                if (p.Contain(headPos)) {
                    newBar.InitBarHead(p);
                }
                if (p.Contain(tailPos)) {
                    newBar.InitBarTail(p);
                }
            }
            // newBar.SetAnchors();
        }
    }

    // transfer all the data from 2d UI
    public static void InitScene(int level) {
        pointToInit = Levels.GetPointData(level);
        barToInit = Levels.GetBarData(level);
        // foreach (Point p in pointToInit) {
        //     p.transform.position.x /= scaleFactor;
        //     p.transform.position.y /= scaleFactor;
        // }
        // foreach (SolidBar b in barToInit) {
        //     b.SetHead(b.GetHead() / scaleFactor);
        //     b.SetTail(b.GetTail() / scaleFactor);
        // }
    }

}
