using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeScriptableObject", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptableObject : ScriptableObject
{
    [field: SerializeField] public TowerStats UpgradedStats { get; private set; }
    [field: SerializeField] public float Cost { get; private set; }
}
