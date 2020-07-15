using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OperationMode
{
    SIMULATION = 0,
    CONFIG
}

public enum InstallMode
{
    DELETE = 0,
    POINT,
    LINK,
    OPERATION
}

/// <summary>
/// 設定関係のUIパーツと機能をまとめたクラス
/// </summary>
public class Config : MonoBehaviour
{
    private SimulationManager simulationManager;

    /// <summary>
    /// 操作モードのenumの変数
    /// </summary>
    public static OperationMode operationMode;

    /// <summary>
    /// 操作モード選択用のスライダー
    /// </summary>
    [SerializeField]
    private Slider operationModeSlider;

    /// <summary>
    /// 設置モードのenumの変数
    /// </summary>
    public static InstallMode installMode;

    /// <summary>
    /// 設置モード選択用のスライダー
    /// </summary>
    [SerializeField]
    private Slider installModeSlider;

    private MapManager mapManager;

    private DataManager dataManager;


    void Start()
    {
        //----ここから他オブジェクトのコンポーネントの取得----

        simulationManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationManager>();
        mapManager = GameObject.Find("MapImage").GetComponent<MapManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        //----ここまで他オブジェクトのコンポーネントの取得----

        //---- ここからモード選択スライダーの設定 ----
        operationModeSlider.onValueChanged.AddListener(SetOperationMode);
        installModeSlider.onValueChanged.AddListener(SetInstallMode);
        //---- ここまでモード選択スライダーの設定 ----

        operationMode = OperationMode.SIMULATION;
        installMode = InstallMode.DELETE;
    }

    void Update()
    {
        // 設定モードでない場合はreturn
        if (operationMode != OperationMode.CONFIG)
            return;
    }

    private void SetOperationMode(float value)
    {
        operationMode = (OperationMode)(int)value;
        if(operationMode == OperationMode.SIMULATION)
        {
            if(mapManager.selectedProduction != null)
            {
                mapManager.selectedProduction.image.color = Color.blue;
                mapManager.selectedProduction = null;
            }
        }
        dataManager.SaveModeData();
    }

    private void SetInstallMode(float value)
    {
        installMode = (InstallMode)(int)value;
        switch (installMode)
        {
            case InstallMode.DELETE:
            case InstallMode.OPERATION:
            case InstallMode.POINT:
                if(mapManager.selectedProduction != null)
                {
                    mapManager.selectedProduction.image.color = Color.blue;
                    mapManager.selectedProduction = null;
                }
                break;
            default:
                break;
        }
        dataManager.SaveModeData();
        //Debug.Log(installMode);
    }


    private void OnApplicationFocus(bool focus)
    {
        // フォーカスされたとき
        if(focus)
        {

        }
        // フォーカスが切れたとき
        else
        {

        }
    }

}
