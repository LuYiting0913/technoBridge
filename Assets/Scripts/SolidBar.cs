using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBar : MonoBehaviour {
    private Vector2 headPosition;
    private Vector2 tailPosition;
    public SpriteRenderer barRenderer;
    private float maxLength = 200f; 

    public void UpdateSolidBar(Vector2 tailPos) {
        tailPosition = tailPos;
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

    public void SetTail(Vector2 vector) {
        tailPosition = vector;
    }

    public Vector2 GetHead() {
        return headPosition;
    }  

    public Vector2 GetTail() {
        return tailPosition;
    }

    public Vector2 CutOff(Vector2 cursor) {
        Vector2 offset = cursor - headPosition;
        return headPosition + Vector2.ClampMagnitude(offset, maxLength); 
    }

}
