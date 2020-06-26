using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class DijkstraCell : MonoBehaviour
{
    [HideInInspector]
    public float cost;

    public DijkstraCell[] nearDijkstraCells;

    private Dijkstra dijkstra;

    [HideInInspector]
    public DijkstraCell beforeCell;

    [HideInInspector]
    public bool isConst;

    private Vector2 beforePos;

    private RectTransform rectTransform;


    void Start()
    {
        cost = int.MaxValue;
        dijkstra = GameObject.Find("Dijkstra").GetComponent<Dijkstra>();
        rectTransform = GetComponent<RectTransform>();

    }



    void Update()
    {
        
    }


    public void OnPointerClick(BaseEventData data)
    {
        if(data.currentInputModule.input.GetMouseButtonUp(1))
            Debug.Log("右クリック");
    }

    public void OnPointerDragBegin(BaseEventData data)
    {
        beforePos = Input.mousePosition;
    }

    public void OnPointerDrag(BaseEventData data)
    {
        Vector2 diff = (Vector2)Input.mousePosition - beforePos;
        beforePos = Input.mousePosition;

        rectTransform.anchoredPosition += diff;
    }
}
