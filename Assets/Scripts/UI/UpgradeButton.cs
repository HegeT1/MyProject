using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    private UpgradeManager _upgradeManagerScript;
    [SerializeField] private UpgradeScriptableObject _upgradeScriptableObject;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private int _pathIndex;

    // Start is called before the first frame update
    void Start()
    {
        _upgradeManagerScript = gameObject.transform.parent.transform.parent.GetComponent<UpgradeManager>();
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void SetListener(UpgradeScriptableObject upgrade, int pathIndex)
    {
        _upgradeScriptableObject = upgrade;
        _pathIndex = pathIndex;
        _costText.SetText("Upgrade for: " + _upgradeScriptableObject.Cost);
        GetComponent<Button>().onClick.AddListener(() => _upgradeManagerScript.UpgradeTower(_upgradeScriptableObject.UpgradedStats, _pathIndex));
    }
}
