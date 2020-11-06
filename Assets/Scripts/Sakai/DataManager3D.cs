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

#region システムデータ関連　JSONという形式で保存するためのクラス

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
        // 商品などのシステムデータを読み込む
        ReadSystemDatas3D();
        // ID-POSデータを読み込む
        ReadIDPosData();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// モードデータの保存
    /// </summary>
    public void SaveModeData3D()
    {
        systemData3D.operationMode = (int)Config.operationMode;
        systemData3D.installMode = (int)Config.installMode;
    }

    /// <summary>
    /// マップデータを保存する
    /// </summary>
    public void SaveMapData3D()
    {
        systemData3D.productionObjects3D.Clear();
        // すべての商品オブジェクトについて
        foreach(Transform transform in mapObjectRoot3D)
        {
            // 商品オブジェクトのとき
            if(transform.GetComponent<Production3D>() != null)
            {
                // Production3Dクラスをゲット
                Production3D production3D = transform.GetComponent<Production3D>();
                production3D.infoPanel.SaveProductionData();
                // 保存用クラスの変数を宣言
                ProductionObject productionObject = new ProductionObject();
                
                // 中身を移す
                productionObject.productionData.productionName = production3D.productionName;
                productionObject.productionData.productionMetaData = production3D.metaData;
                productionObject.productionId = production3D.productionId;
                Vector3 localPosition = production3D.transform.localPosition;
                productionObject.position = localPosition.x + "," + localPosition.z;
                systemData3D.productionObjects3D.Add(productionObject);
            }
        }

        // システムデータをファイルに書き込む
        WriteSystemDatas3D();
    }

    /// <summary>
    /// システムデータをファイルに書き込む
    /// </summary>
    private void WriteSystemDatas3D()
    {
        // JSON形式で保存するために用意したクラスの変数を，JSON文字列に変換
        string data = JsonUtility.ToJson(systemData3D);

        // データを保存するファイルを入れるディレクトリがあるかを確認
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/SystemDatas");
        // ディレクトリがなければ作る
        if (directoryInfo.Exists == false)
        {
            directoryInfo.Create();
        }

        string systemDataFileUser = null;

#if UNITY_EDITOR // Unityエディタだった場合
        // Gitのブランチデータから編集者の名前を取得
        systemDataFileUser = GetGitBranchName();
#endif

        
        FileInfo fileInfo;// = new FileInfo(Application.dataPath + "/SystemDatas/SystemData.json");
        
        // 編集者の名前が取得できていれば
        if (systemDataFileUser != null)
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_" + systemDataFileUser + "_3D.json");
        }
        else
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_3D.json");
        }

        // ファイルが存在していなければ作る
        if (fileInfo.Exists == false)
        {
            fileInfo.Create();
        }


        if (systemDataFileUser == null)
        {
            // StreamWriterでファイルに書き込むための道を作る
            // 鉛筆を持つみたいなイメージ
            using (StreamWriter writer = new StreamWriter(Application.dataPath + "/SystemDatas/SystemData_3D.json"))
            {
                // 書き込む
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


    /// <summary>
    /// システムデータを読み込む
    /// </summary>
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

        // エディタのとき
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

        // StreamReaderでファイルを読み込む道を作る
        using(StreamReader reader = new StreamReader(fileInfo.FullName))
        {
            string data = reader.ReadToEnd();
            //Debug.Log(data);
            systemData3D = JsonUtility.FromJson<SystemData3D>(data);
        }
        // データをUnityの空間に反映
        RestoreSystem();
    }

    /// <summary>
    /// システムデータをUnityの空間に反映
    /// </summary>
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
