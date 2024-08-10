using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider == null || hit.collider.gameObject.GetComponent<Tower>() == null)
            {
                foreach (Transform tower in GameObject.Find("Towers").transform)
                    tower.GetComponent<Tower>().TowerRange.SetActive(false);
                GameObject.Find("Canvas").transform.Find("Main UI").Find("Bottom Panel").Find("Selected Tower").gameObject.SetActive(false);
            }
        }
    }
}
