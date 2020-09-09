using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    [SerializeField]
    private Transform gridOriginTransform;
    
    [SerializeField]
    private GridCell gridCell;

    private int gridCellSize;

    void Start()
    {
        gridCellSize = (int)gridCell.transform.localScale.x;
        agentTraceGrid = new int[1280 / gridCellSize, 850 / gridCellSize];
        InstantiateAgentTraceGrid(agentTraceGrid);
    }



    void Update()
    {
        
    }
    
    /// <summary>
    /// グリッドオブジェクトを生成する
    /// </summary>
    /// <param name="matrix">グリッドデータの配列</param>
    private void InstantiateAgentTraceGrid(int[, ] matrix)
    {
		for(int y = 0; y < matrix.GetLength(1); y++)
		{
			for(int x = 0; x < matrix.GetLength(0); x++)
			{
				Vector3 pos = gridOriginTransform.position;
				pos.x += x * gridCellSize;
				pos.z -= y * gridCellSize;
				GameObject cellObject = Instantiate(gridCell.gameObject, pos, Quaternion.identity);
                GridCell cell = cellObject.GetComponent<GridCell>();
				cell.x = x;
				cell.y = y;
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
		agentTraceGrid[agentPosY, agentPosX] ++;
    }
}
