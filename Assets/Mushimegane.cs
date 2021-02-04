using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mushimegane : MonoBehaviour
{

    [SerializeField]
    private Image imagePrefab;


    [SerializeField]
    private ScrollRect scrollRect;
    /// <summary>
    /// ドラッグ中かどうか
    /// </summary>
    private bool isDragging;

    /// <summary>
    /// マップの外に出たかどうか
    /// </summary>
    private bool isOutOfMap;

    /// <summary>
    /// ドラッグ開始位置
    /// </summary>
    private Vector2 dragBeginPosition;

    /// <summary>
    /// 商品名
    /// </summary>
    //[HideInInspector]
    //public string productionName;


    public List<Production3D> searchProductionList;

    public RectTransform productionListPanelRect;

    /// <summary>
    /// 商品の名前を入力するスクロールビュー
    /// </summary>
    //[HideInInspector]
    public ScrollContentsTest scrollview;

    private string name;

    // Start is called before the first frame update
    void Start()
    {
        // 変数の初期化
        isDragging = false;
        isOutOfMap = false;
        searchProductionList = new List<Production3D>();

        //productionListPanelRect.transform.position

    }

    void Update()
    {

    }

    /// <summary>
    /// 押された瞬間の処理
    /// </summary>
    public void OnPointerDown() {

        ////Debug.Log("pointer down");

        // 操作モードがシミュレーションモードのときのみ操作
        if (Config.operationMode != OperationMode.SIMULATION)
            return;

        ////Debug.Log("pointer down");

        isDragging = true;
    }

    /// <summary>
    /// ドラッグ開始時の処理
    /// </summary>
    public void OnPointerDragBegin()
    {
        // 操作モードがシミュレーションモードのときのみ操作
        if (Config.operationMode != OperationMode.SIMULATION)
            return;

        //Debug.Log("pointer drag begin");
        // ドラッグ開始時の位置を記録：マップ外に出たときに戻ってこれるようにするため
        dragBeginPosition = transform.localPosition;

    }

    /// <summary>
    /// ドラッグ中の処理
    /// </summary>
    public void OnPointerDrag()
    {

        //Config.operationMode = OperationMode.CONFIG;
        //Config.installMode = InstallMode.OPERATION;
        // 操作モードがシミュレーションモードのときのみ操作
        if (Config.operationMode != OperationMode.SIMULATION)
            return;


        //Debug.Log("pointer dragging");
        // マウスの座標を代入
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
           
        // Y座標を固定
        Vector3 localPos = transform.localPosition;
        //localPos.y = 0;
        localPos.y = 6;
        transform.localPosition = localPos;

        //scrollview.myRectTransform.transform.position = Camera.main.WorldToScreenPoint(transform.position);

    }



    /// <summary>
    /// ドラッグ終了時の処理
    /// </summary>
    public void OnPointerDragEnd()
    {
        //isDragging = false;
        if (Config.operationMode != OperationMode.SIMULATION)
            return;


        //Debug.Log("pointer drag end");                
        // ドラッグ終了時にマップ外に出ていたら元の位置に戻す
        if (isOutOfMap)
            {
                transform.localPosition = dragBeginPosition;
                isOutOfMap = false;
            }

    }

    /// <summary>
    /// ポインターが上がった瞬間の処理
    /// </summary>
    public void OnPointerUp()
    {
        if (Config.operationMode != OperationMode.SIMULATION)
            return;


        //Debug.Log("pointer up");

    }

    //ポインターがクリックされた時の処理
    public void OnPointerClick()
    {
        // 右クリックだったら，商品情報の登録パネルを見えるようにする
        if (Input.GetMouseButtonUp(1))
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            /*            if (scrollview == null)
                            Debug.Log("info panel is null");*/
            scrollview.myRectTransform.position = pos;
            scrollview.gameObject.SetActive(true);
            //Debug.Log("show info panel");
        }
    }

    private void AddProductionInfoImageToScrolContent(string name)
    {
        // ScrolViewのContentのRectTransformを取得
        RectTransform rectTransform = scrollRect.content.GetComponent<RectTransform>();
        // roomnodeを新しく生成する
        Image img = Instantiate(imagePrefab, transform.position, Quaternion.identity);
        // roomnodeの親オブジェクトをScrolViewのContentに設定
        img.GetComponent<RectTransform>().SetParent(rectTransform);
        // roomnodeのscaleを調整
        img.GetComponent<RectTransform>().localScale = Vector3.one;

        Text text = img.transform // roomnode
            .GetChild(0) // Button
            .GetChild(0).GetComponent<Text>(); // Text
        text.text = name;


 /*       for (int i = 0; i < rectTransform.ChildCount; i++)
        {
            RoomNode roomNode = rectTransform.GetChild(i).GetComponent<RoomNode>();
            string data = roomNode.recognitionData;
            if (data == exitProductionData)
            {
                roomNode.SetParent(null);
                Destroy(roomNode.gameObject);
            }
        }*/

    }

    //侵入した（当たった）瞬間
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Production")//ProductionPrefab3DにはProductionというタグが付いていて，このタグ以外のタグは反応しないよーっていうやつ
            return;


        Production3D production3D = other.GetComponent<Production3D>();//production3Dという変数はProduction3Dというところからとってきてますよー
        string productionName = production3D.productionName;
        
        //name = productionName;
        AddProductionInfoImageToScrolContent(productionName);
        //Debug.Log(production3D.productionName);//衝突した商品名取得
        //searchProductionList.Add(production3D);//searchProductionListに衝突したproduction3Dを追加
        //Debug.Log("enter : " + searchProductionList.Count);

    }

    //侵入している（当たっている）間
    private void OnTriggerStay(Collider other)
    {
        
    }

    //脱出した（当たっていない）瞬間
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Production")
            return;


        Production3D production3D = other.GetComponent<Production3D>();//production3Dという変数はProduction3Dというｸﾗｽ（？）からとってきてますよー
        searchProductionList.Remove(production3D);//searchProductionListから衝突して抽出したproduction3Dを消去
        Debug.Log("exit : " + searchProductionList.Count);
    }




}
