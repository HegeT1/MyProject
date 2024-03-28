using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private Tower _towerScript;
    [SerializeField] private GameObject _pathPrefab;
    [SerializeField] private GameObject _pathMaxedPrefab;
    
    public void Initialize(Tower towerScript)
    {
        _towerScript = towerScript;
    }

    public void InitializeUpgradePathsLayout()
    {
        Vector2 upgradeWindowSize = gameObject.GetComponent<RectTransform>().sizeDelta;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < _towerScript.TowerScriptableObject.UpgradePaths.Count; i++)
        {
            GameObject path;

            if (_towerScript.GetUpgradePathPosition(i) >= _towerScript.TowerScriptableObject.UpgradePaths[i].Path.Count)
            {
                path = Instantiate(_pathMaxedPrefab, gameObject.transform);
                path.name = "Path " + (i + 1) + " maxed";
            }
            else
            {
                path = Instantiate(_pathPrefab, gameObject.transform);
                path.name = "Path " + (i + 1);
                path.GetComponentInChildren<UpgradeButton>().InitializeListener(_towerScript, i);
            }

            path.GetComponent<RectTransform>().sizeDelta = new Vector2(upgradeWindowSize.x, upgradeWindowSize.y / _towerScript.TowerScriptableObject.UpgradePaths.Count);
            path.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(path.GetComponent<RectTransform>().sizeDelta.x, path.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}
