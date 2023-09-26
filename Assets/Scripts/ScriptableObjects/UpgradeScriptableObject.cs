using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeScriptableObject", menuName = "ScriptableObjects/Upgrade")]
public class UpgradeScriptableObject : ScriptableObject
{
    //[field: Tooltip("Values have to be in %. Upgraded value will be base value * upgrade value. If value is 0 the upgrade will be ignored.")]
    [field: SerializeField] public TowerStats UpgradedStats { get; private set; }
}
