using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitPointController : MonoBehaviour {
    public bool isSplitPoint = false;
    // private Sprite normalPoint = Resources.Load("")

    public void ToggleSplit() {
        InitSplit(!isSplitPoint);
        GetComponent<Point>().isSplit = isSplitPoint;
        GetComponent<Point>().SetSplit();
    }

    public void InitSplit(bool b) {
        isSplitPoint = b;
        if (b) {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            if (!GetComponent<Point>().IsFixed()) {                
                GetComponent<SpriteRenderer>().sprite = PrefabManager.GetSplitPointSprite();
            } else {
                Debug.Log(PrefabManager.GetSplitFixedPointSprite());
                GetComponent<SpriteRenderer>().sprite = PrefabManager.GetSplitFixedPointSprite();
            }
        } else {
            if (!GetComponent<Point>().IsFixed()) {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);
                GetComponent<SpriteRenderer>().sprite = PrefabManager.GetPointSprite();
            } else {
                GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                GetComponent<SpriteRenderer>().sprite = PrefabManager.GetFixedPointSprite();
            }
        }
    }

    public void OnSplited(object source, Stage2Controller e) {
        // Debug.Log("recieve split");
        Destroy(transform.GetChild(0).GetComponent<FixedJoint>());
    }

}
