using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraulicInitiator : MonoBehaviour {
    private static HydraulicInitiator m_Instance;

    private Vector2 start, end;
    private HydraulicController hydraulic;
    private bool isModifying = false;
    private Transform slider;
    private float l, limit;
    
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
        // isActive = i == 1;
    }

    public void OnPressed(object source, Stage1Controller e) {
        RaycastHit2D hit = Physics2D.Raycast(e.startPoint, new Vector3(0, 0, 1));
        if (hit.collider != null && hit.transform.gameObject.GetComponent<HydraulicController>() != null) {
            slider = hit.transform;
            
            limit = hit.transform.parent.gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
            limit /= hit.transform.parent.localScale.x;
            isModifying = true;
            Debug.Log(limit);
        }

    }

    public void OnReleased(object source, Stage1Controller e) {
        isModifying = false;
 
    }
    
    public void OnDragged(object source, Stage1Controller e) {
        if (isModifying) {
            // Vector2 dir = (e.curPoint - e.startPoint) / 200;
            slider.transform.position = e.curPoint;
            // Debug.Log(start);
            // Debug.Log(e.curPoint);
            Debug.Log(slider.transform.localPosition);
            if (slider.localPosition.x > limit) {
                slider.localPosition = new Vector3(limit, slider.localPosition.z, slider.localPosition.z);
            } else if (slider.localPosition.x < - limit) {
                slider.localPosition = new Vector3(-limit, slider.localPosition.z, slider.localPosition.z);
            } else {
                slider.localPosition = new Vector3(slider.localPosition.x, slider.localPosition.z, slider.localPosition.z);
            }
        }
    }

}
