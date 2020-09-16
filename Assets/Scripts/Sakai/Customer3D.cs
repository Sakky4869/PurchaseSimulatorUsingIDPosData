using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer3D : MonoBehaviour
{

    //[HideInInspector]
    public Queue<Production3D> traceProductions3D;

    //[SerializeField]
    public NavMeshAgent agent;

    [SerializeField]
    private List<string> traceProductionList;

    [HideInInspector]
    public SimulationManager3D simulationManager3D;

    [SerializeField]
    private Production3D targetProduction3D;

    [SerializeField]
    private float moveSpeed;

    private bool isShopping;

    [SerializeField]
    private float achievedDistance;

    [SerializeField]
    private Transform exitTransform;
    [SerializeField]
    private bool pathpending;

    private Rigidbody rigidbody;

    private bool isGetPath;

    private bool isStartedGettingPath;

    [HideInInspector]
    public bool isGetFirstProduction;

    [HideInInspector]
    public bool isGettingExit;

    //private float searchRouteStartTime;

    //[SerializeField]
    //private int cornerCount;

    //public int traceProducitonCount { get { return traceProductions3D.Count; } set { } }

    void Start()
    {
        //simulationManager3D = GameObject.Find("SimulationManager3D").GetComponent<SimulationManager3D>();
    }

    void Update()
    {
        if (isShopping == false)
            return;
        if (simulationManager3D.isInSimulation == false)
            return;
        if (simulationManager3D.isPausedSimulation == true)
            return;
        //if (simulationManager3D.isPathPending)
        //{
        //    if(simulationManager3D.writerAgentId == gameObject.GetInstanceID())
        //    {
        //        simulationManager3D.isPathPending = agent.pathPending;
        //    }
        //    return;
        //}
        //if (agent.enabled == false)
        //{
        //    if(Time.time - searchRouteStartTime > 2)
        //    {
        //        agent.enabled = true;
        //        Debug.Log("agent 有効化");
        //    }
        //    return;
        //}

        if (targetProduction3D == null)
        {
            if (traceProductions3D.Count != 0)
            {
                targetProduction3D = traceProductions3D.Dequeue();
            }
            else
            {
                if (exitTransform == null)
                {
                    exitTransform = GameObject.FindGameObjectWithTag("Exit").transform;
                    isGettingExit = true;
                }
                //if(agent.path.corners.Length == 0 || agent.path.corners == null)
                if(isGetPath == false)
                {
                    if(isStartedGettingPath == false)
                    {
                        agent.SetDestination(exitTransform.position);
                        isStartedGettingPath = true;
                    }
                }
                if (agent.pathPending)
                {
                    //agent.SetDestination(exitTransform.position);
                    //simulationManager3D.isPathPending = true;
                    //simulationManager3D.writerAgentId = gameObject.GetInstanceID();
                    //agent.avoidancePriority = 0;// Random.Range(0, 100);
                    //pathpending = agent.pathPending;
                    //if(Time.time - searchRouteStartTime> 1)
                    //{
                    //    agent.enabled = false;
                    //}
                    return;
                }
                if (isGetPath == false)
                {
                    isGetPath = true;
                    isStartedGettingPath = false;
                }
                //else
                //{
                //    if(agent.path.corners.Length < 2)
                //        agent.SetDestination(exitTransform.position);
                //}
                //simulationManager3D.isPathPending = false;
                //agent.avoidancePriority = 99;
                if (GetDistanceToTargetObject() > achievedDistance)
                {
                    UpdatePosition();
                }
                else
                {
                    //simulationManager3D.AddExit();
                    simulationManager3D.customerCount--;
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (targetProduction3D)
            {
                //Debug.Log(agent.path.corners == null);
                //if (agent.path.corners.Length == 0 || agent.path.corners == null)
                //agent.SetDestination(targetProduction3D.transform.position);
                if (isGetPath == false)
                {
                    if (isStartedGettingPath == false)
                    {
                        agent.SetDestination(targetProduction3D.transform.position);
                        isStartedGettingPath = true;
                    }
                }
                if (agent.pathPending)
                {
                    //agent.SetDestination(targetProduction3D.transform.position);
                    //simulationManager3D.isPathPending = true;
                    //simulationManager3D.writerAgentId = gameObject.GetInstanceID();
                    //agent.avoidancePriority = 0;// Random.Range(0, 100);
                    //pathpending = agent.pathPending;
                    //if (Time.time - searchRouteStartTime > 1)
                    //{
                    //    agent.enabled = false;
                    //    Debug.Log("agent 無効化");
                    //}

                    return;
                }
                if (isGetPath == false)
                {
                    isGetPath = true;
                    isStartedGettingPath = false;
                }
                //else
                //{
                //    if(agent.path.corners.Length < 2)
                //        agent.SetDestination(targetProduction3D.transform.position);
                //}
                //simulationManager3D.isPathPending = false;
                //agent.avoidancePriority = 99;

                if (agent.path.corners != null)
                {
                    if (GetDistanceToTargetObject() > achievedDistance)
                    {
                        UpdatePosition();
                    }
                    else
                    {
                        if (isGetFirstProduction == false)
                            isGetFirstProduction = true;
                        targetProduction3D = null;
                        isGetPath = false;
                        //searchRouteStartTime = Time.time;
                    }
                }
            }
        }

        //remaining = agent.remainingDistance;
        //pathpending = agent.pathPending;
        //if (agent.path.corners != null)
        //    cornerCount = agent.path.corners.Length;

    }

    public void StartSimulation3D()
    {
        simulationManager3D = GameObject.Find("SimulationManager3D").GetComponent<SimulationManager3D>();
        isShopping = true;
        agent.enabled = true;
        rigidbody = GetComponent<Rigidbody>();
        //agent.avoidancePriority = Random.Range(0, 100);
        //agent.avoidancePriority = simulationManager3D.GetPriorityOfNavmeshAgent();
        //StartCoroutine(UpdateCoroutine());
    }

    //private IEnumerator UpdateCoroutine()
    //{
    //    while (true)
    //    {
    //        if (isShopping == false)
    //        {
    //            yield return null;
    //            continue;
    //        }
    //        if (simulationManager3D.isInSimulation == false)
    //        {
    //            yield return null;
    //            //return;
    //            continue;
    //        }
    //        if (simulationManager3D.isPausedSimulation == true)
    //        {
    //            yield return null;
    //            //return;
    //            continue;
    //        }

    //        if (targetProduction3D == null)
    //        {
    //            if (traceProductions3D.Count != 0)
    //            {
    //                targetProduction3D = traceProductions3D.Dequeue();
    //            }
    //            else
    //            {
    //                if (exitTransform == null)
    //                    exitTransform = GameObject.FindGameObjectWithTag("Exit").transform;
    //                agent.SetDestination(exitTransform.position);
    //                if (agent.pathPending)
    //                {
    //                    yield return null;
    //                    //return null;
    //                    continue;
    //                }
    //                if (GetDistanceToTargetObject() > achievedDistance)
    //                {
    //                    UpdatePosition();
    //                }
    //                else
    //                {
    //                    Destroy(gameObject);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (targetProduction3D)
    //            {
    //                agent.SetDestination(targetProduction3D.transform.position);
    //                if (agent.pathPending)
    //                {
    //                    //pathpending = agent.pathPending;
    //                    yield return null;
    //                    //return;
    //                    continue;
    //                }

    //                if (agent.path.corners != null)
    //                {
    //                    if (GetDistanceToTargetObject() > achievedDistance)
    //                    {
    //                        UpdatePosition();
    //                    }
    //                    else
    //                    {
    //                        if (isGetFirstProduction == false)
    //                        {
    //                            isGetFirstProduction = true;
    //                        }
    //                        targetProduction3D = null;
    //                    }
    //                }
    //            }
    //        }

    //        yield return null;
    //    }
    //}

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

        //Debug.Log("trace production 3D count : " + traceProductions3D.Count);
    }


    /// <summary>
    /// 到達目標の商品オブジェクトまでの距離を計算
    /// </summary>
    /// <returns>到達目標の商品オブジェクトまでの距離</returns>
    private float GetDistanceToTargetObject()
    {
        Vector3 dist;
        if (targetProduction3D)
        {
            dist = targetProduction3D.transform.position - transform.position;
        }
        else
        {
            dist = exitTransform.position - transform.position;
        }
        dist.y = 0;
        return Vector3.Magnitude(dist);
    }

    private void UpdatePosition()
    {
        Vector3 direction = Vector3.zero;

        if(agent.path.corners.Length >= 2)
        {
            direction = agent.path.corners[1] - transform.position;
            //direction = agent.nextPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;
        }

        rigidbody.position += direction * moveSpeed * Time.deltaTime;

        //transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
