using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerBaseStatsScriptableObject))]
public class DictionaryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //DrawDictionary();
    }

    public void DrawDictionary()
    {
        TowerBaseStatsScriptableObject towerBaseStats = (TowerBaseStatsScriptableObject)target;

        var keyProperty = serializedObject.FindProperty("Keys");
        EditorGUILayout.PropertyField(keyProperty);
        serializedObject.ApplyModifiedProperties();

        for (int i = 0; i < Mathf.Min(towerBaseStats.Keys.Count, towerBaseStats.Values.Count); i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(50));
            towerBaseStats.Keys[i] = (TowerStat)EditorGUILayout.EnumPopup(towerBaseStats.Keys[i]);
            EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(50));
            towerBaseStats.Values[i] = EditorGUILayout.FloatField(towerBaseStats.Values[i]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Add"))
        {
            towerBaseStats.AddKeyValue();
        }

        if (GUILayout.Button("Remove"))
        {
            towerBaseStats.RemoveKeyValue();
        }
    }
}
