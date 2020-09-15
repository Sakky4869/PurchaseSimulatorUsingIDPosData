using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HeatMap : MonoBehaviour
{
    /// <summary>
    /// ヒートマップを作るテクスチャー
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
    /// </summary>
    [SerializeField]
    private RawImage heatMapImage;
    
    /// <summary>
    /// エージェントの行動を記録するグリッドデータの配列
    /// </summary>
    public int[, ] agentTraceGrid;

    [HideInInspector]
    public int maxValueOfAgentTraceGrid;

    [HideInInspector]
    public int minValueOfAgentTraceGrid;
    
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



    void Start()
    {
        minValueOfAgentTraceGrid = 0;
        maxValueOfAgentTraceGrid = 0;
        heatMapIsShown = 0;
        gridCellSize = (int)gridCell.transform.localScale.x;
        agentTraceGrid = new int[425 / gridCellSize, 640 / gridCellSize];
        gridCells = new List<GridCell>();
        InstantiateAgentTraceGrid(agentTraceGrid);
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
				Vector3 pos = gridOriginTransform.position;
				pos.x += x * gridCellSize;
				pos.z -= y * gridCellSize;
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
        // たどったところのグリッドの値を更新
		agentTraceGrid[agentPosY, agentPosX] ++;

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

    public void ShowHeatMap()
    {
        foreach(GridCell cell in gridCells)
        {
            cell.SetGridCellImageColor(minValueOfAgentTraceGrid, maxValueOfAgentTraceGrid, heatMapAlpha);
            if(heatMapIsShown == 0)
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
}
