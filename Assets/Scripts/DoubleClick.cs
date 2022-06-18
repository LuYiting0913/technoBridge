using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClick : MonoBehaviour {
    public float firstClickTime, clickInterval;
    public int clickCount;
    public bool doubleClicked = false;

    public void Start() {
        clickCount = 0;
        clickInterval = 0.3f;      
    }

    public bool RegisterClick() {
        if (clickCount == 0) {
            RegisterFirstClick();
        } else {
            RegisterSecondClick();
        }

        return doubleClicked;
    }

    private void RegisterFirstClick() {
        firstClickTime = Time.time;
        clickCount = 1;
        doubleClicked = false;
    }

    private void RegisterSecondClick() {
        doubleClicked = Time.time < firstClickTime + clickInterval;
        if (!doubleClicked) {
            RegisterFirstClick();
        } else {
            clickCount = 0;
            firstClickTime = 0f;
        }

    }
}
