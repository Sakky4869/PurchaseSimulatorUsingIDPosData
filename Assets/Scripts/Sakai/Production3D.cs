using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production3D : MonoBehaviour
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

    private Vector3 dragBeginPosition;

    private MapManager mapManager;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
