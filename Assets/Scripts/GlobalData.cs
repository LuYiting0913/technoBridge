using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour {

    private static Dictionary<string, List<int>> global = new Dictionary<string, List<int>>();

    private static Dictionary<string, int> local = new Dictionary<string, int>();

    private static Dictionary<string, int> levelCompleted = new Dictionary<string, int>();

    private static Dictionary<int, List<string>> map = new Dictionary<int, List<string>>();

    public static void StoreAllGlobalScores(Dictionary<string, List<int>> dic) {
        global = dic;
    }


    public static void StoreAllLocalScores(Dictionary<string, int> dic) {
        foreach (string key in dic.Keys) {
            // Debug.Log(key);
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
        local[level] = score;
    }

    public static void AddLevelCompleted(string name, int num) {
        levelCompleted[name] = num;
    }

    public static int GetNumOfLevelCompleted() {
        return local.Count;
    }

    public static void IncrementPlayer(string name) {
        Debug.Log("increment");
        Debug.Log(name);
        if (!levelCompleted.ContainsKey(name)) {
            levelCompleted[name] = 1;
        } else {
            levelCompleted[name] += 1;
        }

    }

    public static Dictionary<string, int> GetLevelCompleted() {
        // foreach (string name in levelCompleted.Keys) {
        //     Debug.Log(name);
        //     Debug.Log(levelCompleted[name]);
        // }
        // Debug.Log("before sending");
        return levelCompleted;
    }

}
