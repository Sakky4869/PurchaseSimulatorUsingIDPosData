using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer3D : MonoBehaviour
{
    /// <summary>
    /// たどるProducionクラスのQueue（キュー）
    /// キューってなんやねん↓
    /// https://csharp-ref.com/collection_queue.html
    /// アニメーションも一応↓
    /// https://1.bp.blogspot.com/-N-v_FiIdQXM/XlkFCQQYtPI/AAAAAAAAHR0/zxkuX6WfQS8Y8Mkoj1nHZDWtMOD3MjsUwCLcBGAsYHQ/s1600/0_E33E-AjyAUTFjVmM.gif
    /// </summary>
    //[HideInInspector]
    public Queue<Production3D> traceProductions3D;

    /// <summary>
    /// NavMeshAgentの変数
    /// NavMeshAgentは自動で経路探索してくれるやつ
    /// </summary>
    //[SerializeField]
    public NavMeshAgent agent;

    /// <summary>
    /// たどる商品のメタデータのList
    /// </summary>
    [SerializeField]
    private List<string> traceProductionList;

    /// <summary>
    /// SimulationManager3Dクラスの変数
    /// </summary>
    [HideInInspector]
    public SimulationManager3D simulationManager3D;

    /// <summary>
    /// 次に到達したいProduction3Dオブジェクト
    /// </summary>
    [SerializeField]
    private Production3D targetProduction3D;

    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField]
    private float moveSpeed;

    /// <summary>
    /// 現在購入中かどうか
    /// </summary>
    private bool isShopping;

    /// <summary>
    /// 到達したかどうかを判定する距離
    /// エージェントと商品の距離がこれより小さくなったら到達したことにする
    /// </summary>
    [SerializeField]
    private float achievedDistance;

    /// <summary>
    /// 出口のTransform
    /// Transformってなんやねん↓
    /// https://www.sejuku.net/blog/50983
    /// </summary>
    [SerializeField]
    private Transform exitTransform;

    /// <summary>
    /// NavMeshAgentが経路探索を行っているかどうかをUnity側から見れるようにする変数
    /// あまり気にしなくていい
    /// </summary>
    [SerializeField]
    private bool pathpending;

    /// <summary>
    /// エージェントのRigidody
    /// Rigidodyってなんやねん↓
    /// https://www.sejuku.net/blog/51770
    /// </summary>
    private Rigidbody rigidbody;

    /// <summary>
    /// 経路が確定しているかどうか
    /// </summary>
    private bool isGetPath;

    /// <summary>
    /// 経路探索を開始したかどうか
    /// </summary>
    private bool isStartedGettingPath;

    /// <summary>
    /// 最初の商品に到達したかどうか
    /// </summary>
    [HideInInspector]
    public bool isGetFirstProduction;

    /// <summary>
    /// 出口に向かっているかどうか
    /// </summary>
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
        // もし購入中でなければreturn
        // returnとは？　Config.csの193行目
        if (isShopping == false)
            return;

        // もしシミュレーション中でなければreturn
        if (simulationManager3D.isInSimulation == false)
            return;

        // もしシミュレーションが一時停止中でなければreturn
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

        // 到達目標の商品が変数にないとい
        if (targetProduction3D == null)
        {
            // たどる予定の商品がまだあれば
            if (traceProductions3D.Count != 0)
            {
                // キューから取り出す
                targetProduction3D = traceProductions3D.Dequeue();
            }
            // たどる予定の商品がないとき
            else
            {
                // 出口のオブジェクトがないなら取得
                if (exitTransform == null)
                {
                    exitTransform = GameObject.FindGameObjectWithTag("Exit").transform;
                    isGettingExit = true;
                }

                // NavMeshAgentはデフォルトではリアルタイムに経路探索をしてしまうので，経路探索のタイミング
                // を独自に調整
                
                // 経路が確定していないとき
                if(isGetPath == false)
                {
                    // 経路探索を開始していなければ
                    if(isStartedGettingPath == false)
                    {
                        // 経路を探索する
                        agent.SetDestination(exitTransform.position);
                        isStartedGettingPath = true;
                    }
                }
                // NavMeshAgentが経路探索を行っているとき
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
                // 経路がわかっていないとき
                if (isGetPath == false)
                {
                    // ここのプログラムが実行されているなら，経路探索が終わっているので，経路がわかっている状態にする
                    isGetPath = true;
                    isStartedGettingPath = false;
                }
                

                // 商品に到着していないとき
                if (GetDistanceToTargetObject() > achievedDistance)
                {
                    // 商品に向かって移動
                    UpdatePosition();
                }
                else
                {
                    //simulationManager3D.AddExit();
                    // 出口についたのでエージェントの数を減らす
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

    /// <summary>
    /// シミュレーション開始
    /// </summary>
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

    /// <summary>
    /// たどる商品の情報を登録する
    /// </summary>
    /// <param name="productionQueue">たどるProducion3Dオブジェクトのキュー</param>
    public void RegisterTracePositions(Queue<Production3D> productionQueue)
    {
        if (traceProductions3D == null)
            traceProductions3D = new Queue<Production3D>();

        if (traceProductionList == null)
            traceProductionList = new List<string>();

        // キューのすべての要素について
        while(productionQueue.Count != 0)
        {
            // 取り出す
            Production3D production3D = productionQueue.Dequeue();
            
            // 入れる
            traceProductions3D.Enqueue(production3D);

            // メタデータをリストに追加
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
        // 商品
        if (targetProduction3D)
        {
            dist = targetProduction3D.transform.position - transform.position;
        }
        // 出口
        else
        {
            dist = exitTransform.position - transform.position;
        }
        dist.y = 0;
        // Vector3.Magnitudeってなんやねん↓
        // https://xr-hub.com/archives/12115
        // このサイト，なんか微妙．．．
        return Vector3.Magnitude(dist);
    }

    /// <summary>
    /// 目標に向かって進む
    /// </summary>
    private void UpdatePosition()
    {
        Vector3 direction = Vector3.zero;

        // 進む方向を計算
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
