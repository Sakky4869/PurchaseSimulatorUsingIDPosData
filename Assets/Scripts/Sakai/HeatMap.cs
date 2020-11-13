using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// ヒートマップを作成するクラス
/// </summary>
public class HeatMap : MonoBehaviour
{
    /// <summary>
    /// ヒートマップを作るTexture2D
    /// </summary>
    [SerializeField]
    private Texture2D heatMapTexture;

    /// <summary>
    /// ヒートマップの解像度
    /// </summary>
    [SerializeField, Range(1, 100)]
    private int heatMapResolution;

    /// <summary>
    /// ヒートマップを貼り付けるRawImage
    /// RawImageってなんやねん↓
    /// https://tech.pjin.jp/blog/2017/02/06/unity_ugui_rawimage/
    /// このサイト，なんか微妙．．．
    /// </summary>
    [SerializeField]
    private RawImage heatMapImage;
    
    /// <summary>
    /// エージェントの行動を記録するグリッドデータの配列
    /// </summary>
    public int[, ] agentTraceGrid;

    /// <summary>
    /// 行動を記録したグリッドデータの最大値
    /// </summary>
    [HideInInspector]
    public int maxValueOfAgentTraceGrid;

    /// <summary>
    /// 行動を記録したグリッドデータの最大値（float版）
    /// </summary>
    [HideInInspector]
    public float maxValueOfAgentTraceGridFixed;

    /// <summary>
    /// 行動を記録したグリッドデータの最小値
    /// </summary>
    [HideInInspector]
    public int minValueOfAgentTraceGrid;

    /// <summary>
    /// 行動を記録したグリッドデータの最小値（float版）
    /// </summary>
    [HideInInspector]
    public float minValueOfAgentTraceGridFixed;

    /// <summary>
    /// グリッドデータの数値を合わせたもの
    /// </summary>
    [HideInInspector]
    public int totalValueOfAgentTraceGrid;
    
    /// <summary>
    /// ヒートマップグリッドの始点
    /// </summary>
    [SerializeField]
    private Transform gridOriginTransform;
    
    /// <summary>
    /// グリッドセルのオブジェクト
    /// </summary>
    [SerializeField]
    private GridCell gridCell;

    /// <summary>
    /// GridCellクラスのList
    /// Listってなんやねん↓
    /// https://itsakura.com/csharp-list
    /// </summary>
    private List<GridCell> gridCells;

    /// <summary>
    /// グリッドセルの大きさ
    /// </summary>
    private int gridCellSize;

    /// <summary>
    /// ヒートマップが表示されているかどうか
    /// １－＞表示されている
    /// ０－＞表示されていない
    /// </summary>
    private int heatMapIsShown;

    /// <summary>
    /// ヒートマップの透明度　０～１の小数
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    private float heatMapAlpha;

    /// <summary>
    /// 棒グラフが表示されているかどうか
    /// １－＞表示されている
    /// ０－＞表示されていない
    /// </summary>
    private int stickGraphIsShown;

    /// <summary>
    /// メインカメラの最初の位置
    /// </summary>
    private Vector3 firstCameraPosition;

    /// <summary>
    /// メインカメラの最初の角度
    /// </summary>
    private Quaternion firstCameraRotation;

    /// <summary>
    /// 棒グラフを見るときのカメラのTransform
    /// </summary>
    [SerializeField]
    private Transform stickGraphCameraPosition;

    /// <summary>
    /// 棒グラフのスケール
    /// </summary>
    public float stickGraphScale;




    void Start()
    {
        // 変数の初期化
        minValueOfAgentTraceGrid = 0;
        maxValueOfAgentTraceGrid = 0;
        heatMapIsShown = 0;
        gridCellSize = (int)gridCell.transform.localScale.x;
        agentTraceGrid = new int[425 / gridCellSize, 640 / gridCellSize];
        gridCells = new List<GridCell>();

        // 
        InstantiateAgentTraceGrid(agentTraceGrid);


        firstCameraPosition = Camera.main.gameObject.transform.position;
        firstCameraRotation = Camera.main.gameObject.transform.rotation;
        //SetLinkOfCells();
    }



