using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// ヒートマップの生成スクリプト
    /// </summary>
    private HeatMap heatMap;

    [SerializeField]
    private Color[] myColor;

    private int traceCount;

    [SerializeField]
    private MeshRenderer mesh;

    
    

    void Start()
    {
        heatMap = GameObject.Find("HeatMapCreator").GetComponent<HeatMap>();
        //myColor = new Color[] { Color.red, Color.blue, Color.black };
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // エージェントのときに記録
        if (other.tag != "Agent")
            return;
        traceCount++;
        if (mesh == null)
            Debug.Log("mesh renderer is null");

        if (mesh.material == null)
            Debug.Log("material is null");
        mesh.material.color = myColor[traceCount % 3];
    }
}
