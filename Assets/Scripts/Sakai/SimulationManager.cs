using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SimulationManager : MonoBehaviour
{
    /// <summary>
    /// 商品の名前と位置情報を紐づけて保存する辞書配列
    /// </summary>
    private Dictionary<string, Vector2> productPositions;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private List<Production> productions;

    private List<Link> links;

    /// <summary>
    /// シミュレーション速度（というよりUnityのタイムスケール）
    /// </summary>
    public float simulationSpeed { get { return Time.timeScale; } set { Time.timeScale = value; } }

    [SerializeField]
    private Button startSimulationButton;

    //private Production nearestProductionFromEntrance;

    [SerializeField]
    private Customer customerPrefab;

    [SerializeField]
    private MapManager mapManager;

    [SerializeField]
    private RectTransform entrance;

    /// <summary>
    /// 現在の時刻
    /// 年：月：日：時：分
    /// </summary>
    public string currentTime;

    /// <summary>
    /// 1分のカウント
    /// 5カウントで1分の計算
    /// </summary>
    public int minuteCount;

    [HideInInspector]
    public bool isInSimulation;

    [HideInInspector]
    public bool isPausedSimulation;

    private DataManager dataManager;

    [SerializeField]
    private Text timeText;

    private Config config;

    [SerializeField, Range(0, 1)]
    private float timeScale;

    void Start()
    {
        minuteCount = 0;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        config = GameObject.Find("ConfigArea").GetComponent<Config>();

        Time.timeScale = timeScale;
        
    }

    void Update()
    {
        Time.timeScale = timeScale;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Config.operationMode == OperationMode.CONFIG)
                return;
            isInSimulation = true;
            isPausedSimulation = false;
            StartSimulation(dataManager.iDPosDataRoot);

            //SearchRoute("a,b,c,d", "aaa,bbb,ccc,ddd");
        }

        if (isInSimulation == false)
            return;

        if (isPausedSimulation == true)
            return;

        if (Config.operationMode == OperationMode.CONFIG)
            isInSimulation = false;

        CountTime();
        
    }

    /// <summary>
    /// 時刻のカウント
    /// 
    /// </summary>
    private void CountTime()
    {
        timeText.text = currentTime;

        minuteCount++;
        if (minuteCount > 5)
        {
            minuteCount = 0;
            //Debug.Log(currentTime);
            string[] dataStr = currentTime.Split(':');
            int[] data = new int[dataStr.Length];
            for (int i = 0; i < dataStr.Length; i++)
            {
                data[i] = int.Parse(dataStr[i]);
            }

            // 分を進める
            data[4]++;
            // 分がこえる
            if (data[4] == 60)
            {
                data[4] = 0;
                // 時を進める
                data[3]++;
            }
            // 時がこえる
            if (data[3] == 24)
            {
                data[3] = 0;
                data[2]++;
            }

            // 日がこえる
            // 月とうるう年かどうかで判断
            switch (data[1])
            {
                case 2:
                    bool flag = false;
                    // うるう年の判定
                    if (data[0] % 4 == 0)
                    {
                        flag = true;
                        if (data[0] % 100 == 0)
                        {
                            flag = false;
                            if (data[0] % 400 == 0)
                            {
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        flag = false;
                    }

                    if (flag)
                    {
                        if (data[2] == 30)
                        {
                            data[2] = 1;
                            data[1]++;
                        }
                    }
                    else
                    {
                        if (data[2] == 29)
                        {
                            data[2] = 1;
                            data[1]++;
                        }
                    }
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    // 1か月の30日の場合
                    if (data[2] == 31)
                    {
                        data[2] = 1;
                        data[1]++;
                    }
                    break;
                default:
                    // 1か月が31日の場合
                    if (data[2] == 32)
                    {
                        data[2] = 1;
                        data[1]++;
                    }
                    break;
            }

            // 月がこえる
            if (data[1] == 13)
            {
                data[1] = 1;
                data[0]++;
            }

            currentTime = "" + data[0] + ":" + data[1] + ":" + data[2] + ":" + data[3] + ":" + data[4];

        }
    }

    /// <summary>
    /// シミュレーション開始の直前にすべてのリンクを隠す
    /// </summary>
    private void HideAllLinks()
    {
        foreach(Link link in links)
        {
            // Imageコンポーネントの無効化をすれば見えなくなる
            link.GetComponent<Image>().enabled = false;
        }
    }

    
    /// <summary>
    /// シミュレーションの開始メソッド
    /// </summary>
    /// <param name="iDPosDataRoot">読み込んだID-POSデータ</param>
    private void StartSimulation(IDPosDataRoot iDPosDataRoot)
    {

        links = GetLinks();
        HideAllLinks();

        // 時刻を指定モードのときは、時刻を指定時刻にスキップ
        if (Config.isSpecifiedSimulation)
        {
            currentTime = config.GetSimulationStartTime();
        }

        StartCoroutine(Simulate(iDPosDataRoot));
    }

    private IEnumerator Simulate(IDPosDataRoot iDPosDataRoot)
    {
        foreach(IdPosYear idPosYear in iDPosDataRoot.yearDatas)
        {
            foreach(IdPosMonth idPosMonth in idPosYear.monthDatas)
            {
                if (idPosMonth.month == 0)
                    continue;
                foreach(IdPosDay idPosDay in idPosMonth.dayDatas)
                {
                    if (idPosDay.day == 0)
                        continue;
                    foreach(IdPosHour idPosHour in idPosDay.hourDatas)
                    {
                        if (idPosHour.hour == 0)
                            continue;
                        foreach(IDPosData iDPosData in idPosHour.iDPosDatas)
                        {
                            // 時刻指定があった場合は、指定時刻になるまでスキップ
                            if (Config.isSpecifiedSimulation)
                            {
                                if(IsOverTime(currentTime, iDPosData.entranceTime) == false)
                                {
                                    //yield return null;
                                    continue;
                                }
                            }
                            //Debug.Log("入店:" + iDPosData.entranceTime);
                            bool flag = false;
                            do
                            {
                                // シミュレーションが停止している間は待機
                                while (isInSimulation == false)
                                    yield return null;

                                if (currentTime == iDPosData.entranceTime)
                                {
                                    flag = true;

                                    //Debug.Log("入店");

                                    InstantiateCustomer(iDPosData.productionDatas);

                                }
                                yield return null;
                            } while (flag == false);
                            
                        }
                    }
                }
            }
        }
        //InstantiateCustomer();
        yield return null;
    }

    /// <summary>
    /// 現在時刻が目標時刻を超えているかどうか
    /// </summary>
    /// <param name="current">現在時刻</param>
    /// <param name="target">目標時刻</param>
    /// <returns></returns>
    private bool IsOverTime(string current, string target)
    {
        int[] currentTimeInt = current.Split(':').Select(int.Parse).ToArray();
        int[] targetTimeInt = target.Split(':').Select(int.Parse).ToArray();

        for(int i = 0; i < currentTimeInt.Length; i++)
        {
            if (currentTimeInt[i] >= targetTimeInt[i])
                return true;
        }

        return false;
    }

    /// <summary>
    /// 顧客を生成する
    /// </summary>
    /// <param name="metaDatas">購入した商品のデータ</param>
    private void InstantiateCustomer(List<ProductionData> metaDatas)
    {
        // 顧客のオブジェクトを生成
        Customer customer = Instantiate(customerPrefab, transform.position, Quaternion.identity, mapManager.productionRoot);
        customer.rectTransform.localScale = Vector3.one;
        customer.rectTransform.anchoredPosition = entrance.anchoredPosition;

        Debug.Log(metaDatas.Count);

        // 購入した商品データから経路を探索し，顧客のオブジェクトに登録
        for(int i = 0; i < metaDatas.Count - 1; i++)
        {
            Debug.Log("start : " + metaDatas[i].productionMetaData + " , goal : " + metaDatas[i + 1].productionMetaData);
            Queue<Production> traceProductions = SearchRoute(metaDatas[i].productionMetaData, metaDatas[i + 1].productionMetaData);
            customer.RegisterTracePositions(traceProductions);
        }

        // 顧客オブジェクトのシミュレーション開始
        customer.StartSimulation();
    }


    /// <summary>
    /// 入口に最も近い商品を探す
    /// </summary>
    /// <returns></returns>
    //private Production GetNearestProductionFromExtrance()
    //{
    //    GameObject[] products = GameObject.FindGameObjectsWithTag("Production");
    //    Production[] prods = new Production[products.Length];
    //    for(int i = 0; i < products.Length; i++)
    //    {
    //        prods[i] = products[i].GetComponent<Production>();
    //    }
    //    productions = new List<Production>(prods);

    //    GameObject entrance = GameObject.Find("Entrance");
    //    float minDist = float.MaxValue;
    //    Production production = null;
    //    foreach(Production prod in productions)
    //    {
    //        float dist = Vector2.Distance(entrance.GetComponent<RectTransform>().anchoredPosition, prod.rectTransform.anchoredPosition);
    //        if(dist < minDist)
    //        {
    //            minDist = dist;
    //            production = prod;
    //        }
    //    }

    //    return production;
    //}

    /// <summary>
    /// 商品をすべて取得し，リストにして返す
    /// </summary>
    /// <returns></returns>
    private List<Production> GetProductions()
    {
        GameObject[] products = GameObject.FindGameObjectsWithTag("Production");
        List<Production> list = new List<Production>();
        for (int i = 0; i < products.Length; i++)
        {
            list.Add(products[i].GetComponent<Production>());
            //prods[i] = products[i].GetComponent<Production>();
        }
        return list;
    }

    /// <summary>
    /// 商品と商品をつなぐリンクをすべて取得し，リストにして返す
    /// </summary>
    /// <returns></returns>
    private List<Link> GetLinks()
    {
        return new List<Link>(GameObject.FindObjectsOfType<Link>());
    }

    /// <summary>
    /// ある商品からある商品への経路の探索
    /// </summary>
    /// <param name="start">開始地点のメタデータ</param>
    /// <param name="goal">ゴール地点のメタデータ</param>
    /// <returns>経路になる商品のリスト</returns>
    private Queue<Production> SearchRoute(string start, string goal)
    {
        // 全商品を取得
        productions = GetProductions();
        
        // 開始地点の商品を探してコストを０にする
        // それ以外の商品はコストを-1にする
        foreach(Production production in productions)
        {
            if(production.metaData == start)
            {
                production.cost = 0;
                production.beforeProduction = null;
                //Debug.Log("start");
            }
            else
            {
                production.cost = -1;
                production.beforeProduction = null;
                //Debug.Log("other");
            }
        }

        while (true)
        {
            // 現在処理中の商品
            Production processProduction = null;

            // 全商品に対して確認/更新を行う
            for(int i = 0; i < productions.Count; i++)
            {
                Production production = productions[i];

                // 訪問済み　or コスト未設定の場合はスキップ
                if(production.isConst || production.cost < 0)
                {
                    continue;
                }

                // 処理中の商品がない場合は現在の商品を保持して次へ
                if (processProduction == null)
                {
                    //Debug.Log("いったん保持");
                    processProduction = production;
                    continue;
                }

                // 訪問済みでない商品のうち，一番コストの小さい商品を探す
                if(production.cost < processProduction.cost)
                {
                    //Debug.Log("低コストの発見");
                    processProduction = production;
                }

            }

            // 処理中の商品がなくなったら（すべてのチェックが終わったらループ終了）
            if (processProduction == null)
            {
                break;
            }

            // 処理中の商品に訪問済みのフラグを設定
            processProduction.isConst = true;


            // コストのアップデート
            // 選択された商品（proceccProduction）の現在のコストと
            // 接続されているリンクのコストを足し，それを接続先の商品のコストと比較
            // コストが小さい場合はその値にアップデートする
            for(int i = 0; i < processProduction.links.Count; i++)
            {
                // リンク先の商品を選択
                Production production = processProduction.links[i].GetOpponent(processProduction);
                //if (processProduction.links[i].firstProduction.productionId == processProduction.productionId)
                //    production = processProduction.links[i].secondProduction;
                //else if(processProduction.links[i].secondProduction.productionId == processProduction.productionId)
                //    production = processProduction.links[i].firstProduction;

                // コストを計算
                float cost = processProduction.cost + processProduction.links[i].rectTransform.sizeDelta.x;
                //Debug.Log(cost);
                //Debug.Log("current cost : " + production.cost);

                // コストが未設定 or コストの少ない経路がある場合はアップデート
                if (production.cost < 0 || cost < production.cost)
                {
                    production.cost = cost;
                    production.beforeProduction = processProduction;
                    //Debug.Log("update " + production.productionName);
                }
            }
        }

        // この時点で経路が検索済みになる
        // ゴール商品を選択
        Production goalProduction = null;
        foreach(Production prod in productions)
        {
            if(prod.metaData == goal)
            {
                goalProduction = prod;
            }
        }

        //if (goalProduction == null)
        //    Debug.Log("goal is null");

        Stack<Production> traceProductionStack = new Stack<Production>();
        string path = "Goal -> ";
        Production currentProduction = goalProduction;
        traceProductionStack.Push(goalProduction);
        while (true)
        {
            // 現在登録しようとしているProductionの前のProductionがないとき，つまり最初のProductionのときはwhileを抜ける
            if (currentProduction.beforeProduction == null)
            {
                path += " Start";
                break;
            }
            Production nextProduction = currentProduction.beforeProduction;
            // もともとここで抜けるかどうかの判定をしていたが，ここの前でエラーが出るので上に変更
            //if (!nextProduction)
            //{
            //    path += " Start";
            //    break;
            //}

            traceProductionStack.Push(nextProduction);

            path += nextProduction.productionName + " -> ";
            currentProduction = nextProduction;
        }

        Queue<Production> traceProductionQueue = new Queue<Production>();
        while(traceProductionStack.Count != 0)
        {
            Production p = traceProductionStack.Pop();
            traceProductionQueue.Enqueue(p);
        }


        //Debug.Log(path);

        return traceProductionQueue;
    }


}
