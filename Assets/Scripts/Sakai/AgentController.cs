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

    void Start()
    {
        //agent.speed = speed;
    }

    void Update()
    {
        //if (Input.GetKey(KeyCode.W))
        //{
        //    transform.Translate(0, 0, speed * Time.deltaTime);
        //}

        //if (Input.GetKey(KeyCode.S))
        //{
        //    transform.Translate(0, 0, - speed * Time.deltaTime);
        //}

        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(speed * Time.deltaTime, 0, 0);
        //}

        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(-speed * Time.deltaTime, 0, 0);
        //}

        agent.SetDestination(target.transform.position);
        Debug.Log(agent.pathPending);
        //Debug.Log();
        
    }
}
