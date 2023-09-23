using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyTower : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private GameManager _gameManagerScript;
    [SerializeField] private TowerScriptableObject _towerScriptableObject;
    [SerializeField] private GameObject _tower;
    private bool _isDrag;

    // Start is called before the first frame update
    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void InstantiateTower(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && _gameManagerScript.Money >= _towerScriptableObject.BaseStats.Cost && _tower == null)
        {
            Debug.Log("inst");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;

            _tower = Instantiate(_towerScriptableObject.Prefab, mousePosition, _towerScriptableObject.Prefab.transform.rotation);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("click");
        //_isDrag = false;
        //InstantiateTower(eventData);
        //_tower = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("down");
        _isDrag = true;
        InstantiateTower(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("up");
        if (_isDrag && _tower.GetComponent<Tower>().PlaceTower())
        {
            //Debug.Log("placed");
            _tower = null;
        }
    }

    public void SelectTower(TowerScriptableObject towerScriptableObject)
    {
        Debug.Log(towerScriptableObject.Name);
    }
}
