using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeScriptableObject", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptableObject : ScriptableObject
{
    public Dictionary<Stat, UpgradeStat> Upgrades = new Dictionary<Stat, UpgradeStat>()
    {
        { Stat.Damage, new(){ Value = 2 } } ,
    };

    [field: SerializeField] public TowerStats UpgradedStats { get; private set; }
    [field: SerializeField] public float Cost { get; private set; }
}

[Serializable]
public class UpgradeStat
{
    public float Value;
    public bool IsPercent;
    public Arithemtic Arithemtic;
}

[Serializable]
public enum Arithemtic { Add, Subtract, Multiply, Divide }