    void Update()
    {
        // Pボタンでヒートマップを可視化
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Unityの再生を止めるときにヒートマップが一瞬映るので，それを防ぐ
            if (Input.GetKey(KeyCode.LeftControl))
                return;
            ShowHeatMap();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowStickGraph();
        }
    }
    
    /// <summary>
    /// グリッドオブジェクトを生成する
    /// </summary>
    /// <param name="matrix">グリッドデータの配列</param>
    private void InstantiateAgentTraceGrid(int[, ] matrix)
    {
		for(int y = 0; y < matrix.GetLength(0); y++)
		{
			for(int x = 0; x < matrix.GetLength(1); x++)
			{
                // グリッドの原点からの距離を計算
				Vector3 pos = gridOriginTransform.position;
				pos.x += x * gridCellSize;
				pos.z -= y * gridCellSize;
                // 計算した位置にグリッドを生成
                // Instantiateってなんやねん↓
                // https://www.sejuku.net/blog/48180
                GameObject cellObject = Instantiate(gridCell.gameObject, pos, Quaternion.identity);
                GridCell cell = cellObject.GetComponent<GridCell>();
                gridCells.Add(cell);
                cellObject.transform.SetParent(gridOriginTransform);
				cell.x = x;
				cell.y = y;
                cell.Init();
			}
		}
	}

    // コメント化理由：ヒートマップのグリッドを用いて経路探索をしようとしたが，NavMeshでやることになったので不要
    //private void SetLinkOfCells()
    //{
    //    RaycastHit hit;
    //    foreach(GridCell gridCell in gridCells)
    //    {
    //        Ray[] rays = new Ray[]
    //        {
    //            new Ray(transform.position, transform.forward),
    //            new Ray(transform.position, - transform.forward),
    //            new Ray(transform.position, transform.right),
    //            new Ray(transform.position, - transform.right)
    //        };
    //        for(int i = 0; i < rays.Length; i++)
    //        {
    //            if(Physics.Raycast(rays[i], out hit, 5f))
    //            {
    //                Debug.Log("hit");
    //                if(hit.collider.tag == "Obstacle")
    //                {
    //                    switch (i)
    //                    {
    //                        case 0:
    //                            gridCell.forwardCell = hit.collider.GetComponent<GridCell>();
    //                            break;
    //                        case 1:
    //                            gridCell.backwardCell = hit.collider.GetComponent<GridCell>();
    //                            break;
    //                        case 2:
    //                            gridCell.rightCell = hit.collider.GetComponent<GridCell>();
    //                            break;
    //                        case 3:
    //                            gridCell.leftCell = hit.collider.GetComponent<GridCell>();
    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// グリッド配列の情報更新
    /// </summary>
    /// <param name="agentPosX">エージェントのX座標</param>
    /// <param name="agentPosY">エージェントのY座標</param>
    public void UpdateHeatMapMatrix(int agentPosX, int agentPosY)
    {
        //totalValueOfAgentTraceGrid++;
        // たどったところのグリッドの値を更新
        agentTraceGrid[agentPosY, agentPosX]++;

        // もし現在のグリッドの最大値よりも大きかったら最小値を更新
        if (maxValueOfAgentTraceGrid < agentTraceGrid[agentPosY, agentPosX])
        {
            maxValueOfAgentTraceGrid = agentTraceGrid[agentPosY, agentPosX];
        }

        // もし現在のグリッドの最小値よりも小さかったら最小値を更新
        if(minValueOfAgentTraceGrid > agentTraceGrid[agentPosY, agentPosX])
        {
            minValueOfAgentTraceGrid = agentTraceGrid[agentPosY, agentPosX];
        }
    }

    /// <summary>
    /// ヒートマップを表示する
    /// </summary>
    public void ShowHeatMap()
    {
        //float average = (float)totalValueOfAgentTraceGrid / gridCells.Count;

        //float minValue = 0, maxValue = 0;

        //for(int i = 0; i < gridCells.Count; i++)
        //{
        //    gridCells[i].traceCountFixed = (float)gridCells[i].traceCount / average;

        //    if(i == 0)
        //    {
        //        minValue = gridCells[i].traceCountFixed;
        //        maxValue = gridCells[i].traceCountFixed;
        //    }
        //    else
        //    {
        //        if(minValue > gridCells[i].traceCountFixed)
        //        {
        //            minValue = gridCells[i].traceCountFixed;
        //        }
        //        if(maxValue < gridCells[i].traceCountFixed)
        //        {
        //            maxValue = gridCells[i].traceCountFixed;
        //        }
        //    }

        //}

        //minValueOfAgentTraceGridFixed = minValue;
        //maxValueOfAgentTraceGridFixed = maxValue;
        
        foreach(GridCell cell in gridCells)
        {
            //cell.SetGridCellImageColor(minValueOfAgentTraceGridFixed, maxValueOfAgentTraceGridFixed, heatMapAlpha);
            // 現在のデータに応じてグリッドセルの色を設定
            cell.SetGridCellImageColor(minValueOfAgentTraceGrid, maxValueOfAgentTraceGrid, heatMapAlpha);
            if (heatMapIsShown == 0)
            {
                //cell.heatMapCellImage.gameObject.SetActive(true);
                cell.heatMapImageParent.SetActive(true);
            }
            else
            {
                //cell.heatMapCellImage.gameObject.SetActive(false);
                cell.heatMapImageParent.SetActive(false);
            }
        }

        heatMapIsShown = 1 - heatMapIsShown;
    }


    private void ShowStickGraph()
    {
        foreach (GridCell cell in gridCells)
        {
            //cell.SetGridCellImageColor(minValueOfAgentTraceGridFixed, maxValueOfAgentTraceGridFixed, heatMapAlpha);
            // 現在のデータに応じてグリッドセルの色を設定
            cell.SetStickScale(minValueOfAgentTraceGrid, maxValueOfAgentTraceGrid, heatMapAlpha);
            //cell.SetGridCellImageColor(minValueOfAgentTraceGrid, maxValueOfAgentTraceGrid, heatMapAlpha);
            if (stickGraphIsShown == 0)
            {
                //cell.heatMapCellImage.gameObject.SetActive(true);
                cell.stick.SetActive(true);
                Camera.main.transform.position = stickGraphCameraPosition.position;
                Vector3 rotation = Camera.main.transform.rotation.eulerAngles;
                rotation.x = 65;
                Camera.main.transform.rotation = Quaternion.Euler(rotation);
                Camera.main.orthographic = false;
                //Debug.Log("main camera fixed");

            }
            else
            {
                //cell.heatMapCellImage.gameObject.SetActive(false);
                cell.stick.SetActive(false);
                Camera.main.transform.position = firstCameraPosition;
                Camera.main.transform.rotation = firstCameraRotation;
                Camera.main.orthographic = true;
                //Debug.Log("main camera reset");
            }
        }

        stickGraphIsShown = 1 - stickGraphIsShown;
    }
}
