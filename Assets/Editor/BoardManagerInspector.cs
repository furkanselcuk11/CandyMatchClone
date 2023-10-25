using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardManager))]    // Scriptinin oldu�u yerde g�sterilir
public class BoardManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BoardManager boardManager = (BoardManager)target;   // ��indeki datalara eri�im
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Board Level Editor for Match-3 Game");
        if (GUILayout.Button("Open Board Editor"))
        {
            BoardLevelEditor window = EditorWindow.GetWindow<BoardLevelEditor>();
            // Pencere geni�li�i ve y�ksekli�ini ayarla
            window.position = new Rect(100, 100, 1000, 800); // (x, y, width, height)
            window.Show();
        }
    }
}