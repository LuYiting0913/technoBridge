using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBar : MonoBehaviour {
    public Vector2 headPosition;
    public SpriteRenderer barRenderer;

    public void UpdateSolidBar(Vector2 tailPosition) {
        transform.position = (headPosition + tailPosition) / 2;

        Vector2 dir = tailPosition - headPosition;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        float length = dir.magnitude;
        Debug.Log(length);
        // not ideal here, change later
        barRenderer.size = new Vector2(length / 10, barRenderer.size.y);
    }


}
