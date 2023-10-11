using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    //private UpgradeManager _upgradeManagerScript;
    [SerializeField] private TextMeshProUGUI _costText;
    //[SerializeField] private int _pathIndex;
    //private Tower _towerScript;

    // Start is called before the first frame update
    void Start()
    {
        //_upgradeManagerScript = gameObject.transform.parent.transform.parent.GetComponent<UpgradeManager>();
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void InitializeListener(Tower towerScript, int pathIndex)
    {
        float upgradeCost = towerScript.TowerScriptableObject.UpgradePaths[pathIndex].Path[towerScript.GetUpgradePathPosition(pathIndex)].Cost;
        //_towerScript = towerScript;
        //_pathIndex = pathIndex;
        _costText.SetText("Upgrade for: " + upgradeCost);
        GetComponent<Button>().onClick.AddListener(() => towerScript.UpgradeTower(pathIndex));
    }
}
