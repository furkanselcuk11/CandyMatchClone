using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardManager))]    // Scriptinin olduðu yerde gösterilir
public class BoardManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BoardManager boardManager = (BoardManager)target;   // Ýçindeki datalara eriþim
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Board Level Editor for Match-3 Game");
        if (GUILayout.Button("Open Board Editor"))
        {
            BoardLevelEditor window = EditorWindow.GetWindow<BoardLevelEditor>();
            // Pencere geniþliði ve yüksekliðini ayarla
            window.position = new Rect(100, 100, 1000, 800); // (x, y, width, height)
            window.Show();
        }
    }
}