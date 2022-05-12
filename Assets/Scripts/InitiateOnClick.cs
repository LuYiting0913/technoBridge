using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InitiateOnClick : MonoBehaviour {
    
    [SerializeField] private GameObject newObject;

    private bool isDragging = false;

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            if (!isDragging) {
                Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Instantiate(newObject, new Vector3(cursor.x, cursor.y, 0), Quaternion.identity, transform);  
            } else {
                
            }
        }
            
    }
}
