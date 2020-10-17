using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Customer : MonoBehaviour
{
    /// <summary>
    /// たどるProducionクラスのQueue（キュー）
    /// キューってなんやねん↓
    /// https://csharp-ref.com/collection_queue.html
    /// アニメーションも一応↓
    /// https://1.bp.blogspot.com/-N-v_FiIdQXM/XlkFCQQYtPI/AAAAAAAAHR0/zxkuX6WfQS8Y8Mkoj1nHZDWtMOD3MjsUwCLcBGAsYHQ/s1600/0_E33E-AjyAUTFjVmM.gif
    /// </summary>
    [HideInInspector]// [HideInInspector]はUnity使いながらのほうがやりやすいのでここでは省略
    public Queue<Production> traceProductions;

    /// <summary>
    /// たどる商品のデータのList
    /// </summary>
    public List<string> traceProductionList;


    //[HideInInspector]
    //public List<string> dataStrings;

    /// <summary>
    /// SimulationManagerクラスの変数
    /// </summary>
    [HideInInspector]
    public SimulationManager simulationManager;

    /// <summary>
    /// 次に到達した商品オブジェクト
    /// </summary>
    private Production targetProduction;

    /// <summary>
    /// 自分のRectTransform
    /// RectTransformってなんやねん↓
    /// https://xr-hub.com/archives/11543
    /// </summary>
    public RectTransform rectTransform;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// 現在購入中かどうか
    /// </summary>
    [HideInInspector]
    public bool isShopping;

    /// <summary>
    /// 出口のRectTransform
    /// </summary>
    private RectTransform exitRectTransform;
    
    /// <summary>
    /// 到達したかどうかを判定する距離
    /// エージェントと商品の距離がこれより小さくなったら到達したことにする
    /// </summary>
    [SerializeField]
    private float achievedDistance;

    void Start()
    {
        //traceProductions = new Queue<Production>();
        // SimulationManagerを取得
        // 詳しくはConfig.csの147行目
        simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        //rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // もし購入中でなければreturn
        // returnとは？　Config.csの193行目
        if (isShopping == false)
            return;
        // もしシミュレーション中でなければreturn
        if (simulationManager.isInSimulation == false)
            return;
        // もしシミュレーションが一時停止中でなければreturn
        if (simulationManager.isPausedSimulation == true)
            return;

        // 到達目標の商品が変数にないとき
        if(targetProduction == null)
        {
            // たどる商品がなくなってないとき
            if(traceProductions.Count != 0)
            {
                // キューから取り出す
                targetProduction = traceProductions.Dequeue();
                //Debug.Log("Next Production : " + targetProduction.metaData);
                targetProduction.image.color = Color.red;
            }
            // たどる商品がなくなったら出口へ
            else
            {
                //Debug.Log("exit");

                exitRectTransform = GameObject.Find("Exit").GetComponent<RectTransform>();
            }
        }
        else
        {
            // 到達目標の商品があれば
            if (targetProduction)
            {
                // 移動する方向を計算
                // 到達目標の商品の位置のベクトル　－　自分の位置のベクトルを計算
                // 最後のnormalizedで正規化（大きさを１にする）
                Vector2 direct = (targetProduction.rectTransform.anchoredPosition - rectTransform.anchoredPosition).normalized;
                
                // 計算した方向に進む
                rectTransform.anchoredPosition += direct * moveSpeed;
            }
            
            // ターゲット商品に到着したら次の商品をターゲットにする
            if(IsGetTargetProduction(targetProduction, achievedDistance))
            {
                targetProduction.image.color = Color.blue;
                targetProduction = null;
            }
        }
        
        // 出口に向かうとき
        if (exitRectTransform)
        {
            Vector2 direct = (exitRectTransform.anchoredPosition - rectTransform.anchoredPosition).normalized;
            rectTransform.anchoredPosition += direct * moveSpeed;
        }
    }

    /// <summary>
    /// 買い物状態にする
    /// </summary>
    public void StartSimulation()
    {
        isShopping = true;
    }

    /// <summary>
    /// たどる場所を登録する
    /// </summary>
    /// <param name="datas">Productionクラスのキュー</param>
    public void RegisterTracePositions(Queue<Production> productionQueue)
    {
        // もしQueueの変数に値がないときはnew
        if (traceProductions == null)
            traceProductions = new Queue<Production>();
        //Debug.Log(productionQueue.Count);
        
        // もしListの変数に値がないときはnew
        if (traceProductionList == null)
            traceProductionList = new List<string>();
        
        // Queueの中身がなくなるまで
        while(productionQueue.Count != 0)
        {
            // 取り出す
            Production p = productionQueue.Dequeue();

            // 入れる
            traceProductions.Enqueue(p);

            // メタデータをListに追加
            traceProductionList.Add(p.metaData + " " + p.productionName);
        }

    }
    
    /// <summary>
    /// 到達目標の商品にたどり着いたかどうかを判定する
    /// </summary>
    /// <param name="target">到達目標の商品のクラス</param>
    /// <param name="distance">到達したかどうかを判定する距離</param>
    /// <returns></returns>
    private bool IsGetTargetProduction(Production target, float distance)
    {
        // 距離計算
        // Vector2.Distanceについて↓
        // https://loumo.jp/archives/5347
        float dist = Vector2.Distance(rectTransform.anchoredPosition, target.rectTransform.anchoredPosition);
        return dist < distance;
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag != "Production")
    //        return;
    //    // 移動先の商品にたどりついたら，ターゲット変数の初期化
    //    if(collision.GetComponent<Production>() == targetProduction)
    //    {
    //        //Debug.Log(targetProduction.metaData);
    //        targetProduction.image.color = Color.blue;
    //        targetProduction = null;
    //    }
    //}

}
