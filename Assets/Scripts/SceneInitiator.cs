using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitiator : MonoBehaviour {
    public GameObject barTemplate;
    public GameObject pointTemplate;
    public Transform pointParent;
    public Transform barParent;
    public static List<Vector2> pointPos = new List<Vector2>();
    public static List<SolidBar> bar = new List<SolidBar>();
    private static float scaleFactor = 10f;

    public void Awake() {
        //point.Clear();
        //bar.Clear();
    }

    public void Start() {
        Debug.Log(pointPos.Count);
        foreach (Vector2 v in pointPos) {
            Vector3 pos = new Vector3(v.x, v.y, 0);
            Instantiate(pointTemplate, pos, Quaternion.identity, pointParent);
        }
        foreach (SolidBar b in bar) {
            Vector2 midPoint = (b.GetHead() + b.GetTail()) / 2;
            Vector2 dir = (b.GetTail() - b.GetHead());
            GameObject scaledTemplate = barTemplate;
            scaledTemplate.transform.localScale = new Vector3(1, dir.magnitude / 2, 1);
            float angle = Vector2.SignedAngle(Vector2.up, dir);

            Instantiate(scaledTemplate, new Vector3(midPoint.x, midPoint.y, 0), 
                        Quaternion.Euler(new Vector3(0, 0, angle)), barParent);
        }
    }

    public static void InitScene(List<Vector2> points, List<SolidBar> bars) {
        bar = bars;
        foreach (Vector2 vec in pointPos) {
            pointPos.Add(vec / scaleFactor);
            Debug.Log(vec);
        }
        foreach (SolidBar b in bar) {
            b.SetHead(b.GetHead() / scaleFactor);
            b.SetTail(b.GetTail() / scaleFactor);
            Debug.Log(b);
        }
    }

    // private void RenderScene() {
    //     foreach (Point pt in point) {
    //         Vector3 pos = new Vector3(pt.transform.position.x, pt.transform.position.y, 0);
    //         Instantiate(pointTemplate, pos, Quaternion.identity, pointParent);
    //     }
    // }
}
