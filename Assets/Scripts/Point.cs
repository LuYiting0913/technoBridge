using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
    public bool isDragging = false;
    public List<SolidBar> connectedBars; 

    public void Update() {
        // if (isDragging) {
        //     Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     transform.position = new Vector3(cursor.x, cursor.y, 0);
        // }
    }
    
}
