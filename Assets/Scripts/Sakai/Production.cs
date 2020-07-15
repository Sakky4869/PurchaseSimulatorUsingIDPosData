using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class Production : MonoBehaviour
{
    /// <summary>
    /// ドラッグ中かどうか
    /// </summary>
    private bool isDragging;

    /// <summary>
    /// マップの外に出たかどうか
    /// </summary>
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

    private Vector2 dragBeginPosition;

    private MapManager mapManager;

    //[HideInInspector]
    public List<Link> links;
    
    [HideInInspector]
    public ProductionInfoPanel productionInfoPanel;

    /// <summary>
    /// コスト
    /// </summary>
    //[HideInInspector]
    public float cost;

    /// <summary>
    /// 確定したかどうかのフラグ
    /// </summary>
    //[HideInInspector]
    public bool isConst;

    //[HideInInspector]
    public Production beforeProduction;

    [HideInInspector]
    public Image image;

    [HideInInspector]
    public RectTransform rectTransform;

    /// <summary>
    /// リンクに取り付ける情報
    /// 年:月：日：時：分：秒：ミリ秒を文字列にしたもの
    /// </summary>
    //[HideInInspector]
    public string productionId;

    void Start()
    {
        isDragging = false;
        isOutOfMap = false;
        mapManager = GameObject.Find("MapImage").GetComponent<MapManager>();
        //links = new List<Link>();
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        if(productionName == "" || productionName == null)
        {
            productionName = "";
        }
        if(metaData == "" || metaData == null)
        {
            metaData = ",,,";
        }

        productionInfoPanel = transform.GetChild(0).GetComponent<ProductionInfoPanel>();

        productionInfoPanel.gameObject.SetActive(false);

        //Debug.Log("start method");

    }



    void Update()
    {

    }

    /// <summary>
    /// 商品のデータを情報パネルのInputFieldに設定する
    /// </summary>
    public void SetValueToInfoPanel()
    {
        productionInfoPanel = transform.GetChild(0).GetComponent<ProductionInfoPanel>();
        string[] meta = metaData.Split(',');
        productionInfoPanel.SetValueToInputField(meta[0], meta[1], meta[2], meta[3], productionName);
        
    }



    /// <summary>
    /// 押された瞬間の処理
    /// </summary>
    public void OnPointerDown()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        //Debug.Log("down");

        switch (Config.installMode)
        {
            case InstallMode.DELETE:
                // 接続先の商品のリンク情報を削除
                foreach(Link link in links)
                {
                    Production production = link.GetOpponent(this);
                    production.links.Remove(link);
                    link.DeleteLink();
                }
                Destroy(gameObject);

                break;
            case InstallMode.OPERATION:
                isDragging = true;
                break;
            case InstallMode.LINK:
                // 何も商品が選択されていない場合は自身を選択
                if (mapManager.selectedProduction == null)
                {
                    mapManager.selectedProduction = this;
                    image.color = Color.green;
                }
                // 商品が選択されていた場合はリンクを描画して自身を選択
                else
                {
                    mapManager.selectedProduction.image.color = Color.blue;
                    bool sameFlag = false;
                    // すでに同じ関係のリンクがある場合は自身の選択のみ行う
                    if (links == null)
                        links = new List<Link>();
                    foreach(Link link in links)
                    {
                        if (link.firstProduction == mapManager.selectedProduction || link.secondProduction == mapManager.selectedProduction)
                        {
                            sameFlag = true;
                            break;
                        }
                    }
                    if(sameFlag == false)
                    {
                        //Link link = mapManager.InstantiateLink(mapManager.selectedProduction, this);
                        mapManager.InstantiateLink(mapManager.selectedProduction, this);
                    }
                    mapManager.selectedProduction = this;
                    image.color = Color.green;
                }
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// ドラッグ開始直後の処理
    /// </summary>
    public void OnPointerDragBegin()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        if (Config.installMode == InstallMode.OPERATION)
        {
            dragBeginPosition = transform.position;
        }
    }

    /// <summary>
    /// ドラッグ中の処理
    /// </summary>
    public void OnPointerDrag()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        //Debug.Log("drag");
        if (Config.installMode == InstallMode.OPERATION)
        {
            // マウスの位置と一致させる
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerDragEnd()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        if (Config.installMode == InstallMode.OPERATION)
        {
            if (isOutOfMap)
            {
                transform.position = dragBeginPosition;
                isOutOfMap = false;
            }
        }
    }

    /// <summary>
    /// 上がった瞬間の処理
    /// </summary>
    public void OnPointerUp()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
    }

    public void OnPointerClick()
    {
        if (Config.operationMode != OperationMode.CONFIG)
            return;
        if(Config.installMode == InstallMode.OPERATION)
        {
            if (Input.GetMouseButtonUp(1))
            {
                string[] data = metaData.Split(',');
                productionInfoPanel.SetValueToInputField(data[0], data[1], data[2], data[3], productionName);
                productionInfoPanel.gameObject.SetActive(true);
            }
        }
        //Debug.Log("click");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "MapImage")
        {
            isOutOfMap = true;
        }
    }


}
