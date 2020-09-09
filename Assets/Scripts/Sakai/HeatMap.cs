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

    private int num;

    void Start()
    {
        minValueOfAgentTraceGrid = 0;
        maxValueOfAgentTraceGrid = 0;
        num = 0;
        gridCellSize = (int)gridCell.transform.localScale.x;
        agentTraceGrid = new int[850 / gridCellSize, 1280 / gridCellSize];
        gridCells = new List<GridCell>();
        InstantiateAgentTraceGrid(agentTraceGrid);
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
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
				cell.x = x;
				cell.y = y;
                cell.Init();
			}
		}
	}

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
            cell.SetGridCellImageColor(minValueOfAgentTraceGrid, maxValueOfAgentTraceGrid);
            if(num == 0)
            {
                cell.heatMapCellImage.gameObject.SetActive(true);
            }
            else
            {
                cell.heatMapCellImage.gameObject.SetActive(false);
            }
        }

        num = 1 - num;
    }
}
