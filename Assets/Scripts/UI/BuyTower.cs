using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyTower : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TowerScriptableObject _towerScriptableObject;
    [SerializeField] private GameObject _statsWindow;
    private ShopManager _shopManagerScritp;

    private void Start()
    {
        _shopManagerScritp = gameObject.transform.parent.transform.parent.GetComponent<ShopManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
            _shopManagerScritp.InstantiateTower(_towerScriptableObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_shopManagerScritp.Tower != null && _shopManagerScritp.Tower.GetComponent<Tower>().PlaceTower())
            _shopManagerScritp.Tower = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TowerStatsWindow towerStatsWindowScript = _statsWindow.GetComponent<TowerStatsWindow>();

        towerStatsWindowScript.SetStats(_towerScriptableObject.BaseStats);
        towerStatsWindowScript.SetPosition(gameObject.transform.localPosition);
        _statsWindow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _statsWindow.SetActive(false);
    }
}
