using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IslandGenerator))]
[CanEditMultipleObjects]
public class IslandGeneratorEditor : Editor
{
    SerializedProperty serializedGenerator;
    void OnEnable()
    {
        serializedGenerator = serializedObject.FindProperty("IslandGenerator");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        //setting values
        IslandGenerator thisGenerator = (IslandGenerator) target;
        thisGenerator.polygonCount = EditorGUILayout.IntField("Polygon Count" ,thisGenerator.polygonCount);
        thisGenerator.resolution = EditorGUILayout.IntField("Resolution" ,thisGenerator.resolution);
        thisGenerator.lloydIterations = EditorGUILayout.IntField("LLoyd Iterations" ,thisGenerator.lloydIterations);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        //EditorGUI.DrawPreviewTexture(new Rect(lastRect.x, lastRect.y, 100, 100), thisGenerator.firstStep); TODO: fix the preview texture
        
    }
}
