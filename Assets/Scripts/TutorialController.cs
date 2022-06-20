using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public List<Point> allPoints;
    private Dictionary<Point, bool> isCovered = new Dictionary<Point, bool>();
    private int step = 0;
    private int l;

    public void Start() {
        l = transform.childCount;
        foreach (Point p in allPoints) {
            if (p != null) isCovered.Add(p, false);
        }

        // for (int i = 0; i < l; i++) {
        //     transform.GetChild(i).gameObject.SetActive(i == step);
        // }
        // DontDestroyOnLoad(this.gameObject);
    }

    public void NextStep() {
        step += 1;
        if (step < l) {
            ShowStep(step);
        } else {
            Close();
        }
    }

    private void ShowStep(int i) {
        transform.GetChild(i - 1).gameObject.SetActive(false);
        transform.GetChild(i).gameObject.SetActive(true);
    }

    public void Close() {
        transform.GetChild(l - 1).gameObject.SetActive(false);
    }

    public Point FindGuidePoint(Vector3 pos) {
        // Debug.Log(pos);
        foreach (Point p in allPoints) {
            // Debug.Log(p.GetPosition());
            if (p.Contain(pos)) {
                CoverPoint(p);
                Debug.Log(AllPointsCovered());
                Debug.Log("chhecked all guide pts");
                return p;
            } 
        }
        return null;
    }

    public void CoverPoint(Point p) {
        isCovered[p] = true;
    }

    public bool AllPointsCovered() {
        foreach (Point p in allPoints) {
            if (!isCovered[p]) return false;
        }
        return true;  
    }


}
