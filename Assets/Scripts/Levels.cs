using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels {
    private static Dictionary<int, List<PointReference>> pointData = new Dictionary<int, List<PointReference>>();
    private static Dictionary<int, List<SolidBarReference>> barData = new Dictionary<int, List<SolidBarReference>>();

    public static List<PointReference> GetPointData(int level) {
        return pointData[level];
    }

    public static List<SolidBarReference> GetBarData(int level) {
        return barData[level];
    }

    public static void UpdateLevelData(int level, List<PointReference> newPoints, List<SolidBarReference> newBars) {
        Debug.Log("Updated to levels");
        pointData[level] = newPoints;
        barData[level] = newBars;
    }
}
