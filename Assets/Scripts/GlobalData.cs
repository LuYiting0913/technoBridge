using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

    private static Dictionary<string, List<int>> global = new Dictionary<string, List<int>>();

    private static Dictionary<string, int> local = new Dictionary<string, int>();

    public static void StoreAllGlobalScores(Dictionary<string, List<int>> dic) {
        global = dic;
    }


    public static void StoreAllLocalScores(Dictionary<string, int> dic) {
        foreach (string key in dic.Keys) {
            Debug.Log(key);
        }
    }

    public static List<int> GetGlobalData(int level) {
        if (!global.ContainsKey(level.ToString())) {
            return new List<int>(){
                19000,
                12222,
                5555,
                11,
                2222,
                6666,
                1000,
                22000,
                35000,
                30000
        
            };
        }
        return global[level.ToString()];
    }

    public static int GetLocalData(int level) {
        if (!local.ContainsKey(level.ToString())) return 0;
        return local[level.ToString()];
        // return 0;

    }

    public static void AddLocalData(string level, int score) {
        Debug.Log(level);
        Debug.Log(score);
        local[level] = score;
    }
}
