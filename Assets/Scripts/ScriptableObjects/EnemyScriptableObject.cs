using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [field: SerializeField] public EnemyStats BaseStats { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}

[Serializable]
public struct EnemyStats
{
    public float MoveSpeed;
    public float MaxHealthPoints;
    public int DamageToPlayer;
    public float MoneyWorth;
}