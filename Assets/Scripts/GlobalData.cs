using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

    private static Dictionary<string, List<int>> global = new Dictionary<string, List<int>>();

    public static void StoreAllScores(Dictionary<string, List<int>> dic) {
        global = dic;
    }

    public static List<int> GetGlobalData(int level) {
        // if (!global.ContainsKey(level.ToString()))
        return new List<int>(){
            19000,
            12222,
            5555,
            11,
            2222,
            6666,
            1000
    
        }; 
        // return global[level.ToString()];
    }
}
