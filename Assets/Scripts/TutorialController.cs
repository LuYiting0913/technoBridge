using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    private int step = 0;
    private int l;

    public void Start() {
        l = transform.childCount;
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
        gameObject.SetActive(false);
    }
}
