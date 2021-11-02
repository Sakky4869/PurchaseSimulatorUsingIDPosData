using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Production3D : MonoBehaviour
{
    /// <summary>
    /// ドラッグ中かどうか
    /// </summary>
    private bool isDragging;

    /// <summary>
    /// マップの外に出たかどうか
    /// </summary>
    [SerializeField]
    private bool isOutOfMap;


    /// <summary>
    /// 商品名
    /// </summary>
    //[HideInInspector]
    public string productionName;

    /// <summary>
    /// 部門・AU・ラインをつなげたデータ
    /// </summary>
    //[HideInInspector]
    public string metaData;

    [HideInInspector]
    public string productionId;

    /// <summary>
    /// ドラッグの開始位置
    /// </summary>
    private Vector3 dragBeginPosition;

    private MapManager mapManager;

    /// <summary>
    /// 商品のY座標を固定するため
    /// </summary>
    private GameObject productionOrigin;

    /// <summary>
    /// 商品の情報を入力するパネル
    /// </summary>
    //[HideInInspector]
    public ProductionInfoPanel3D infoPanel;


    void Start()
    {
        // 変数の初期化
        isDragging = false;
        isOutOfMap = false;
        productionOrigin = GameObject.Find("ProductionOrigin");
        //mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        if(productionName == null)
        {
            productionName = "";
        }

        if(metaData == null)
        {
            metaData = "";
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 商品のデータを情報パネルのInputFieldに設定する
    /// </summary>
    public void SetValueToInfoPanel()
    {
        //Debug.Log(metaData);
        string[] data = metaData.Split(',');
        infoPanel.SetValueToInputField(data[0], data[1], data[2], data[3], productionName);
        infoPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 押された瞬間の処理
    /// </summary>
    public void OnPointerDown()
    {
        // 操作モードが設定モードのときのみ操作
        if (Config.operationMode != OperationMode.CONFIG)
            return;

        // 設置モードごとで挙動を変える
        switch (Config.installMode)
        {
            // コメント化理由：これを実行するのはOnPointerClickのところ
            //// 削除モードのとき
            //case InstallMode.DELETE:
            //    // 情報パネルを削除
            //    if (infoPanel != null)
            //        Destroy(infoPanel);
                
            //    //シンプルに削除 
            //    Destroy(gameObject);

            //    break;
            // 捜査モードのとき
            case InstallMode.OPERATION:

                // ドラッグ開始
                isDragging = true;

                break;
            case InstallMode.POINT:
                break;
            // リンクモードのとき（これは2Dのときにリンクをつなげるのに使用していたが，3Dでは不要になったので今は未使用）
            // バグを防ぐためにコードは残す
            case InstallMode.LINK:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ドラッグ開始時の処理
    /// </summary>
    public void OnPointerDragBegin()
    {
        //Config.operationMode = OperationMode.CONFIG;
        //Config.installMode = InstallMode.OPERATION;
        // 操作モードが設定モードのときのみ操作
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        // 設置モードが移動モードのとき
        if(Config.installMode == InstallMode.OPERATION)
        {
            // ドラッグ開始時の位置を記録：マップ外に出たときに戻ってこれるようにするため
            dragBeginPosition = transform.localPosition;
        }
    }

    /// <summary>
    /// ドラッグ中の処理
    /// </summary>
    public void OnPointerDrag()
    {
        //Config.operationMode = OperationMode.CONFIG;
        //Config.installMode = InstallMode.OPERATION;
        // 操作モードが設定モードのときのみ操作
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        if(Config.installMode == InstallMode.OPERATION)
        {
            // マウスの座標を代入
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Y座標を固定
            Vector3 localPos = transform.localPosition;
            //localPos.y = 0;
            localPos.y = 6;
            transform.localPosition = localPos;

            infoPanel.myRectTransform.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    /// <summary>
    /// ドラッグ終了時の処理
    /// </summary>
    public void OnPointerDragEnd()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        if(Config.installMode == InstallMode.OPERATION)
        {
            // ドラッグ終了時にマップ外に出ていたら元の位置に戻す
            if (isOutOfMap)
            {
                transform.localPosition = dragBeginPosition;
                isOutOfMap = false;
            }
        }
    }

    /// <summary>
    /// ポインターが上がった瞬間の処理
    /// </summary>
    public void OnPointerUp()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
    }

    /// <summary>
    /// クリックされたときの処理
    /// </summary>
    public void OnPointerClick()
    {
        //if (Input.GetMouseButtonUp(1))
        //{
        //    Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        //    //infoPanel.myRectTransform.position = pos;
        //    testImage.rectTransform.position = pos;
        //}
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        if(Config.installMode == InstallMode.OPERATION)
        {
            // 右クリックだったら，商品情報の登録パネルを見えるようにする
            if (Input.GetMouseButtonUp(1))
            {
                Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                if (infoPanel == null)
                    //Debug.Log("info panel is null");
                infoPanel.myRectTransform.position = pos;
                infoPanel.gameObject.SetActive(true);
                //Debug.Log("show info panel");
            }
        }else if(Config.installMode == InstallMode.DELETE)
        {
            if (Input.GetMouseButtonUp(0))
            {
                // 情報パネルを削除
                if (infoPanel != null)
                    Destroy(infoPanel);

                //シンプルに削除 
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "MapAreaCollider")
        {
            isOutOfMap = false;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "MapAreaCollider")
        {
            isOutOfMap = true;
        }
    }
}
