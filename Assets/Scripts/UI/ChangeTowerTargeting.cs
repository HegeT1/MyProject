using UnityEngine;
using UnityEngine.UI;

public class ChangeTowerTargeting : MonoBehaviour
{
    [SerializeField] private TowerTargeting _towerTargeting;
    private Tower _towerScript;

    public void ChangeTargeting()
    {
        _towerScript.ChangeTargeting(_towerTargeting);
    }

    public void InitializeButton(TowerTargeting targeting, Tower towerScript)
    {
        _towerScript = towerScript;
        Image buttonImage = gameObject.GetComponent<Image>();
        Color activeColor = new(0.5f, 1f, 1f);
        if (targeting == _towerTargeting)
            buttonImage.color = activeColor;
        else
            buttonImage.color = Color.white;
    }
}
