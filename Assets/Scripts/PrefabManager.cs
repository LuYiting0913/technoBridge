using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager {
    private static string point2D = "Prefab/Object2D/Point";
    private static string fixedPoint2D = "Prefab/Object2D/FixedPoint";
    private static string splitPoint2D = "Prefab/Object2D/SplitPoint";
    private static string gridLine = "Prefab/GridLine";

    private static string point3D = "Prefab/Object3D/Point3D";
    // private static string fixedPoint3D = "Prefab/Object3D/FixedPoint";
    private static string splitPoint3D = "Prefab/Object3D/SplitPoint3D";

    private static string splitePointSprite = "Sprite/Object2D/SplitPoint";
    private static string pointSprite = "Sprite/Object2D/Point";



    public static GameObject GetPoint2DTemplate() {
        return Resources.Load<GameObject>(point2D);
    } 

    public static GameObject GetFixedPoint2DTemplate() {
        return Resources.Load<GameObject>(fixedPoint2D);
    } 

    public static GameObject GetSplitPoint2DTemplate() {
        return Resources.Load<GameObject>(splitPoint2D);
    } 

    public static GameObject GetPoint3DTemplate() {
        return Resources.Load<GameObject>(point3D);
    } 

    public static GameObject GetSplitPoint3DTemplate() {
        return Resources.Load<GameObject>(splitPoint3D);
    } 

    public static GameObject GetGridLine() {
        return Resources.Load<GameObject>(gridLine);
    }

    public static Sprite GetSplitPointSprite() {
        return Resources.Load<Sprite>(splitePointSprite);
    }

    public static Sprite GetPointSprite() {
        return Resources.Load<Sprite>(pointSprite);
    }

    
}
