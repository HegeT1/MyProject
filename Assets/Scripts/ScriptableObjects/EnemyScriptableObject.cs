using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public Type Type;
    [field: SerializeField] public Stats BaseStats { get; private set; }
    public GameObject Prefab;
}

[Serializable]
public struct Stats
{
    public float MoveSpeed;
    public float MaxHealthPoints;
    public int DamageToPlayer;
}

public enum Type
{
    Easy,
    Medium,
    Hard
}