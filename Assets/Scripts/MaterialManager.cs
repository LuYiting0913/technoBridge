using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager {
    private static Dictionary<int, int> materialLength = new Dictionary<int, int>() {
        {1, 200},//wood
        {2, 300} // steel
    };

    private static Dictionary<int, string> materialSprite = new Dictionary<int, string>() {
        {1, "Sprite/WoodSprite"},
        {2, "Sprite/SteelSprite"}
    };

    private static Dictionary<int, string> materialTemplate3D = new Dictionary<int, string>() {
        {1, "Prefab/tempWoodBar"},
        {2, "Prefab/tempSteelBar"}
    };

    private static Dictionary<int, string> materialTemplate2D = new Dictionary<int, string>() {
        {1, "Prefab/WoodBar"},
        {2, "Prefab/SteelBar"}
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
}
