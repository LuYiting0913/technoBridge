using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitiator : MonoBehaviour {
    public GameObject barTemplate;
    public GameObject pointTemplate;
    public Transform pointParent;
    public Transform barParent;

    private static List<SolidBar3D> bar = new List<SolidBar3D>();
    //private static List<Point3D> point = new List<Point3D>();

    public static List<Vector2> pointPos = new List<Vector2>();
    public static List<SolidBar> bar2D = new List<SolidBar>();
    private static float scaleFactor = 10f;

    public void Awake() {
        //point.Clear();
        //bar.Clear();
    }

    public void Start() {
        Debug.Log(pointPos.Count);
        // init components
        foreach (Vector2 v in pointPos) {
            Vector3 pos = new Vector3(v.x, v.y, 0);
            GameObject scaledTemplate = pointTemplate;
            scaledTemplate.transform.localScale = new Vector3(2, 2, 2);

            Rigidbody point = Instantiate(scaledTemplate, pos, Quaternion.identity, pointParent).GetComponent<Rigidbody>();
            
            //point.isKinemtic = PointManager.GetPoint(v).isFixed();
            Point3DManager.AddPoint(pos, point);
            
        }    
        
        foreach (SolidBar b in bar2D) {
            Vector3 headPos = new Vector3(b.GetHead().x, b.GetHead().y, 0);
            Vector3 tailPos = new Vector3(b.GetTail().x, b.GetTail().y, 0);

            // calculate position and rotation
            Vector3 midPoint = (headPos + tailPos) / 2;
            Vector2 dir = (headPos - tailPos);
            GameObject scaledTemplate = barTemplate;
            scaledTemplate.transform.localScale = new Vector3(1, dir.magnitude / 2, 1);
            float angle = Vector2.SignedAngle(Vector2.up, dir);

            SolidBar3D newBar = Instantiate(scaledTemplate, midPoint, 
                                            Quaternion.Euler(new Vector3(0, 0, angle)), barParent).
                                            GetComponent<SolidBar3D>();

            newBar.InitSolidBar(Point3DManager.GetPoint(headPos), Point3DManager.GetPoint(tailPos));
            bar.Add(newBar);
        }
    }

    // transfer all the data from 2d UI
    public static void InitScene(List<Vector2> points, List<SolidBar> bars) {
        bar2D = bars;
        foreach (Vector2 vec in points) {
            pointPos.Add(vec / scaleFactor);
        }
        foreach (SolidBar b in bar2D) {
            b.SetHead(b.GetHead() / scaleFactor);
            b.SetTail(b.GetTail() / scaleFactor);
        }
    }

}
