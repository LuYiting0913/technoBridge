using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBar : MonoBehaviour {
    private Vector2 headPosition;
    public SpriteRenderer barRenderer;
    private float maxLength = 100f; 

    public void UpdateSolidBar(Vector2 tailPosition) {
        transform.position = (headPosition + tailPosition) / 2;

        Vector2 dir = tailPosition - headPosition;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        float length = dir.magnitude;
        // not ideal here, change later
        barRenderer.size = new Vector2(length / 10, barRenderer.size.y);
    }

    public void SetHead(Vector2 vector) {
        headPosition = vector;
    }

    public Vector2 GetHead() {
        return headPosition;
    }  

    public Vector2 CutOff(Vector2 cursor) {
        Vector2 offset = cursor - headPosition;
        Debug.Log(maxLength);
        return headPosition + Vector2.ClampMagnitude(offset, maxLength); 
    }

}
