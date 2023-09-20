using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveScriptableObject", menuName = "ScriptableObjects/Wave")]
public class WaveScriptableObject : ScriptableObject
{
    [field: SerializeField] public int Number { get; private set; }
    [field: SerializeField] public WaveType Type { get; private set; }
    [field: SerializeField] public List<WavePart> Parts { get; private set; }
    [field: SerializeField] public float MoneyAwarded { get; private set; }
}

[Serializable]
public struct WavePart
{
    public int EnemyCount;
    public float EnemySpacing;
    public EnemyScriptableObject EnemyObject;
    public float DelayForNextPart;
}

[Serializable]
public enum WaveType
{
    Normal,
    Boss
}