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

    private MapManager3D mapManager3D;

    private DataManager dataManager;

    private DataManager3D dataManager3D;

    /// <summary>
    /// シミュレーションの開始時刻を指定するかのチェックボックス
    /// </summary>
    [SerializeField]
    private Toggle specifiedToggle;

    /// <summary>
    /// シミュレーションで開始時刻を指定するかどうか
    /// </summary>
    public static bool isSpecifiedSimulation;

    [SerializeField]
    private InputField yearInput;

    [SerializeField]
    private InputField monthInput;

    [SerializeField]
    private InputField dayInput;

    [SerializeField]
    private InputField hourInput;

    [SerializeField]
    private InputField minuteInput;


    void Start()
    {
        //----ここから他オブジェクトのコンポーネントの取得----

        simulationManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationManager>();
        // MapManagerを探して初期化
        if(GameObject.Find("MapImage"))
            mapManager = GameObject.Find("MapImage").GetComponent<MapManager>();
        if (GameObject.Find("Stage"))
            mapManager3D = GameObject.Find("Stage").GetComponent<MapManager3D>();

        // DataManagerを探して初期化
        if(GameObject.Find("DataManager"))
            dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        if (GameObject.Find("DataManager3D"))
            dataManager3D = GameObject.Find("DataManager3D").GetComponent<DataManager3D>();
        //----ここまで他オブジェクトのコンポーネントの取得----

        //---- ここからモード選択スライダーの設定 ----
        operationModeSlider.onValueChanged.AddListener(SetOperationMode);
        installModeSlider.onValueChanged.AddListener(SetInstallMode);
		if(specifiedToggle != null)
			specifiedToggle.onValueChanged.AddListener(SetIsSpecifiedMode);
        //---- ここまでモード選択スライダーの設定 ----

        operationMode = OperationMode.SIMULATION;
        installMode = InstallMode.DELETE;
        isSpecifiedSimulation = false;

    }

    void Update()
    {
        // 設定モードでない場合はreturn
        if (operationMode != OperationMode.CONFIG)
            return;
    }

    public string GetSimulationStartTime()
    {
        // どれか一つでも情報が抜けていたらnullを返す
        if (yearInput.text == "" || yearInput.text == null)
            return null;
        if (monthInput.text == "" || monthInput.text == null)
            return null;
        if (dayInput.text == "" || dayInput.text == null)
            return null;
        if (hourInput.text == "" || hourInput.text == null)
            return null;
        if (minuteInput.text == "" || minuteInput.text == null)
            return null;

        return yearInput.text + ":" + monthInput.text + ":" + dayInput.text + ":" + hourInput.text + ":" + minuteInput.text;
    }


    private void SetOperationMode(float value)
    {
        operationMode = (OperationMode)(int)value;
        if (dataManager3D != null)
        {
            dataManager3D.SaveModeData3D();
            return;
        }
        if (operationMode == OperationMode.SIMULATION)
        {
            // 2Dモードのときのみ実行
            if (mapManager != null)
            {
                if(mapManager.selectedProduction != null)
                {
                    mapManager.selectedProduction.image.color = Color.blue;
                    mapManager.selectedProduction = null;
                }
            }
        }
        else
        {
            if(mapManager != null)
            {
                Link[] links = GameObject.FindObjectsOfType<Link>();
                foreach(Link link in links)
                {
                    link.GetComponent<Image>().enabled = true;
                }
            }
        }
        if(dataManager != null)
            dataManager.SaveModeData();
    }

    private void SetInstallMode(float value)
    {
        installMode = (InstallMode)(int)value;
        if(dataManager3D != null)
        {
            dataManager3D.SaveModeData3D();
            return;
        }
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
        if(dataManager != null)
            dataManager.SaveModeData();
        //Debug.Log(installMode);
    }

    private void SetIsSpecifiedMode(bool flag)
    {
        isSpecifiedSimulation = flag;
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
