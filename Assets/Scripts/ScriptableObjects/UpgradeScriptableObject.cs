using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class UpgradeStat
{
    public float Value;
    public bool IsPercent;
    public bool BaseValue = true;
    public Arithemtic Arithemtic;
}

[Serializable]
public enum Arithemtic { Add, Subtract, Multiply, Divide }

[CreateAssetMenu(fileName = "UpgradeScriptableObject", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    [field: SerializeField] public List<TowerStat> TowerStats { get; private set; } = new();

    [field: SerializeField] public float Cost { get; private set; }
    [field: SerializeField] public List<TowerStat> Keys { get; private set; } = new();
    [field: SerializeField] public List<UpgradeStat> Values { get; private set; } = new();
    [field: SerializeField] public Dictionary<TowerStat, UpgradeStat> Upgrades { get; private set; } = new();

    public void UpdateDictionary()
    {
        Upgrades.Clear();
        for (int i = 0; i < Mathf.Min(Keys.Count, Values.Count); i++)
            Upgrades.Add(Keys[i], Values[i]);
    }

    public void AddKeyValue()
    {
        if (GetAvailableStat(true) != null)
        {
            Keys.Add((TowerStat)GetAvailableStat(true));
            Values.Add(new UpgradeStat());
            UpdateDictionary();
        }
    }

    public void RemoveKeyValue()
    {
        if (Keys.Count > 0 && Values.Count > 0)
        {
            Keys.RemoveAt(Keys.Count - 1);
            Values.RemoveAt(Values.Count - 1);
            UpdateDictionary();
        }
    }

    public TowerStat? GetAvailableStat(bool val)
    {
        foreach (TowerStat stat in Enum.GetValues(typeof(TowerStat)))
        {
            if(!val)
            {
                Debug.Log(Keys.Count);
                Debug.Log(stat + "   " + Keys.Contains(stat));
            }

            if (!Keys.Contains(stat))
            {
                
                return stat;
            }
        }
        return null;
    }

    public void OnBeforeSerialize()
    {
        UpdateDictionary();
    }

    public void OnAfterDeserialize()
    {
        UpdateDictionary();
    }
}