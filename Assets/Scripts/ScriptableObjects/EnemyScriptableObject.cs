using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [field: SerializeField] public EnemyStats BaseStats { get; private set; }
    public GameObject Prefab;
}

[Serializable]
public struct EnemyStats
{
    public float MoveSpeed;
    public float MaxHealthPoints;
    public int DamageToPlayer;
}