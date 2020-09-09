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
    /// エージェントの行動を記録する配列
    /// </summary>
    public int[, ] agentTraceGrid;
    
    [SerializeField]
    private Transform gridOriginTransform;
    
    [SerializeField]
    private GridCell gridCell;

    void Start()
    {
        agentTraceGrid = new int[1280, 850];
        InstantiateAgentTraceGrid(agentTraceGrid);
    }



    void Update()
    {
        
    }
    
    
    private void InstantiateAgentTranceGrid(int[, ] matrix)
    {
		for(int y = 0; y < matrix.GetLength(1); y++)
		{
			for(int x = 0; x < matrix.GetLength(0); x++)
			{
				Vector3 pos = gridOrigin.position;
				pos.x = x;
				pos.y = y;
				GridCell cell = Instantiate(gridCell.gameObject, pos, Quaternion.identity) as GridCell;
				cell.x = x;
				cell.y = y;
			}
		}
	}

    public void UpdateHeatMapMatrix(int agentPosX, int agentPosY)
    {
		agentTraceGrid[agentPosY, agentPosX] ++;
    }
}
