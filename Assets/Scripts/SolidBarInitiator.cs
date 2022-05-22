using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolidBarInitiator : MonoBehaviour {
    private static bool startedInit = false;
    public static SolidBar currentBar;
    public static Point beginPoint;
    public static Point endPoint;
    private static int currentMaterial = 1;
    // 0: add bar, 1: select bar

    // public int level;
    private static GameObject barTemplate;
    private static Transform barParent;
    private static GameObject pointTemplate;
    private static Transform pointParent;
    // public LayerMask clickable;

    private static void ClearAll() {
        currentBar = null;
        beginPoint = null;
        endPoint = null;
    }

    public static void InitializeBar(Vector2 headPos, int material, Transform pParent, Transform bParent) {
        ClearAll();

        startedInit = true;
        currentMaterial = material;
        barParent = bParent;
        pointParent = pParent;
        barTemplate = MaterialManager.GetTemplate2D(currentMaterial);
        pointTemplate = Resources.Load<GameObject>("Prefab/Point");
        Debug.Log("init");
        currentBar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
        Vector3 head = new Vector3(headPos.x, headPos.y, 0);
        
        // check if beginPoint already exists
        if (AssetManager.HasPoint(head)) {
            beginPoint = AssetManager.GetPoint(head);
            currentBar.SetHead(beginPoint.GetPosition());
        } else {
            beginPoint = Instantiate(pointTemplate, head, Quaternion.identity, pointParent).GetComponent<Point>();
            AssetManager.AddPoint(beginPoint);
            currentBar.SetHead(head);
        }
        endPoint = Instantiate(pointTemplate, head, Quaternion.identity, pointParent).GetComponent<Point>();
    }

    public static void FinalizeBar(Vector2 tailPos) {
        startedInit = false;
        Vector3 cutOffVector = currentBar.CutOff(new Vector3(tailPos.x, tailPos.y, 0));

        // check if endPoint already exists
        if (AssetManager.HasPoint(cutOffVector)) {
            Destroy(endPoint.gameObject);
            endPoint = AssetManager.GetPoint(cutOffVector);
        } else {
            endPoint.transform.position = cutOffVector; 
            endPoint.UpdatePosition();
            AssetManager.AddPoint(endPoint);
        }

        beginPoint.AddConnectedBar(currentBar);
        endPoint.AddConnectedBar(currentBar);
        currentBar.SetR(beginPoint, endPoint);
        AssetManager.AddBar(currentBar);
        
        // commit changes to the central class
        //Levels.UpdateLevelData(0, AssetManager.GeneratePointReference(), AssetManager.GenerateBarReference());
        // continue to the next bar
        //InitializeBar(endPoint.transform.position);
    }

    private void DeleteBar() {
        startedInit = false;
        Destroy(currentBar.gameObject);
        //AssetManager.DeleteBar(currentBar);
        if (beginPoint.isSingle() && !beginPoint.IsFixed()) {
            Destroy(beginPoint.gameObject);
            AssetManager.DeletePoint(beginPoint);
        }
        if (endPoint.isSingle() && !endPoint.IsFixed()) {
            Destroy(endPoint.gameObject);
            AssetManager.DeletePoint(endPoint);
        }
    }

}
