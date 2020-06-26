using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellCatcher : MonoBehaviour
{
    [SerializeField]
    private DijkstraCell myCell;

    enum CatcherKind
    {
        UP = 0,
        RIGHT,
        DOWN,
        LEFT
    }

    [SerializeField]
    private CatcherKind catcherKind;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DijkstraCell>() == null)
            return;
        myCell.nearDijkstraCells[(int)catcherKind] = collision.GetComponent<DijkstraCell>();
        gameObject.SetActive(false);
    }
}
