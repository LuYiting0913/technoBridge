using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager {
    private static string point2D = "Prefab/Object2D/Point";
    private static string fixedPoint2D = "Prefab/Object2D/FixedPoint";
    private static string gridLine = "Prefab/GridLine";


    public static GameObject GetPoint2DTemplate() {
        return Resources.Load<GameObject>(point2D);
    } 

    public static GameObject GetFixedPoint2DTemplate() {
        return Resources.Load<GameObject>(fixedPoint2D);
    } 

    public static GameObject GetGridLine() {
        return Resources.Load<GameObject>(gridLine);
    }
}
