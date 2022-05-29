using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager {
    private static string point2D = "Prefab/Point";
    private static string fixedPoint2D = "Prefab/FixedPoint";


    public static GameObject GetPoint2DTemplate() {
        return Resources.Load<GameObject>(point2D);
    } 

    public static GameObject GetFixedPoint2DTemplate() {
        return Resources.Load<GameObject>(fixedPoint2D);
    } 
}
