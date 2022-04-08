using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class CustomButton : Editor
{
    public override void OnInspectorGUI () {
        DrawDefaultInspector();

        Test myScript = (Test)target;
        if (GUILayout.Button("Create scene")) {
            myScript.CreateScene();
        }
    }
}
