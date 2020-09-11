using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GridCell forwardCell;

    //[HideInInspector]
    public GridCell backwardCell;

    //[HideInInspector]
    public GridCell rightCell;

    //[HideInInspector]
    public GridCell leftCell;

    /// <summary>
    /// ヒートマップの生成スクリプト
    /// </summary>
    [SerializeField]
    private HeatMap heatMap;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private Color[] myColor;

    /// <summary>
    /// このグリッドセルを通った回数
    /// </summary>
    [SerializeField]
    private int traceCount;

    //[SerializeField]
    //private MeshRenderer mesh;

    /// <summary>
    /// このグリッドセルのイメージの色
    /// </summary>
    //[SerializeField]
    public Image heatMapCellImage;

    public GameObject heatMapImageParent;

    [HideInInspector]
    public float cost;

    [HideInInspector]
    public bool isVisited;

    
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init()
    {
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
        float ratio = 2 * (traceCount - minV) / (maxV - minV);
        int b = (int)Mathf.Max(0, 255 * (1 - ratio));
        int r = (int)Mathf.Max(0, 255 * (ratio - 1));
        int g = 255 - b - r;

        // Imageにセット
        heatMapCellImage.color = new Color(r, g, b, alpha);
    }

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
        traceCount++;
        heatMap.UpdateHeatMapMatrix(x, y);
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
