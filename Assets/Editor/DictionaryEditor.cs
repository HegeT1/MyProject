using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerBaseStats))]
public class DictionaryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TowerBaseStats towerBaseStats = (TowerBaseStats)target;

        for (int i = 0; i < Mathf.Min(towerBaseStats.Keys.Count, towerBaseStats.Values.Count); i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(50));
            towerBaseStats.Keys[i] = (Stat)EditorGUILayout.EnumPopup(towerBaseStats.Keys[i]);
            EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(50));
            towerBaseStats.Values[i] = EditorGUILayout.FloatField(towerBaseStats.Values[i]);
            EditorGUILayout.EndHorizontal();
        }

        if(GUILayout.Button("Add"))
        {
            towerBaseStats.AddKeyValue();
        }
    }
}
