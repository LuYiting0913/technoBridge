using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager {
    private static Dictionary<int, int> materialLength = new Dictionary<int, int>() {
        {0, 100}, // pavement
        {1, 100}, // wood
        {2, 170}, // steel
        {3, 1000},// rope
        {4, 1000},// cable
        {5, 170} // hydraulics
    };

    private static Dictionary<int, string> materialSprite = new Dictionary<int, string>() {
        {0, "Sprite/Object2D/PavementSprite"},
        {1, "Sprite/Object2D/WoodSprite"},
        {2, "Sprite/Object2D/SteelSprite"},
        {3, "Sprite/Object2D/RopeSprite"},
        {4, "Sprite/Object2D/CableSprite"},
        {5, "Sprite/Object2D/HydraulicSprite"}
    };

    private static Dictionary<int, string> materialTemplate3D = new Dictionary<int, string>() {
        {0, "Prefab/Object3D/tempPavement"},
        {1, "Prefab/Object3D/tempWoodBar"},
        {2, "Prefab/Object3D/tempSteelBar"},
        {3, "prefab/Object3D/tempRope"},
        {4, "prefab/Object3D/tempCable"},
        {5, "Prefab/Object3D/tempHydraulic"}
    };

    private static Dictionary<int, string> materialTemplate2D = new Dictionary<int, string>() {
        {0, "Prefab/Object2D/Pavement"},
        {1, "Prefab/Object2D/WoodBar"},
        {2, "Prefab/Object2D/SteelBar"},
        {3, "Prefab/Object2D/Rope"},
        {4, "Prefab/Object2D/Cable"}, 
        {5, "Prefab/Object2D/Hydraulic"}
    };

    private static Dictionary<int, int> materialIntegrity = new Dictionary<int, int>() {
        {0, 3000},
        {1, 5000},
        {2, 9000},
        {3, 1000},
        {4, 1500},
        {5, 10000}
    };

    private static Dictionary<int, float> materialCostPerUnitLength = new Dictionary<int, float>() {
        {0, 1f},
        {1, 0.7f},
        {2, 2f},
        {3, 0.5f},
        {4, 1f},
        {5, 3f}
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
