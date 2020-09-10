using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/** ＜メモ＞
 * 商品のたどり着いたときのエージェントと商品の距離：10
 * 
 */

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

    private List<GameObject> tracePositionObjectList;

    private bool switchFlag;

    private GameObject spawned;


    void Start()
    {
        switchFlag = false;
        spawned = Instantiate(tracePositionObject, Vector3.zero, Quaternion.identity);
        tracePositionObjectList = new List<GameObject>();
    }

    public void Init()
    {
        tracePositions = new Queue<Vector3>();
    }

    void Update()
    {

        agent.SetDestination(target.transform.position);

        if (agent.pathPending)
            return;
        if (agent.path.corners != null)
        {
            //InstantiateTracePositionObjects();
            if(GetDistanceToTargetObject() > 10)
                UpdatePosition();
        }

        
        //Debug.Log(GetDistanceToTargetObject());
    }

    private void InstantiateTracePositionObjects()
    {
        //Debug.Log(agent.path.corners.Length);
        if(agent.path.corners.Length > 2)
        {
            //spawned.transform.position = agent.path.corners[agent.path.corners.Length - 2];
            //spawned.transform.position = agent.path.corners[1];
            transform.position = Vector3.Lerp(transform.position, agent.path.corners[1], 0.85f);
        }
        //spawned.transform.position = agent.path.corners[1];
        //for (int i = 0; i < agent.path.corners.Length; i++)
        //{
        //    if (tracePositionObjectList.Count <= i)
        //    {
        //        tracePositionObjectList.Add(Instantiate(tracePositionObject, agent.path.corners[i], Quaternion.identity));
        //    }
        //    else
        //    {
        //        tracePositionObjectList[i].transform.position = agent.path.corners[i];
        //    }
        //}

        //foreach (Vector3 pos in agent.path.corners)
        //{
        //    Instantiate(tracePositionObject, pos, Quaternion.identity);
        //}

        //Instantiate(tracePositionObject, agent.path.corners[(int)agent.path.corners.Length / 2], Quaternion.identity);
    }


    private float GetDistanceToTargetObject()
    {
        Vector3 dist = target.transform.position - transform.position;
        dist.y = 0;
        return Vector3.Magnitude(dist);//Distance(target.transform.position, transform.position);
        // コメント化理由：距離の計算対象が変わったため
        //if(agent.path.corners.Length > 1)
        //{
        //    //Debug.DrawLine(agent.transform.position, agent.path.corners[1], Color.red, 0.1f);
        //    return Vector3.Distance(agent.transform.position, agent.path.corners[1]);
        //}
        //return float.MaxValue;
    }

    private void UpdateQueue()
    {

    }

    private void UpdatePosition()
    {
        Vector3 direction = Vector3.zero;
        //Debug.Log("経路数：" + agent.path.corners.Length);
        if (agent.path.corners.Length >= 2)
        {
            // 進む角度を決める
            direction = agent.path.corners[1] - transform.position;
            //direction = agent.nextPosition - transform.position;
            direction.y = 0;
            direction = direction.normalized;
            //spawned.transform.position = agent.path.corners[agent.path.corners.Length - 2];
            //spawned.transform.position = agent.path.corners[1];
            //transform.position = Vector3.Lerp(transform.position, agent.path.corners[1], 0.85f);
            //spawned.transform.position = agent.path.corners[1];
        }
        // コメント化理由：たどり着きたいターゲットがあるとき，たどる地点の個数が必ず2以上あるため
        //else
        //{
        //    //Debug.Log("経路数：" + agent.path.corners.Length);
        //    direction = agent.path.corners[0] - transform.position;
        //    //direction = agent.nextPosition - transform.position;
        //    direction.y = 0;
        //    direction = direction.normalized;
        //    spawned.transform.position = agent.path.corners[0];
        //}
        transform.Translate(direction * speed * Time.deltaTime);
        
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
