using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour {
    private static int duration = 1;

    public void ToggleSprite() {
        if (IsOn()) {
            ToggleOff().SetActive(true);
            ToggleOn().SetActive(false);
            StartCoroutine(ShowForAWhile(ToggleOffMessage(), duration));
        } else {
            ToggleOff().SetActive(false);
            ToggleOn().SetActive(true);
            StartCoroutine(ShowForAWhile(ToggleOnMessage(), duration));
        }
    }

    private bool IsOn() {
        return transform.GetChild(0).gameObject.activeSelf;
    }

    private GameObject ToggleOn() {
        return transform.GetChild(0).gameObject;
    }

    private GameObject ToggleOff() {
        return transform.GetChild(1).gameObject;
    }

    private GameObject ToggleOnMessage() {
        return transform.GetChild(2).gameObject;
    }

    private GameObject ToggleOffMessage() {
        return transform.GetChild(3).gameObject;
    }

    private IEnumerator ShowForAWhile(GameObject message, int dur) {
        message.SetActive(true);
        yield return new WaitForSeconds(dur);
        message.SetActive(false);
    }
}
