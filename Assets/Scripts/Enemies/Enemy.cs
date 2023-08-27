using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> points;

    private int currentPoint = 0;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float healthPoints;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[currentPoint].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPoint < points.Count)
        {
            transform.position = Vector2.MoveTowards(transform.position, points[currentPoint].transform.position, moveSpeed * Time.deltaTime);
            
            if(transform.position == points[currentPoint].transform.position)
            {
                currentPoint++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("End"))
        {
            Destroy(gameObject);
        }
    }
}
