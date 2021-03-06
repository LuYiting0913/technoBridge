using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Levels {
    public static string currentUserName;

    private static Dictionary<int, List<PointReference>> pointData = new Dictionary<int, List<PointReference>>();
    private static Dictionary<int, List<SolidBarReference>> barData = new Dictionary<int, List<SolidBarReference>>();
    private static Dictionary<int, List<Vehicle>> vehicleData = new Dictionary<int, List<Vehicle>>();
    private static Dictionary<int, Vector3> backgroundPosition = new Dictionary<int, Vector3>();
    private static Dictionary<int, float> backgroundScale = new Dictionary<int, float>();
    
    private static Dictionary<int, int> bestScore = new Dictionary<int, int>();
    private static Dictionary<int, int> star = new Dictionary<int, int>();
    private static bool[] isInited = new bool[31];
   
    public Dictionary<string, int> bestScoreIns = new Dictionary<string, int>();


    public static void ClearLevel(int level) {
        pointData[level] = new List<PointReference>();
        barData[level] = new List<SolidBarReference>();
    }
    
    public static void InitLevel(int level) {
        if (!isInited[level]) {
            switch (level)
            {
                case 0:
                    Level0.InitLevel();
                    break;
                case 1:
                    Level1.InitLevel();
                    break;
                case 2:
                    Level2.InitLevel();
                    break;
                case 3:
                    Level3.InitLevel();
                    break;
                case 4:
                    Level4.InitLevel();
                    break;
                case 5:
                    Level5.InitLevel();
                    break;
                case 6:
                    Level6.InitLevel();
                    break;
                case 7:
                    Level7.InitLevel();
                    break;
                case 8:
                    Level8.InitLevel();
                    break;
                case 9:
                    Level9.InitLevel();
                    break;
                case 10:
                    Level10.InitLevel();
                    break;
                case 11:
                    Level11.InitLevel();
                    break;
                case 12:
                    Level12.InitLevel();
                    break;
                case 13:
                    Level13.InitLevel();
                    break;
                case 14:
                    Level14.InitLevel();
                    break;
                case 15:
                    Level15.InitLevel();
                    break;
                case 16:
                    Level16.InitLevel();
                    break;
                case 17:
                    Level17.InitLevel();
                    break;
                case 18:
                    Level18.InitLevel();
                    break;
                case 19:
                    Level19.InitLevel();
                    break;
                case 20:
                    Level20.InitLevel();
                    break;
                case 21:
                    Level21.InitLevel();
                    break;
                case 22:
                    Level22.InitLevel();
                    break;
                case 23:
                    Level23.InitLevel();
                    break;
                case 24:
                    Level24.InitLevel();
                    break;
                case 25:
                    Level25.InitLevel();
                    break;
                case 26:
                    Level26.InitLevel();
                    break;
                case 27:
                    Level27.InitLevel();
                    break;
                case 28:
                    Level28.InitLevel();
                    break;
                case 29:
                    Level29.InitLevel();
                    break;
                case 30:
                    Level30.InitLevel();
                    break;
                default:
                    break;
            }
            isInited[level] = true;
        }
    }

    public static bool IsLevelEdited(int level) {
	    return pointData.ContainsKey(level);
    }
    
    public static List<PointReference> GetPointData(int level) {
        return pointData[level];
    }

    public static List<SolidBarReference> GetBarData(int level) {
        return barData[level];
    }

    public static void UpdateLevelData(int level, List<PointReference> newPoints, List<SolidBarReference> newBars) {
        Debug.Log("Updated to levels");
        isInited[level] = true;
        pointData[level] = newPoints;
        barData[level] = newBars;
    }

    public static void SetVehicleData(int level, List<Vehicle> vehicles) {
        vehicleData[level] = vehicles;
    }

    public static List<Vehicle> GetVehicleData(int level) {
        return vehicleData[level];
    }

    public static bool IsInited(int level) {
        return isInited[level];
    }

    public static void UpdateBackground(int level, Vector3 v, float f) {
        backgroundPosition[level] = v;
        backgroundScale[level] = f;
    }

    public static void SetBackgroundPostion(int level, Vector3 v) {
        backgroundPosition[level] = v;
    }

    public static Vector3 GetBackgroundPosition(int level) {
        return backgroundPosition.ContainsKey(level) ? backgroundPosition[level] : new Vector3(0, 0, 105);
    }

    public static float GetBackgroundScale(int level) {
        return backgroundScale.ContainsKey(level) ? backgroundScale[level] : 1f;
    }

    // public static void SetBudget(int level, int b) {
    //     budget[level] = b;
    // }

    // public static int GetBudget(int level) {
    //     return budget[level];
    // }

    public static void UpdateBestScore(int level, int score, int starLevel) {
        if (bestScore.ContainsKey(level)) {
            if (bestScore[level] > score) bestScore[level] = score;
            // if (star[level] > starLevel) star[level]
        } else {
            bestScore[level] = score;
        }

        if (star.ContainsKey(level)) {
            if (star[level] > starLevel) star[level] = starLevel;
            // if (star[level] > starLevel) star[level]
        } else {
            star[level] = starLevel;
        }
         
    }

    // public static int GetStarLevel(int level) {
    //     if (star.ContainsKey(level)) {
    //         return star[level];
    //     } else {
    //         Debug.Log(false);
    //         return 0;
    //     }
    // }

    public static Dictionary<string, int> GetAllBestScores() {
        Dictionary<string, int> s = new Dictionary<string, int>();
        for (int i = 1; i < 50; i++) {
            // if (p.ContainsKey(i)) pointDataIns[i.ToString()] = p[i];
            // if (b.ContainsKey(i)) barDataIns[i.ToString()] = b[i];
            if (bestScore.ContainsKey(i)) s[i.ToString()] = bestScore[i];
        }
        return s;

    }

    public static int GetNumOfLevelCompleted() {
        return GetAllBestScores().Count;
    }

}
