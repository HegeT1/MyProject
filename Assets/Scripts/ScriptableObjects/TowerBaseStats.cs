using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "TowerBaseStatsScriptableObject", menuName = "ScriptableObjects/TowerBaseStats")]
public class TowerBaseStats : ScriptableObject/*, ISerializationCallbackReceiver*/
{
    public List<Stat> Keys { get; private set; }
    public List<float> Values { get; private set; }

    //[field: SerializeField] public Dictionary<Stat, float> BaseStats { get; private set; } = new();

    //public bool modify;

    //public void OnAfterDeserialize()
    //{
    //    BaseStats = new();

    //    for(int i = 0; i < Mathf.Min(Keys.Count, Values.Count); i++)
    //    {
    //        BaseStats.Add(Keys[i], Values[i]);
    //    }
    //}

    //public void OnBeforeSerialize()
    //{
    //    if(!modify)
    //    {
    //        Keys.Clear();
    //        Values.Clear();

    //        foreach(var keyValue in BaseStats)
    //        {
    //            Keys.Add(keyValue.Key);
    //            Values.Add(keyValue.Value);
    //        }
    //    }
    //}

    private List<Stat> _allStats = new()
    {
        Stat.Damage,
        Stat.AttackSpeed,
        Stat.Range,
        Stat.CriticalChance,
        Stat.CriticalDamage,
        Stat.TargetableEnemies
    };

    public void AddKeyValue()
    {
        if(GetAvailableStat() != null)
        {
            Keys.Add((Stat)GetAvailableStat());
            Values.Add(0);
        }
    }

    private Stat? GetAvailableStat()
    {
        foreach(Stat stat in _allStats)
        {
            if (!Keys.Contains(stat))
                return stat;
        }
        return null;
    }
}

[Serializable]
public enum Stat
{
    Damage,
    AttackSpeed,
    Range,
    CriticalChance,
    CriticalDamage,
    TargetableEnemies,
}