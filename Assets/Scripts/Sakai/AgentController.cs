using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{

    [SerializeField]
    private int speed;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private GameObject target;

    private Queue<Vector3> tracePositions;

    [SerializeField]
    private GameObject tracePositionObject;

    private bool switchFlag;

    void Start()
    {
        switchFlag = false;
    }

    public void Init()
    {
        tracePositions = new Queue<Vector3>();
    }

    void Update()
    {
        
        agent.SetDestination(target.transform.position);

        //Debug.Log(agent.pathPending);

        //if (agent.pathPending == false)
        //{
        //    InstantiateTracePositionObjects();
        //    //if(switchFlag == false){
        //    //    switchFlag = true;
        //    //}
        //}
        //else
        //{
        //    if (switchFlag)
        //        switchFlag = false;
        //}

        //UpdateRotation();
        //UpdatePosition();


    }

    private void InstantiateTracePositionObjects()
    {
        foreach (Vector3 pos in agent.path.corners)
        {
            Instantiate(tracePositionObject, pos, Quaternion.identity);
        }
        //Instantiate(tracePositionObject, agent.path.corners[0], Quaternion.identity);
    }

    private void UpdateQueue()
    {

    }

    private void UpdatePosition()
    {
        transform.Translate(transform.forward * speed);
    }

    private void UpdateRotation()
    {
        if (agent == null)
        {
            Debug.Log("agent is null");
            return;
        }

        if(agent.nextPosition == null)
        {
            Debug.Log("agent next position is null");
            return;
        }

        Quaternion angle = Quaternion.LookRotation(agent.nextPosition - transform.position);
        angle.x = 0;
        angle.z = 0;
        transform.rotation = angle;
    }

}
