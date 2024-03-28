using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradePaths
{
    public List<UpgradeScriptableObject> Path;
}

[CreateAssetMenu(fileName = "TowerScriptableObject", menuName = "ScriptableObjects/Tower")]
public class TowerScriptableObject : ScriptableObject
{
    [field: SerializeField] public TowerBaseStatsScriptableObject TowerBaseStats { get; private set; }

    [field: SerializeField] public string Name { get; private set; }
    [field: Min(0)]
    [field: SerializeField] public float Cost { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public ProjectileScriptableObject Projectile { get; private set; }
    [field: SerializeField] public List<UpgradePaths> UpgradePaths { get; private set; }
}