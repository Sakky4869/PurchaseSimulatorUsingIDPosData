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

    void Start()
    {
        
    }

    public void Init()
    {
        tracePositions = new Queue<Vector3>();
    }

    void Update()
    {
        //if(agent.pathPending == true)
            agent.SetDestination(target.transform.position);
        //Debug.Log(agent.pathPending);
        //Debug.Log(agent.pathStatus);
        //Debug.Log(agent.nextPosition.x);
        UpdateRotation();
        UpdatePosition();


    }

    private void UpdateQueue()
    {

    }

    private void UpdatePosition()
    {
        transform.Translate(transform.rotation * transform.forward * speed);
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

        Quaternion angle = Quaternion.Euler(agent.nextPosition - transform.position);
        transform.rotation = angle;
    }

}
