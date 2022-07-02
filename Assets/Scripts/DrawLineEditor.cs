using UnityEngine;
using UnityEditor;

// [CustomEditor( typeof( DrawLine ) )]
public class DrawLineEditor : Editor {

    void OnSceneGUI() {
        // get the chosen game object
        Handles.color = new Color(1,0,0);
        Handles.DrawLine(new Vector3(0, 0,0), new Vector3(100, 0, 0), 10);
        Debug.Log("drawn");
    }
}