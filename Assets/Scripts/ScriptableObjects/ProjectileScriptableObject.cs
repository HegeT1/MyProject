using JetBrains.Annotations;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject", menuName = "ScriptableObjects/Projectile")]
public class ProjectileScriptableObject : ScriptableObject
{
    [field: SerializeField] public ProjectileStats BaseStats { get;  set; }
    [field: SerializeField] public ProjectileType Type { get; set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}

[Serializable]
public struct ProjectileStats
{
    public float Speed;
    public float UpTime;
    public int Pierce;
}

[Serializable]
public enum ProjectileType
{
    Linear,
    Homing,
    LinearBounce,
    AOE
}