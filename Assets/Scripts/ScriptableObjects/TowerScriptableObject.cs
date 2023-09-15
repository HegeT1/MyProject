using System;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "TowerScriptableObject", menuName = "ScriptableObjects/Tower")]
public class TowerScriptableObject : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public TowerStats BaseStats { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public ProjectileScriptableObject Projectile { get; private set; }
}

[Serializable]
public struct TowerStats
{
    [Min(0)]
    public float Range;
    public float AttackSpeed;
    [Min(0)]
    public float Damage;
    [Range(0f, 1f)]
    public float CriticalChance;
    [Min(1)]
    public float CriticalDamage;
    [Min(1)]
    public int TargetableEnemies;
    [Min(0)]
    public float Cost;
}
