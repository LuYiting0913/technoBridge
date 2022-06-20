using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEffect : MonoBehaviour {
    private float transparency;
    private Color baseColor;
    private SpriteRenderer renderer;
    public int counter;

    public void Start() {
        renderer = GetComponent<SpriteRenderer>();
        baseColor = renderer.material.color;
        transparency = baseColor.a;
    }

    public void FixedUpdate() {
        counter += 1;
        if (counter <= 50) {
            renderer.material.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * counter / 50f);
        } else if (counter <= 100) {
            renderer.material.color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * (2 - counter / 50f));
        } else {
            counter = 0;
        }
    }

}
