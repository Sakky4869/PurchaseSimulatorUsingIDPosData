using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer3D : MonoBehaviour
{

    [HideInInspector]
    public Queue<Production3D> traceProductions3D;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private List<string> traceProductionList;

    [HideInInspector]
    public SimulationManager3D simulationManager3D;

    private Production3D targetProduction3D;

    [SerializeField]
    private float moveSpeed;

    private bool isShopping;

    [SerializeField]
    private float achievedDistance;

    private Transform exitTransform;

    void Start()
    {
        
    }

    void Update()
    {
        if (isShopping == false)
            return;
        if (simulationManager3D.isInSimulation == false)
            return;
        if (simulationManager3D.isPausedSimulation == false)
            return;

        if(targetProduction3D == null)
        {
            if(traceProductions3D.Count != 0)
            {
                targetProduction3D = traceProductions3D.Dequeue();
            }
            else
            {
                exitTransform = GameObject.FindGameObjectWithTag("Exit").transform;
            }
        }
        else
        {
            if (targetProduction3D)
            {
                agent.SetDestination(targetProduction3D.transform.position);
                if (agent.pathPending)
                    return;
                if(agent.path.corners != null)
                {
                    if(GetDistanceToTargetObject() > achievedDistance)
                    {
                        UpdatePosition();
                    }
                    else
                    {
                        targetProduction3D = null;
                    }
                }
            }
        }
    }

    public void StartSimulation3D()
    {
        isShopping = true;
    }

    public void RegisterTracePositions(Queue<Production3D> productionQueue)
    {
        if (traceProductions3D == null)
            traceProductions3D = new Queue<Production3D>();

        if (traceProductionList == null)
            traceProductionList = new List<string>();

        while(productionQueue.Count != 0)
        {
            Production3D production3D = productionQueue.Dequeue();
            traceProductions3D.Enqueue(production3D);
            traceProductionList.Add(production3D.metaData + " " + production3D.productionName);
        }
    }


    /// <summary>
    /// 到達目標の商品オブジェクトまでの距離を計算
    /// </summary>
    /// <returns>到達目標の商品オブジェクトまでの距離</returns>
    private float GetDistanceToTargetObject()
    {
        Vector3 dist = targetProduction3D.transform.position - transform.position;
        dist.y = 0;
        return Vector3.Magnitude(dist);
    }

    private void UpdatePosition()
    {
        Vector3 direction = Vector3.zero;

        if(agent.path.corners.Length >= 2)
        {
            direction = agent.path.corners[1] - transform.position;
            direction.y = 0;
            direction = direction.normalized;
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
