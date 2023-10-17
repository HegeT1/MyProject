using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tower))]
public class TowerEditor : Editor
{
    private bool _showStats;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //DrawDictionary();
    }

    private void DrawDictionary()
    {
        Tower tower = (Tower)target;

        _showStats = EditorGUILayout.Foldout(_showStats, "Tower Stats", true);
        if (_showStats)
        {
            EditorGUI.indentLevel++;
            if (tower.Keys.Count > 0 || tower.Values.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(tower.Keys.Count, tower.Values.Count); i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(50));
                    tower.Keys[i] = (TowerStat)EditorGUILayout.EnumPopup(tower.Keys[i]);
                    EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(50));
                    tower.Values[i] = EditorGUILayout.FloatField(tower.Values[i]);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
                EditorGUILayout.LabelField("No Stats");
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();
        tower.RefreshKeysValues();
    }
}
