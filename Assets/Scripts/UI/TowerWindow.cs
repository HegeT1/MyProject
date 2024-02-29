using UnityEngine;

public class TowerWindow : MonoBehaviour
{
    private Tower _towerScript;
    
    public void SetupWindow(Tower towerScript)
    {
        foreach(Transform tower in GameObject.Find("Towers").transform)
            tower.GetComponent<Tower>().TowerRange.SetActive(false);

        _towerScript = towerScript;
        _towerScript.TowerRange.SetActive(true);

        InitializeTowerTargetingButtons();
        InitializeUpgradePaths();
        InitializeSellButton();
    }

    private void InitializeTowerTargetingButtons()
    {
        foreach(ChangeTowerTargeting button in transform.GetChild(1).transform.GetComponentsInChildren<ChangeTowerTargeting>())
        {
            button.InitializeButton(_towerScript.TowerTargeting, _towerScript);
        }
    }

    private void InitializeUpgradePaths()
    {
        UpgradeManager upgradeManagerScript = transform.GetComponentInChildren<UpgradeManager>();
        
        //upgradeManagerScript.InitializeUpgradePaths(_towerScript.TowerScriptableObject.UpgradePaths, _towerScript.UpgradeIndexes);
        upgradeManagerScript.Initialize(_towerScript);
        upgradeManagerScript.InitializeUpgradePathsLayout();
    }

    private void InitializeSellButton()
    {
        transform.GetComponentInChildren<SellTower>().Initialize(_towerScript);
    }
}
