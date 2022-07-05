using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

    private static Dictionary<string, List<int>> global = new Dictionary<string, List<int>>();

    public static void StoreAllScores(Dictionary<string, List<int>> dic) {
        global = dic;
    }
}
