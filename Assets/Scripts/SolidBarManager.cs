using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBarManager : MonoBehaviour {
    private static List<SolidBar> allBars = new List<SolidBar>();

    public static void AddBar(SolidBar bar) {
        allBars.Add(bar);
    }

    public static List<SolidBar> GetAll() {
        return allBars;
    }

    public static List<SolidBar> GetAllStartAt(Vector2 v) {
        List<SolidBar> result = new List<SolidBar>();
        foreach (SolidBar bar in allBars) {
            if (bar.GetHead().x == v.x && bar.GetHead().y == v.y) {
                result.Add(bar);
            }
        }
        return result;
    }

}
