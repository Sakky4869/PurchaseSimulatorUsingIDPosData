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

    void Start()
    {
        heatMapTexture = new Texture2D(1280, 850);
        heatMapTexture = (Texture2D)heatMapImage.mainTexture;
        for(int i = 0; i < heatMapTexture.height; i++)
        {
            for(int j = 0; j < heatMapTexture.width; j++)
            {
                heatMapTexture.SetPixel(j, i, Color.red);
            }
        }

        heatMapImage.texture = heatMapTexture;
    }



    void Update()
    {
        
    }

    public void UpdateHeatMapMatrix(int agentPosX, int agentPosY)
    {

    }
}
