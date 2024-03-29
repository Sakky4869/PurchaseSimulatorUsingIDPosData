﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    /// <summary>
    /// マウスがマップの上にあるかどうか
    /// ある：True
    /// ない：False
    /// </summary>
    private bool mouseIn;

    /// <summary>
    /// Productionのプレハブ
    /// プレハブとは？　MapManager3D.csの19行目
    /// </summary>
    [SerializeField]
    private Production productionPrefab;

    //[SerializeField]
    //private 

    //[SerializeField]
    /// <summary>
    /// Productionオブジェクトの親のRectTransform
    /// </summary>
    public RectTransform productionRoot;

    //private List<Production> productions;

    /// <summary>
    /// Linkのプレハブ
    /// </summary>
    [SerializeField]
    private Link linkPrefab;

    /// <summary>
    /// 現在選択されているProuductionオブジェクト
    /// </summary>
    [HideInInspector]
    public Production selectedProduction;


    void Start()
    {

    }

    void Update()
    {
        // 操作モードが設定モードでなければreturn
        // returnとは？　Config.csの193行目
        if (Config.operationMode != OperationMode.CONFIG)
            return;

        // 左クリックされたとき
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 設置モードがPOINTなら
            if(Config.installMode == InstallMode.POINT)
            {
                InstantiateProduction();
            }
        }
    }

    /// <summary>
    /// クリックで生成するときに使う
    /// ProductionIDの設定も行う
    /// </summary>
    private void InstantiateProduction()
    {
        // マウスがマップ外ならreturn
        if (mouseIn == false)
            return;
        Production production = Instantiate(productionPrefab, Input.mousePosition, Quaternion.identity, productionRoot);
        production.rectTransform = production.GetComponent<RectTransform>();
        production.rectTransform.localScale = Vector3.one;
        DateTime now = DateTime.Now;
        production.productionId = now.Year + ":" + now.Month + ":" + now.Day + ":" + now.Hour + ":" + now.Minute + ":" + now.Second + ":" + now.Millisecond;
        //production.productionInfoPanel.gameObject.SetActive(false);
        SortProductionsAndLinks();
    }

    /// <summary>
    /// 起動時に状態を復元するときに使う
    /// </summary>
    /// <param name="productionObject">商品データ</param>
    public void InstantiateProduction(ProductionObject productionObject)
    {
        // 設置位置のデータを取得
        string[] positionData = productionObject.position.Split(',');
        //Debug.Log(positionData[0] + "," + positionData[1]);
        Vector2 pos = new Vector2(float.Parse(positionData[0]), float.Parse(positionData[1]));
        Production production = Instantiate(productionPrefab, pos, Quaternion.identity, productionRoot);
        production.rectTransform = production.GetComponent<RectTransform>();
        production.rectTransform.anchoredPosition = pos;
        production.rectTransform.localScale = Vector3.one;
        
        production.productionName = productionObject.productionData.productionName;
        //Debug.Log("production name : " + production.productionName);
        production.metaData = productionObject.productionData.productionMetaData;
        //Debug.Log("meta data : " + production.metaData);
        production.productionId = productionObject.productionId;
        production.SetValueToInfoPanel();
        production.productionInfoPanel.gameObject.SetActive(false);
        SortProductionsAndLinks();
    }

    /// <summary>
    /// Linkを生成
    /// </summary>
    /// <param name="first">始点のProduction</param>
    /// <param name="second">終点のProduction</param>
    public void InstantiateLink(Production first, Production second)
    {
        Link link = Instantiate(linkPrefab, first.rectTransform.anchoredPosition, Quaternion.identity, productionRoot);
        link.rectTransform = link.GetComponent<RectTransform>();
        link.rectTransform.localScale = Vector3.one;
        link.DrawLink(first, second);
        link.firstProduction = first;
        first.links.Add(link);
        link.secondProduction = second;
        second.links.Add(link);
        //Debug.Log("first : " + first.links.Count);
        //Debug.Log("second : " + second.links.Count);
        //Debug.Log("call");
        //return link;
        SortProductionsAndLinks();
    }

    /// <summary>
    /// システムデータからLinkを復活させるときに使う
    /// </summary>
    /// <param name="linkObject">システムデータ内のLinkのデータ</param>
    public void InstantiateLink(LinkObject linkObject)
    {
        Production first = null;
        Production second = null;
        foreach(GameObject product in GameObject.FindGameObjectsWithTag("Production"))
        {
            Production production = product.GetComponent<Production>();
            //Debug.Log("debug");
            //Debug.Log(production.productionId + " , " + linkObject.firstProductionId);
            //Debug.Log(production.productionId + " , " + linkObject.secondProductionId);
            if(production.productionId == linkObject.firstProductionId)
            {
                first = production;
                if (second != null)
                    break;
                continue;
                //Debug.Log("find first");
            }
            else if(production.productionId == linkObject.secondProductionId)
            {
                second = production;
                if (first != null)
                    break;
                continue;
                //Debug.Log("find second");
            }
        }

        //Debug.Log(first + " , " + second);
        if(first != null && second != null)
        {
            InstantiateLink(first, second);
        }
    }

    /// <summary>
    /// 設置した商品とリンクの階層関係をソートして，商品が手前に来るようにする
    /// Unityの2Dでは，下にあるものほど手前に表示される
    /// </summary>
    private void SortProductionsAndLinks()
    {
        List<Production> productions = new List<Production>();
        List<Link> links = new List<Link>();

        for(int i = 0; i < productionRoot.childCount; i++)
        {
            // オブジェクトが商品だった場合
            if(productionRoot.GetChild(i).GetComponent<Production>() != null)
            {
                productions.Add(productionRoot.GetChild(i).GetComponent<Production>());
            }
            // オブジェクトがリンクだった場合
            else if(productionRoot.GetChild(i).GetComponent<Link>() != null)
            {
                links.Add(productionRoot.GetChild(i).GetComponent<Link>());
            }
        }

        // 商品がリンクよりも手前になるように並べ替える
        int index = 2;

        foreach(Link link in links)
        {
            link.transform.SetSiblingIndex(index);
            index++;
        }

        foreach(Production production in productions)
        {
            production.transform.SetSiblingIndex(index);
            index++;
        }
    }



    public void OnPointerEnter()
    {
        mouseIn = true;
    }

    public void OnPoiterExit()
    {
        mouseIn = false;
    }
}
