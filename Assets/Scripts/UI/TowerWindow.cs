using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWindow : MonoBehaviour
{
    private Tower _towerScript;
    
    public void SetupWindow(Tower towerScript)
    {
        _towerScript = towerScript;

        SetupTowerTargetingButtons();
        SetupUpgradePaths();
    }

    private void SetupTowerTargetingButtons()
    {
        foreach(ChangeTowerTargeting button in transform.GetChild(1).transform.GetComponentsInChildren<ChangeTowerTargeting>())
        {
            button.SetButton(_towerScript.TowerTargeting, _towerScript);
        }
    }

    private void SetupUpgradePaths()
    {
        UpgradeManager upgradeManagerScript = transform.GetComponentInChildren<UpgradeManager>();

        upgradeManagerScript.SetUpgradePaths(_towerScript.TowerScriptableObject.UpgradePaths, _towerScript.UgradeIndexes);
        upgradeManagerScript.Set(_towerScript);
    }
}
