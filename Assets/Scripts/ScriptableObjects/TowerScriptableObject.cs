using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerScriptableObject", menuName = "ScriptableObjects/Tower")]
public class TowerScriptableObject : ScriptableObject
{
    [field: SerializeField] public TowerStats BaseStats { get; private set; }
    [field: SerializeField] public ProjectileScriptableObject Projectile { get; private set; }
}

[Serializable]
public struct TowerStats
{
    public float Range;
    public float AttackSpeed;
    public float Damage;
    public int TargetableEnemies;
}
