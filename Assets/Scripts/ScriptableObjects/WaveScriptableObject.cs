using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveScriptableObject", menuName = "ScriptableObjects/Wave")]
public class WaveScriptableObject : ScriptableObject
{
    public int WaveNumber;
    public WaveType WaveType;
    public List<WavePart> WaveParts;
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