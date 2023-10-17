using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerBaseStatsScriptableObject", menuName = "ScriptableObjects/TowerBaseStats")]
public class TowerBaseStatsScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    [field: SerializeField] public List<TowerStat> Keys { get; private set; } = new();
    [field: SerializeField] public List<float> Values { get; private set; } = new();
    public Dictionary<TowerStat, float> BaseStats { get; private set; } = new();

    private void UpdateDictionary()
    {
        BaseStats.Clear();
        for(int i = 0; i < Mathf.Min(Keys.Count, Values.Count); i++)
            BaseStats.Add(Keys[i], Values[i]);
    }

    public void AddKeyValue()
    {
        if(GetAvailableStat() != null)
        {
            Keys.Add((TowerStat)GetAvailableStat());
            Values.Add(0);
            UpdateDictionary();
        }
    }

    public void RemoveKeyValue()
    {
        if(Keys.Count > 0 && Values.Count > 0)
        {
            Keys.RemoveAt(Keys.Count - 1);
            Values.RemoveAt(Values.Count - 1);
            UpdateDictionary();
        }
    }

    private TowerStat? GetAvailableStat()
    {
        foreach(TowerStat stat in Enum.GetValues(typeof(TowerStat)))
        {
            if (!Keys.Contains(stat))
                return stat;
        }
        return null;
    }

    public float? GetStatValue(TowerStat stat)
    {
        if(BaseStats.ContainsKey(stat))
            return BaseStats[stat];
        return null;
    }

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        BaseStats.Clear();

        for (int i = 0; i < Mathf.Min(Keys.Count, Values.Count); i++)
            BaseStats.Add(Keys[i], Values[i]);
    }
}