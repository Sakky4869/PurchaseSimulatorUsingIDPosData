using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager3D : MonoBehaviour
{
    /// <summary>
    /// マウスがマップ上にあるかどうか
    /// ある：True
    /// ない：False
    /// </summary>
    [SerializeField]
    private bool mouseIn;

    /// <summary>
    /// 商品オブジェクトのプレハブ
    /// プレハブってなんやねん↓
    /// https://xr-hub.com/archives/11132
    /// </summary>
    [SerializeField]
    private Production3D production3DPrefab;

    /// <summary>
    /// 商品オブジェクトの親オブジェクトのTransform
    /// </summary>
    [SerializeField]
    private Transform productionObjectRoot;

    /// <summary>
    /// 商品情報登録パネル　3D版
    /// </summary>
    [SerializeField]
    private ProductionInfoPanel3D productionInfoPanel3D;

    [SerializeField]
    private RectTransform canvas;

    [SerializeField]
    private float convertScaleFrom2DTo3D;


    void Start()
    {
        //Config.operationMode = OperationMode.CONFIG;s
    }

    void Update()
    {
        // 操作モードが設定モードでなければreturn
        // returnとは？　Config.csの193行目
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        // 設置モードがPOINTでなければreturn
        if (Config.installMode != InstallMode.POINT)
            return;
        // 左クリックされた
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 3D商品オブジェクトを生成
            InstantiateProduction3D();
        }

    }

    /// <summary>
    /// クリックで商品オブジェクトを生成する
    /// ProductionIDの設定も行う
    /// </summary>
    private void InstantiateProduction3D()
    {
        // マウスがマップ外ならreturn
        if (mouseIn == false)
            return;
        // 画面上のマウスの位置をUnityの3D空間上の位置に変換
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Y座標がずれるので，補正
        pos.y = 6;
        Production3D production3D = Instantiate(production3DPrefab, pos, Quaternion.identity);
        production3D.gameObject.transform.SetParent(productionObjectRoot);
        
        // 時刻クラスを使って商品のIDを設定
        DateTime now = DateTime.Now;
        production3D.productionId = now.Year + ":" + now.Month + ":" + now.Day + ":" + now.Hour + ":" + now.Minute + ":" + now.Second + ":" + now.Millisecond;
        // 情報パネルの生成と商品オブジェクトへのセット
        ProductionInfoPanel3D panel3D = Instantiate(productionInfoPanel3D, transform.position, Quaternion.identity);
        panel3D.myRectTransform = panel3D.GetComponent<RectTransform>();
        panel3D.myRectTransform.SetParent(canvas);
        panel3D.myRectTransform.transform.position = Camera.main.WorldToScreenPoint(production3D.transform.position);
        panel3D.production3D = production3D;
        production3D.infoPanel = panel3D;
        production3D.infoPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// システムデータから読み取ったあとに商品オブジェクトを生成するメソッド
    /// </summary>
    /// <param name="productionObject">商品データ</param>
    public void InstantiateProduction3D(ProductionObject productionObject)
    {
        // 位置データを取得
        string[] positionData = productionObject.position.Split(',');

        // 位置決め
        // float.Parseってなんやねん↓
        // https://www.sejuku.net/blog/44977
        Vector3 pos = new Vector3(float.Parse(positionData[0]), 0, float.Parse(positionData[1]));
        Production3D production3D = Instantiate(production3DPrefab, pos, Quaternion.identity);
        production3D.transform.SetParent(productionObjectRoot);
        production3D.transform.localPosition = pos;

        // 商品情報代入
        production3D.productionName = productionObject.productionData.productionName;
        if (production3D.productionName == "exit")
        {
            // 出口用商品だったら，ゲームオブジェクトのタグをexitに設定
            production3D.gameObject.tag = "Exit";
        }
        production3D.metaData = productionObject.productionData.productionMetaData;
        production3D.productionId = productionObject.productionId;

        // 情報パネルの生成と商品オブジェクトへのセット
        ProductionInfoPanel3D panel3D = Instantiate(productionInfoPanel3D, transform.position, Quaternion.identity);
        panel3D.myRectTransform = panel3D.GetComponent<RectTransform>();
        panel3D.myRectTransform.SetParent(canvas);
        panel3D.myRectTransform.transform.position = Camera.main.WorldToScreenPoint(production3D.transform.position);
        panel3D.production3D = production3D;
        production3D.infoPanel = panel3D;
        //Debug.Log("assign info panel");
        production3D.SetValueToInfoPanel();
        production3D.infoPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// マップにマウスが入ったときに実行される
    /// </summary>
    public void OnPointerEnter()
    {
        //Debug.Log("mouse in");
        mouseIn = true;
    }

    /// <summary>
    /// マップからマウスが出たときに実行される
    /// </summary>
    public void OnPointerExit()
    {
        //Debug.Log("mouse out");
        mouseIn = false;
    }
}
