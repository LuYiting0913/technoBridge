using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager {
    private static Dictionary<int, int> materialLength = new Dictionary<int, int>() {
        {0, 100}, // pavement
        {1, 100}, // wood
        {2, 170}, // steel
        {3, 1000} // rope
    };

    private static Dictionary<int, string> materialSprite = new Dictionary<int, string>() {
        {0, "Sprite/PavementSprite"},
        {1, "Sprite/WoodSprite"},
        {2, "Sprite/SteelSprite"},
        {3, "Sprite/WoodSprite"}
    };

    private static Dictionary<int, string> materialTemplate3D = new Dictionary<int, string>() {
        {0, "Prefab/tempPavement"},
        {1, "Prefab/tempWoodBar"},
        {2, "Prefab/tempSteelBar"},
        {3, "prefab/tempRope"}
    };

    private static Dictionary<int, string> materialTemplate2D = new Dictionary<int, string>() {
        {0, "Prefab/Pavement"},
        {1, "Prefab/WoodBar"},
        {2, "Prefab/SteelBar"},
        {3, "Prefab/Rope"}
    };

    private static Dictionary<int, int> materialIntegrity = new Dictionary<int, int>() {
        {0, 3000},
        {1, 5000},
        {2, 9000},
        {3, 7000}
    };

    private static Dictionary<int, float> materialCostPerUnitLength = new Dictionary<int, float>() {
        {0, 1f},
        {1, 0.7f},
        {2, 2f},
        {3, 0.5f}
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

    public static float GetMaterialCost(int material) {
        return materialCostPerUnitLength[material];
    }
}
