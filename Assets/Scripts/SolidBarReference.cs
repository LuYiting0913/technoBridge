using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBarReference {
    private Vector2 headPosition;
    private Vector2 tailPosition;
    private int headSplitNum, tailSplitNum;
    private int material;
    private float maxLength = 200f; 
    private float hydraulicFactor;

    public SolidBarReference(Vector2 head, Vector2 tail, int m, float factor, int headSplit, int tailSplit) {
        headPosition = head;
        tailPosition = tail;
        material = m;
        headSplitNum = headSplit;
        tailSplitNum = tailSplit;
        hydraulicFactor = factor;
    }

    public static SolidBarReference of(SolidBar bar) {
        return new SolidBarReference(bar.GetHead(), bar.GetTail(), bar.material, bar.GetHydraulicFactor(),
                                    bar.headSplitNum, bar.tailSplitNum);
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

    public float GetLength() {
        return (headPosition - tailPosition).magnitude;
    }

    public int GetMaterial() {
        return material;
    }

    public bool IsHeadSplit() {
        return headSplitNum != -1;
    }

    public bool IsTailSplit() {
        return tailSplitNum != -1;
    }

    public int GetTailSplitNum() {
        return tailSplitNum;
    }

    public int GetHeadSplitNum() {
        return headSplitNum;
    }

    public Point GetHeadSplit(Point point) {
        if (IsHeadSplit()) {
            return point.transform.GetChild(headSplitNum).GetComponent<Point>();
        }
        return point;
        
    }

    public Point GetTailSplit(Point point) {
        if (IsTailSplit()) {
            return point.transform.GetChild(tailSplitNum).GetComponent<Point>();
        }
        return point;
        
    }

    public float GetHydraulicFactor() {
        return hydraulicFactor;
    }
}
