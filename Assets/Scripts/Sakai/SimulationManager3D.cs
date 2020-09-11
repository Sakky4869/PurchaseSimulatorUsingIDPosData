﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager3D : SimulationManager
{
    [SerializeField]
    private List<Production3D> productions3D;

    private DataManager3D dataManager3D;

    [SerializeField]
    private Customer3D customer3DPrefab;

    [SerializeField]
    private Transform entrance3D;


    void Start()
    {
        dataManager3D = GameObject.Find("DataManager3D").GetComponent<DataManager3D>();
        config = GameObject.Find("ConfigArea").GetComponent<Config>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Config.operationMode == OperationMode.CONFIG)
                return;
            isInSimulation = true;
            isPausedSimulation = false;
            StartSimulation(dataManager3D.iDPosDataRoot);
        }

        if (isInSimulation == false)
            return;

        if (isPausedSimulation == true)
            return;

        if (Config.operationMode == OperationMode.CONFIG)
            isInSimulation = false;

        CountTime();
    }


    private void StartSimulation(IDPosDataRoot iDPosDataRoot)
    {
        // 時刻を指定モードのときは，時刻を指定時刻にスキップ
        if (Config.isSpecifiedSimulation)
        {
            currentTime = config.GetSimulationStartTime();
        }

        if (productions3D == null || productions3D.Count == 0)
            productions3D = GetProductions();

        StartCoroutine(Simulate(iDPosDataRoot));
    }

    /// <summary>
    /// 顧客を生成する
    /// 3Dバージョン
    /// </summary>
    /// <param name="metaDatas"></param>
    protected override void InstantiateCustomer(List<ProductionData> metaDatas)
    {
        // 顧客オブジェクトを生成
        Customer3D customer3D = Instantiate(customer3DPrefab, transform.position, Quaternion.identity);
        customer3D.transform.position = entrance3D.position;

        // 購入した商品データから経路を探索し，顧客のオブジェクトに登録
        customer3D.RegisterTracePositions(SearchRoute(metaDatas));

        // シミュレーション開始
        customer3D.StartSimulation3D();

    }

    /// <summary>
    /// 商品オブジェクトを探してリストにして返す
    /// </summary>
    /// <returns></returns>
    private List<Production3D> GetProductions()
    {
        GameObject[] products3D = GameObject.FindGameObjectsWithTag("Production");
        List<Production3D> list = new List<Production3D>();
        foreach(GameObject product in products3D)
        {
            list.Add(product.GetComponent<Production3D>());
        }
        return list;
    }


    /// <summary>
    /// 商品のたどる順番を決定
    /// </summary>
    /// <param name="metaDatas">購入した商品データ</param>
    /// <returns>商品のキュー</returns>
    private Queue<Production3D> SearchRoute(List<ProductionData> metaDatas)
    {
        // 入り口に近いものから並べ替えるためのリスト
        List<Production3D> productions = new List<Production3D>();
        
        // 購入した商品が見つかったら商品リストに加える
        foreach(ProductionData data in metaDatas)
        {
            foreach(Production3D production3D in productions3D)
            {
                if(data.productionMetaData == production3D.metaData)
                {
                    productions.Add(production3D);
                    break;
                }
            }
        }

        // この時点で購入した商品のオブジェクトデータを取得できているので，
        // 入り口に近い順にソートをする

        Dictionary<float, Production3D> distanceDictionary = new Dictionary<float, Production3D>();
        foreach(Production3D production in productions)
        {
            // 商品ごとに入り口からの距離を計算して保存
            distanceDictionary.Add(Vector3.Distance(production.transform.position, entrance3D.position), production);
        }

        List<float> distances = new List<float>(distanceDictionary.Keys.ToArray<float>());
        // 小さい順に並べ替え
        distances.Sort();

        // この時点で各商品の入り口までの距離のリストが，値の小さい順に並べ替えられている
        // あとは，リストの先頭から順に商品をQueueに入れていけば，入り口から近い順に商品を並べ替えられる

        Queue<Production3D> productionQueue = new Queue<Production3D>();

        foreach(float dist in distances)
        {
            productionQueue.Enqueue(distanceDictionary[dist]);
        }

        return productionQueue;
    }



}