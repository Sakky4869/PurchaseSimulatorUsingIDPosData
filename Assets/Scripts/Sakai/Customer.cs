using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    /// <summary>
    /// たどる商品の位置のQueue
    /// </summary>
    [HideInInspector]
    public Queue<Vector2> tracePositions;

    /// <summary>
    /// 読み取られたID-POSデータ
    /// </summary>
    [HideInInspector]
    public List<string> dataStrings;

    [HideInInspector]
    public SimulationManager simulationManager;

    void Start()
    {
        tracePositions = new Queue<Vector2>();
        simulationManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationManager>();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// たどる場所を登録する
    /// </summary>
    /// <param name="datas"></param>
    public void RegisterTracePositions(List<string> datas)
    {
        for(int i = datas.Count - 1; i > 0; i--)
        {
            tracePositions.Enqueue(simulationManager.GetProductPosition(datas[i]));
        }
    }
}
