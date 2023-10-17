using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradeScriptableObject))]
public class UpgradeEditor : Editor
{
    private bool _showUpgrades = true;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        // Remove

        UpgradeScriptableObject upgrade = (UpgradeScriptableObject)target;

        serializedObject.Update();
        var keys = serializedObject.FindProperty("<Keys>k__BackingField");
        var values = serializedObject.FindProperty("<Values>k__BackingField");
        if (GUILayout.Button("Add"))
        {
            if(upgrade.GetAvailableStat(true) != null)
            {
                keys.arraySize++;
                values.arraySize++;

                serializedObject.ApplyModifiedProperties();

                upgrade.Keys[keys.arraySize - 1] = (TowerStat)upgrade.GetAvailableStat(false);
                upgrade.Values[values.arraySize - 1] = new();

                serializedObject.ApplyModifiedProperties();
            }
        }

        if(GUILayout.Button("Remove"))
        {
            keys.arraySize--;
            values.arraySize--;
        }

        if (GUILayout.Button("Log"))
        {
            foreach (TowerStat stat in upgrade.Keys)
                Debug.Log(stat);
        }

        var cost = serializedObject.FindProperty("<Cost>k__BackingField");
        EditorGUILayout.PropertyField(cost, new GUIContent("Cost"));

        _showUpgrades = EditorGUILayout.Foldout(_showUpgrades, "Stats To Be Upgraded", true);
        if(_showUpgrades)
        {
            EditorGUI.indentLevel++;
            if(keys.arraySize > 0 && values.arraySize > 0)
            {
                for (int i = 0; i < Mathf.Min(keys.arraySize, values.arraySize); i++)
                {
                    var key = keys.GetArrayElementAtIndex(i);
                    var value = values.GetArrayElementAtIndex(i);

                    GUILayout.BeginVertical("window");

                    EditorGUILayout.PropertyField(key, new GUIContent("Stat"));

                    EditorGUILayout.PropertyField(value);

                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.LabelField("No Upgradable Stats");
            EditorGUI.indentLevel--;
        }
        serializedObject.ApplyModifiedProperties();

        // Remove



        //DrawDictionary();
    }

    private void DrawDictionary()
    {
        UpgradeScriptableObject upgrade = (UpgradeScriptableObject)target;

        _showUpgrades = EditorGUILayout.Foldout(_showUpgrades, "Stats To Be Upgraded", true);

        if(_showUpgrades)
        {
            EditorGUI.indentLevel++;
            if (upgrade.Keys.Count > 0 || upgrade.Values.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(upgrade.Keys.Count, upgrade.Values.Count); i++)
                {
                    GUILayout.BeginVertical("window");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stat", GUILayout.MaxWidth(120));
                    upgrade.Keys[i] = (TowerStat)EditorGUILayout.EnumPopup(upgrade.Keys[i]);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(120));
                    upgrade.Values[i].Value = EditorGUILayout.FloatField(upgrade.Values[i].Value);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Is Percent", GUILayout.MaxWidth(120));
                    upgrade.Values[i].IsPercent = EditorGUILayout.Toggle(upgrade.Values[i].IsPercent);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Arithemtic", GUILayout.MaxWidth(120));
                    upgrade.Values[i].Arithemtic = (Arithemtic)EditorGUILayout.EnumPopup(upgrade.Values[i].Arithemtic);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    GUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.LabelField("No Upgradable Stats");

            if (GUILayout.Button("Add"))
            {
                upgrade.AddKeyValue();
            }

            if (GUILayout.Button("Remove"))
            {
                upgrade.RemoveKeyValue();
            }

            EditorGUI.indentLevel--;
        }
    }
}
