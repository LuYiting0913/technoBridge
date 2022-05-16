using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBarReference {
    private Vector2 headPosition;
    private Vector2 tailPosition;
    //public int type;
    private float maxLength = 200f; 

    public static SolidBarReference of(SolidBar bar) {
        SolidBarReference reference = new SolidBarReference();
        reference.SetReference(bar.GetHead(), bar.GetTail());
        return reference;
    }


    public void SetReference(Vector2 head, Vector2 tail) {
        headPosition = head;
        tailPosition = tail;
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

    public Vector3 GetHead3D() {
        return new Vector3(headPosition.x, headPosition.y, 0);
    }

    public Vector2 GetTail() {
        return tailPosition;
    }

    public Vector3 GetTail3D() {
        return new Vector3(tailPosition.x, tailPosition.y, 0);
    }

    public Vector2 GetDirection() {
        return new Vector2(tailPosition.x - headPosition.x, tailPosition.y - headPosition.y);
    }
}
