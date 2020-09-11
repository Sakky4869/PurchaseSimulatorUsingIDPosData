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
    /// </summary>
    [SerializeField]
    private Production3D production3DPrefab;

    [SerializeField]
    private Transform productionObjectRoot;

    [SerializeField]
    private ProductionInfoPanel3D productionInfoPanel3D;

    [SerializeField]
    private RectTransform canvas;


    void Start()
    {
        //Config.operationMode = OperationMode.CONFIG;s
    }

    void Update()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        if (Config.installMode != InstallMode.POINT)
            return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            InstantiateProduction3D();
        }

    }

    /// <summary>
    /// クリックで商品オブジェクトを生成する
    /// ProductionIDの設定も行う
    /// </summary>
    private void InstantiateProduction3D()
    {
        if (mouseIn == false)
            return;
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.y = 6;
        Production3D production3D = Instantiate(production3DPrefab, pos, Quaternion.identity);
        production3D.gameObject.transform.SetParent(productionObjectRoot);
        DateTime now = DateTime.Now;
        production3D.productionId = now.Year + ":" + now.Month + ":" + now.Day + ":" + now.Hour + ":" + now.Minute + ":" + now.Second + ":" + now.Millisecond;
    }

    /// <summary>
    /// システムデータから読み取ったあとに商品オブジェクトを生成するメソッド
    /// </summary>
    /// <param name="productionObject">商品データ</param>
    public void InstantiateProduction3D(ProductionObject productionObject)
    {
        // 位置決め
        string[] positionData = productionObject.position.Split(',');
        Vector3 pos = new Vector3(float.Parse(positionData[0]), 6, float.Parse(positionData[1]));
        Production3D production3D = Instantiate(production3DPrefab, pos, Quaternion.identity);
        production3D.transform.SetParent(productionObjectRoot);
        production3D.transform.localPosition = pos;
        // 商品情報代入
        production3D.productionName = productionObject.productionData.productionName;
        production3D.metaData = productionObject.productionData.productionMetaData;
        production3D.productionId = productionObject.productionId;
        // 情報パネルの生成と商品オブジェクトへのセット
        ProductionInfoPanel3D panel3D = Instantiate(productionInfoPanel3D, transform.position, Quaternion.identity);
        panel3D.myRectTransform = panel3D.GetComponent<RectTransform>();
        panel3D.myRectTransform.SetParent(canvas);
        panel3D.myRectTransform.transform.position = Camera.main.WorldToScreenPoint(production3D.transform.position);
        panel3D.production3D = production3D;
        production3D.infoPanel = panel3D;
    }

    public void OnPointerEnter()
    {
        //Debug.Log("mouse in");
        mouseIn = true;
    }

    public void OnPointerExit()
    {
        //Debug.Log("mouse out");
        mouseIn = false;
    }
}
