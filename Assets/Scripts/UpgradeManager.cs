using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<UpgradePaths> _upgradePaths;
    [SerializeField] private List<int> _upgradeIndexes;
    [SerializeField] private GameObject _pathPrefab;
    private Tower _towerScript;
    
    public void SetUpgradePaths(List<UpgradePaths> upgradePaths, List<int> upgradePathsIndexes)
    {
        _upgradePaths = upgradePaths;
        _upgradeIndexes = upgradePathsIndexes;

        SetUpgradePathsLayout();
    }

    public void Set(Tower towerScript)
    {
        _towerScript = towerScript;
    }

    public void UpgradeTower(TowerStats upgradeStats, int pathIndex)
    {
        Debug.Log(pathIndex);
        _upgradeIndexes[pathIndex]++;
        _towerScript.TowerStats = upgradeStats;
    }

    private void SetUpgradePathsLayout()
    {
        Vector2 upgradeWindowSize = gameObject.GetComponent<RectTransform>().sizeDelta;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < _upgradePaths.Count; i++)
        {
            GameObject path = Instantiate(_pathPrefab, gameObject.transform);
            path.name = "Path " + (i + 1);
            path.GetComponent<RectTransform>().sizeDelta = new Vector2(upgradeWindowSize.x, upgradeWindowSize.y / _upgradePaths.Count);

            path.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(path.GetComponent<RectTransform>().sizeDelta.x, path.GetComponent<RectTransform>().sizeDelta.y);

            _pathPrefab.GetComponentInChildren<UpgradeButton>().SetListener(_upgradePaths[i].Path[_upgradeIndexes[i]], i);
        }
    }
}
