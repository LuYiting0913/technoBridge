using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HydraulicInitiator : MonoBehaviour {
    private static HydraulicInitiator m_Instance;
    private bool isActive = true;

    private Vector2 start, end;
    private SolidBar hydraulic;
    private bool isModifying = false;
    private Transform slider;
    private float l, limit;
    private int duration = 2;
    
    private void Awake() {
        if (m_Instance == null) {
            m_Instance = this;
            //DontDestroyOnLoad(m_Instance);
        } else if (m_Instance != this) {
            Destroy(m_Instance);
        }
    }

    public void Start() {

    }

    public static HydraulicInitiator GetInstance() {
        return m_Instance;
    }
     
    public void OnModeChanged(object source, int i) {
        isActive = i != 0;
    }

    public void OnPressed(object source, Stage1Controller e) {
        RaycastHit2D hit = Physics2D.Raycast(e.GetStartPoint(), new Vector3(0, 0, 1));
        if (isActive && hit.collider != null) {
            
            if (hit.transform.gameObject.GetComponent<HydraulicController>() != null) {
                slider = hit.transform;
                hydraulic = hit.transform.parent.gameObject.GetComponent<SolidBar>();
                limit = hit.transform.parent.gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
                limit /= hit.transform.parent.localScale.x;
                isModifying = true;
            } else if (hit.transform.gameObject.GetComponent<Point>() != null) {
                DoubleClick clickedPoint = hit.transform.gameObject.GetComponent<DoubleClick>();
                // Debug.Log("chckking double");
                if (clickedPoint.RegisterClick()) {
                    // Debug.Log("regied 2 click");
                    clickedPoint.GetComponent<SplitPointController>().ToggleSplit();
                }
            } else if (hit.transform.gameObject.GetComponent<SplitBarController>() != null) {
                RaycastHit2D[] hits = Physics2D.RaycastAll(e.GetStartPoint(), new Vector3(0, 0, 1), 30);
                foreach (RaycastHit2D h in hits) {
                    SplitBarController splitBar = h.transform.gameObject.GetComponent<SplitBarController>();
                    if (splitBar != null) {
                        Debug.Log(h.transform.gameObject);
                        splitBar.ToggleSplit();
                    }
                }
                
                
            }
        }

    }

    public void OnReleased(object source, Stage1Controller e) {
        if (isActive) isModifying = false;
 
    }
    
    public void OnDragged(object source, Stage1Controller e) {
        if (isActive && isModifying) {
            AssetManager.DeleteBar(hydraulic);
            // Vector2 dir = (e.GetCurPoint() - e.GetStartPoint()) / 200;
            slider.transform.position = e.GetCurPoint();
            float position;
            // Debug.Log(slider.transform.localPosition);
            if (slider.localPosition.x > limit) {
                position = limit;
            } else if (slider.localPosition.x < - limit) {
                position = - limit;
            } else {
                position = slider.localPosition.x;
            }

            slider.localPosition = new Vector3(position, slider.localPosition.z, slider.localPosition.z);
            float factor = Math.Abs(position) / limit + 0.5f;
            
            transform.GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = GetString(factor);
            if (!transform.GetChild(0).gameObject.activeSelf) {
                StartCoroutine(ShowForAWhile(transform.GetChild(0).gameObject, duration));
            }
            hydraulic.SetHydraulicFactor(factor);
            // 0.5 - 1: contract, 1 - 1.5 extend
            
            AssetManager.AddBar(hydraulic);
        }
    }

    private IEnumerator ShowForAWhile(GameObject message, int dur) {
        message.SetActive(true);
        yield return new WaitForSeconds(dur);
        message.SetActive(false);
    }

    private string GetString(float f) {
        if (f <= 1) {
            return "Contract " + (int) ((1 - f) * 100) + "%";
        } else {
            return "Extend " + (int) ((f - 1) * 100) + "%";
        }
    }


}
