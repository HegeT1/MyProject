using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    private GameManager _gameManagerScript;
    public GameObject Tower;
    [SerializeField] private GameObject _towerParentObject;

    // Start is called before the first frame update
    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void InstantiateTower(TowerScriptableObject towerScriptableObject)
    {
        if (_gameManagerScript.Money >= towerScriptableObject.Cost && Tower == null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;

            Tower = Instantiate(towerScriptableObject.Prefab, mousePosition, towerScriptableObject.Prefab.transform.rotation, _towerParentObject.transform);
        }
    }
}
