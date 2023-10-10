using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellTower : MonoBehaviour
{
    private Tower _towerScript;

    public void Set(Tower towerScript) 
    {
        _towerScript = towerScript;    
    }

    public void Sell()
    {
        _towerScript.SellTower();
    }
}
