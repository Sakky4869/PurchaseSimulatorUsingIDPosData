using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ヒートマップのセルのクラス
/// </summary>
public class GridCell : MonoBehaviour
{
    /// <summary>
    /// グリッド上のX座標
    /// </summary>
    [HideInInspector]
    public int x;

    /// <summary>
    /// グリッド上のY座標
    /// </summary>
    [HideInInspector]
    public int y;

    //[HideInInspector]
    //public GridCell forwardCell;

    //[HideInInspector]
    //public GridCell backwardCell;

    //[HideInInspector]
    //public GridCell rightCell;

    //[HideInInspector]
    //public GridCell leftCell;

    /// <summary>
    /// ヒートマップの生成スクリプト
    /// </summary>
    [SerializeField]
    private HeatMap heatMap;

    /// <summary>
    /// ヒートマップに映るときの色の配列
    /// </summary>
    [SerializeField]
    private Color[] myColor;

    /// <summary>
    /// このグリッドセルを通った回数
    /// </summary>
    //[SerializeField]
    public int traceCount;

    /// <summary>
    /// 
    /// </summary>
    //public float traceCountFixed;

    //[SerializeField]
    //private MeshRenderer mesh;

    /// <summary>
    /// このグリッドセルのイメージs
    /// </summary>
    //[SerializeField]
    public Image heatMapCellImage;

    /// <summary>
    /// ヒートマップの親オブジェクトにするGameObject
    /// </summary>
    public GameObject heatMapImageParent;

    /// <summary>
    /// コスト
    /// </summary>
    [HideInInspector]
    public float cost;

    /// <summary>
    /// このセルが通過されたことがあるか
    /// </summary>
    [HideInInspector]
    public bool isVisited;

    /// <summary>
    /// 棒グラフ表示に用いる棒オブジェクト
    /// </summary>
    //[SerializeField]
    public GameObject stick;

    
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 初期化メソッド
    /// </summary>
    public void Init()
    {
        // ヒートマップを取得
        heatMap = GameObject.Find("HeatMapCreator").GetComponent<HeatMap>();
        GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// グリッドのデータの最大値と最小値とこのセルの値からヒートマップでの色を計算
    /// </summary>
    /// <param name="minValue">グリッドのデータの最小値</param>
    /// <param name="maxValue">グリッドのデータの最大値</param>
    public void SetGridCellImageColor(int minValue, int maxValue, float alpha)
    {
        // グリッドのデータの最大値と最小値とこのセルの値からヒートマップでの色を計算
        float minV = minValue;
        float maxV = maxValue;
        //float ratio = 2f * (traceCountFixed - minV) / (maxV - minV);
        float ratio = 2 * (traceCount - minV) / (maxV - minV);
        int b = (int)Mathf.Max(0, 255 * (1 - ratio));
        int r = (int)Mathf.Max(0, 255 * (ratio - 1));
        int g = 255 - b - r;

        // Imageにセット
        heatMapCellImage.color = new Color(r, g, b, alpha);
    }

    /// <summary>
    /// 棒グラフを表示する
    /// </summary>
    /// <param name="minValue">グリッドデータの最小値</param>
    /// <param name="maxValue">グリッドデータの最大値</param>
    /// <param name="alpha">棒グラフの透明度</param>
    public void SetStickScale(int minValue, int maxValue, float alpha)
    {
        Vector3 stickScale = new Vector3(1, traceCount / heatMap.stickGraphScale, 1);
        stick.transform.localScale = stickScale;
        //Vector3 position = new Vector3(0, traceCount / 2 - 0.5f, 0);
        Vector3 position = new Vector3(0, stickScale.y / 2 - 0.5f, 0);
        stick.transform.localPosition = position;
        //stick.SetActive(true);
    }


    /// <summary>
    /// Unityの機能で，当たり判定を取るためのメソッド
    /// Colliderを使う
    /// Colliderってなんやねん↓
    /// https://www.sejuku.net/blog/59171
    /// </summary>
    /// <param name="other">当たったオブジェクトのCollider</param>
    private void OnTriggerEnter(Collider other)
    {
        // 障害物のときは見えなくする
        //if (other.tag == "Obstacle")
        //{
        //    GetComponent<MeshRenderer>().enabled = false;
        //    gameObject.SetActive(false);

        //}

        // エージェントのときに記録
        if (other.tag != "Agent")
            return;
        Customer3D customer3D = other.GetComponent<Customer3D>();
        if(customer3D.isGetFirstProduction == true && customer3D.isGettingExit == false)
        {
            traceCount++;
            // ヒートマップの配列を更新
            heatMap.UpdateHeatMapMatrix(x, y);
        }
        //SetGridCellImageColor(heatMap.minValueOfAgentTraceGrid, heatMap.maxValueOfAgentTraceGrid);
        
        // コメント化理由：Mesh Rendererを使わない方法でやることにしたため
        //if (mesh == null)
        //    Debug.Log("mesh renderer is null");

        //if (mesh.material == null)
        //    Debug.Log("material is null");
        //mesh.material.color = myColor[traceCount % 3];
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    // 障害物のときは見えるようにする
    //    if (other.tag == "Obstacle")
    //        GetComponent<MeshRenderer>().enabled = true;
    //}
}
