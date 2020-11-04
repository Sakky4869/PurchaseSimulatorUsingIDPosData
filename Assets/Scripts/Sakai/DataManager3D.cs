using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/**
 * ＜メモ＞
 * 3D用のシステムデータクラスを記述
 * リンクの保存がないところ以外は2Dと同じ
 */

#region システムデータ関連

/// <summary>
/// シミュレーションシステムのデータを保存するクラス
/// 3Dバージョン
/// </summary>
[Serializable]
public class SystemData3D
{
    /// <summary>
    /// 操作モード
    /// </summary>
    public int operationMode;

    /// <summary>
    /// 設置モード
    /// </summary>
    public int installMode;

    public List<ProductionObject> productionObjects3D;

    public SystemData3D()
    {
        productionObjects3D = new List<ProductionObject>();
    }

}



#endregion


public class DataManager3D : DataManager
{
    /// <summary>
    /// システム内のデータ
    /// </summary>
    public SystemData3D systemData3D;

    // コメント化理由：継承元のクラスにある
    /// <summary>
    /// ID-POSデータ
    /// </summary>
    //public IDPosDataRoot iDPosDataRoot3D;

    // コメント化理由：継承元のクラスにある
    /// <summary>
    /// 入店時刻のデータ
    /// </summary>
    //public TimeDataRoot timeDataRoot;

    // コメント化理由：継承元のクラスにある
    /// <summary>
    /// ID-POSデータのファイル
    /// </summary>
    //[SerializeField]
    //private string idPosDataFileName;

    [SerializeField]
    private Transform mapObjectRoot3D;

    private MapManager3D mapManager3D;

    //private SimulationManager simulationManager;

    

    void Start()
    {
        systemData3D = new SystemData3D();
        iDPosDataRoot = new IDPosDataRoot();
        timeDataRoot = new TimeDataRoot();
        mapManager3D = GameObject.Find("Stage").GetComponent<MapManager3D>();
        simulationManager = GameObject.Find("SimulationManager3D").GetComponent<SimulationManager>();
        ReadSystemDatas3D();
        ReadIDPosData();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// モードデータの保存
    /// ファイルへの書き込みはしない
    /// </summary>
    public void SaveModeData3D()
    {
        systemData3D.operationMode = (int)Config.operationMode;
        systemData3D.installMode = (int)Config.installMode;
    }

    /// <summary>
    /// マップデータを保存する
    /// ファイルへの書き込みはしない
    /// </summary>
    public void SaveMapData3D()
    {
        systemData3D.productionObjects3D.Clear();
        foreach(Transform transform in mapObjectRoot3D)
        {
            // 商品オブジェクトのとき
            if(transform.GetComponent<Production3D>() != null)
            {
                Production3D production3D = transform.GetComponent<Production3D>();
                ProductionObject productionObject = new ProductionObject();
                productionObject.productionData.productionName = production3D.productionName;
                productionObject.productionData.productionMetaData = production3D.metaData;
                productionObject.productionId = production3D.productionId;
                Vector3 localPosition = production3D.transform.localPosition;
                productionObject.position = localPosition.x + "," + localPosition.z;
                systemData3D.productionObjects3D.Add(productionObject);
            }
        }

        WriteSystemDatas3D();
    }

    private void WriteSystemDatas3D()
    {
        string data = JsonUtility.ToJson(systemData3D);

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/SystemDatas");
        if (directoryInfo.Exists == false)
        {
            directoryInfo.Create();
        }

        string systemDataFileUser = null;

#if UNITY_EDITOR
        systemDataFileUser = GetGitBranchName();
#endif

        FileInfo fileInfo;// = new FileInfo(Application.dataPath + "/SystemDatas/SystemData.json");
        if (systemDataFileUser != null)
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_" + systemDataFileUser + "_3D.json");
        }
        else
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_3D.json");
        }


        if (fileInfo.Exists == false)
        {
            fileInfo.Create();
        }


        if (systemDataFileUser == null)
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + "/SystemDatas/SystemData_3D.json"))
            {
                writer.Write(data);
            }
        }
        else
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + "/SystemDatas/SystemData_" + systemDataFileUser + "_3D.json"))
            {
                writer.Write(data);
            }
        }
    }


    private void ReadSystemDatas3D()
    {
        string systemDataFileUser = null;
#if UNITY_EDITOR
        systemDataFileUser = GetGitBranchName();
#endif

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/SystemDatas");
        if (directoryInfo.Exists == false)
            return;

        FileInfo fileInfo;

        // エディタではないとき
        if (systemDataFileUser != null)
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_" + systemDataFileUser + "_3D.json");
            //fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_" + "Nakamura" + "_3D.json");
            //Debug.Log("got branch name");
        }
        else
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_3D.json");
        }

        if (fileInfo.Exists == false)
        {
            //Debug.Log("file not exists");
            return;
        }

        using(StreamReader reader = new StreamReader(fileInfo.FullName))
        {
            string data = reader.ReadToEnd();
            Debug.Log(data);
            systemData3D = JsonUtility.FromJson<SystemData3D>(data);
        }
        RestoreSystem();
    }

    private void RestoreSystem()
    {
        if(systemData3D == null)
        {
            systemData3D = new SystemData3D();
            return;
        }

        if(systemData3D.productionObjects3D == null)
        {
            return;
        }

        //Debug.Log("production count : " + systemData3D.productionObjects3D.Count);

        foreach(ProductionObject productionObject in systemData3D.productionObjects3D)
        {
            mapManager3D.InstantiateProduction3D(productionObject);
        }

        //Debug.Log("restore system data");
        
    }

    
}
