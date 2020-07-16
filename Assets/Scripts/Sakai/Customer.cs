using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    /// <summary>
    /// たどる商品の位置のQueue
    /// </summary>
    [HideInInspector]
    public Queue<Production> traceProductions;

    /// <summary>
    /// 読み取られたID-POSデータ
    /// </summary>
    [HideInInspector]
    public List<string> dataStrings;

    [HideInInspector]
    public SimulationManager simulationManager;

    private Production targetProduction;

    public RectTransform rectTransform;

    public float moveSpeed;

    [HideInInspector]
    public bool isShopping;

    private RectTransform exitRectTransform;

    void Start()
    {
        //traceProductions = new Queue<Production>();
        simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        //rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isShopping == false)
            return;
        if (simulationManager.isInSimulation == false)
            return;
        if (simulationManager.isPausedSimulation == true)
            return;

        if(targetProduction == null)
        {
            if(traceProductions.Count != 0)
            {
                targetProduction = traceProductions.Dequeue();
                targetProduction.image.color = Color.yellow;
            }
            else
            {
                //Debug.Log("exit");
                exitRectTransform = GameObject.Find("Exit").GetComponent<RectTransform>();
            }
        }
        else
        {
            if (targetProduction)
            {
                Vector2 direct = (targetProduction.rectTransform.anchoredPosition - rectTransform.anchoredPosition).normalized;
                rectTransform.anchoredPosition += direct * moveSpeed;
            }
        }
        
        if (exitRectTransform)
        {
            Vector2 direct = (exitRectTransform.anchoredPosition - rectTransform.anchoredPosition).normalized;
            rectTransform.anchoredPosition += direct * moveSpeed;
        }
    }


    public void StartSimulation()
    {
        isShopping = true;
    }

    /// <summary>
    /// たどる場所を登録する
    /// </summary>
    /// <param name="datas"></param>
    public void RegisterTracePositions(Queue<Production> productionQueue)
    {
        if (traceProductions == null)
            traceProductions = new Queue<Production>();
        //Debug.Log(productionQueue.Count);
        while(productionQueue.Count != 0)
        {
            traceProductions.Enqueue(productionQueue.Dequeue());
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Production")
            return;
        // 移動先の商品にたどりついたら，ターゲット変数の初期化
        if(collision.GetComponent<Production>() == targetProduction)
        {
            //Debug.Log(targetProduction.metaData);
            targetProduction.image.color = Color.blue;
            targetProduction = null;
        }
    }

}
