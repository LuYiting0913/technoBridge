using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager {
    private static Dictionary<int, int> materialLength = new Dictionary<int, int>() {
        {0, 200}, // pavement
        {1, 200}, // wood
        {2, 300}  // steel
    };

    private static Dictionary<int, string> materialSprite = new Dictionary<int, string>() {
        {0, "Sprite/PavementSprite"},
        {1, "Sprite/WoodSprite"},
        {2, "Sprite/SteelSprite"}
    };

    private static Dictionary<int, string> materialTemplate3D = new Dictionary<int, string>() {
        {0, "Prefab/tempPavement"},
        {1, "Prefab/tempWoodBar"},
        {2, "Prefab/tempSteelBar"}
    };

    private static Dictionary<int, string> materialTemplate2D = new Dictionary<int, string>() {
        {0, "Prefab/Pavement"},
        {1, "Prefab/WoodBar"},
        {2, "Prefab/SteelBar"}
    };

    private static Dictionary<int, int> materialIntegrity = new Dictionary<int, int>() {
        {0, 4000},
        {1, 200},
        {2, 600}
    };

    public static float GetMaxLength(int material) {
        return materialLength[material];
    }

    public static Sprite GetSprite(int material) {
        return Resources.Load<Sprite>(materialSprite[material]);
    }

    public static GameObject GetTemplate3D(int material) {
        return Resources.Load<GameObject>(materialTemplate3D[material]);
    }

    public static GameObject GetTemplate2D(int material) {
        return Resources.Load<GameObject>(materialTemplate2D[material]);
    }

    public static int GetIntegrity(int material) {
        return materialIntegrity[material];
    }
}