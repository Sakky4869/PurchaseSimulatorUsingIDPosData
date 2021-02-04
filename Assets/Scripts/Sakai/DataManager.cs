using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

#region ID-POSデータ関連
/// <summary>
/// かごごとのID－POSデータのルート
/// </summary>
[Serializable]
public class IDPosDataRoot
{
    //public List<IDPosData> datas;
    public IdPosYear[] yearDatas;
    public IDPosDataRoot()
    {
        //datas = new List<IDPosData>();
        yearDatas = new IdPosYear[1];
        yearDatas[0] = new IdPosYear();
    }
}

[Serializable]
public class IdPosYear
{
    public int year;
    public IdPosMonth[] monthDatas;
    public IdPosYear()
    {
        monthDatas = new IdPosMonth[12];
        for(int i = 0; i < monthDatas.Length; i++)
        {
            monthDatas[i] = new IdPosMonth();
        }
    }
}

[Serializable]
public class IdPosMonth
{
    public int month;

    //public List<IdPosDay> dayDatas;
    public IdPosDay[] dayDatas;
    public IdPosMonth()
    {
        dayDatas = new IdPosDay[31];
        for(int i = 0; i < dayDatas.Length; i++)
        {
            dayDatas[i] = new IdPosDay();
        }
    }
}

[Serializable]
public class IdPosDay
{
    public int day;

    //public List<IdPosHour> hourDatas;

    public IdPosHour[] hourDatas;

    public IdPosDay()
    {
        hourDatas = new IdPosHour[24];
        for(int i = 0; i < hourDatas.Length; i++)
        {
            hourDatas[i] = new IdPosHour();
        }
    }
}

[Serializable]
public class IdPosHour
{
    public int hour;

    public List<IDPosData> iDPosDatas;

    public IdPosHour()
    {
        iDPosDatas = new List<IDPosData>();
    }
}

/// <summary>
/// かごの中のデータ
/// </summary>
[Serializable]
public class IDPosData
{
    /// <summary>
    /// 購入商品のデータ
    /// </summary>
    public List<ProductionData> productionDatas;

    /// <summary>
    /// 加工コード
    /// </summary>
    public string kakouCode;

    /// <summary>
    /// 購入時刻の文字列
    /// </summary>
    public string purchaseTime;

    /// <summary>
    /// 入店時刻を文字列にしたもの
    /// </summary>
    public string entranceTime;

    public IDPosData()
    {
        productionDatas = new List<ProductionData>();
        kakouCode = "";
    }
}

/// <summary>
/// 商品のデータ
/// </summary>
[Serializable]
public class ProductionData
{
    /// <summary>
    /// 商品名
    /// </summary>
    public string productionName;

    /// <summary>
    /// 商品のメタデータ
    /// </summary>
    public string productionMetaData;

    public ProductionData(string name, string metaData)
    {
        productionName = name;
        productionMetaData = metaData;
    }

    public ProductionData()
    {

    }
}

[Serializable]
public class TimeDataRoot
{
    public int year;

    public MonthData[] monthDatas;

    public TimeDataRoot()
    {
        monthDatas = new MonthData[12];

        for (int i = 0; i < monthDatas.Length; i++)
        {
            monthDatas[i] = new MonthData();
        }
    }
}

[Serializable]
public class MonthData
{
    public int month;

    public DayData[] dayDatas;
    
    public MonthData(int month)
    {
        this.month = month;
        dayDatas = new DayData[31];
    }
    public MonthData()
    {
        dayDatas = new DayData[31];
        for (int i = 0; i < dayDatas.Length; i++)
        {
            dayDatas[i] = new DayData();
        }
    }
}

/// <summary>
/// 1日の各時刻ごとの入店人数データを保存する
/// </summary>
[Serializable]
public class DayData
{
    public int day;

    public int[] hourData;

    public DayData()
    {
        hourData = new int[24];
    }
}

#endregion

#region システムデータ関連
/// <summary>
/// シミュレーションシステムのデータを保存するクラス
/// </summary>
[Serializable]
public class SystemData
{
    /// <summary>
    /// 操作モード
    /// </summary>
    public int operationMode;

    /// <summary>
    /// 設置モード
    /// </summary>
    public int installMode;

    /// <summary>
    /// 設置した商品のリスト
    /// </summary>
    public List<ProductionObject> productionObjects;

    /// <summary>
    /// 設置した商品につながるリンクのリスト
    /// </summary>
    public List<LinkObject> linkObjects;

    public SystemData()
    {
        productionObjects = new List<ProductionObject>();
        linkObjects = new List<LinkObject>();
    }
}

[Serializable]
public class ProductionObject
{
    /// <summary>
    /// 商品のデータを保存
    /// </summary>
    public ProductionData productionData;

    /// <summary>
    /// 商品の位置を保存
    /// X座標,Y座標を文字列化
    /// 3DのときはX座標，Y座標を文字列化
    /// </summary>
    public string position;

    /// <summary>
    /// 商品オブジェクトのID
    /// </summary>
    public string productionId;

    public ProductionObject()
    {
        productionData = new ProductionData();
    }
}

[Serializable]
public class LinkObject
{
    /// <summary>
    /// 始点の商品のID
    /// </summary>
    public string firstProductionId;

    /// <summary>
    /// 終点の商品のID
    /// </summary>
    public string secondProductionId;
}

#endregion

public class DataManager : MonoBehaviour
{
    /// <summary>
    /// システム内のデータ
    /// </summary>
    public SystemData systemData;

    /// <summary>
    /// ID-POSデータ
    /// </summary>
    public IDPosDataRoot iDPosDataRoot;

    /// <summary>
    /// 入店時刻のデータ
    /// </summary>
    public TimeDataRoot timeDataRoot;

    /// <summary>
    /// ID-POSデータのファイル名
    /// </summary>
    [SerializeField]
    protected string idPosDataFileName;

    /// <summary>
    /// マップに置くオブジェクトのルート
    /// </summary>
    [SerializeField]
    private Transform mapObjectRoot;

    private MapManager mapManager;

    protected SimulationManager simulationManager;

    void Start()
    {
        systemData = new SystemData();
        iDPosDataRoot = new IDPosDataRoot();
        timeDataRoot = new TimeDataRoot();
        mapManager = GameObject.Find("MapImage").GetComponent<MapManager>();
        simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        ReadSystemDatas();
        ReadIDPosData();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// モードデータを保存する
    /// 書き込みはしない
    /// </summary>
    public void SaveModeData()
    {
        systemData.operationMode = (int)Config.operationMode;
        systemData.installMode = (int)Config.installMode;
        //WriteSystemDatas();
    }

    /// <summary>
    /// マップデータを保存する
    /// 書き込みはしない
    /// </summary>
    public void SaveMapData()
    {
        systemData.productionObjects.Clear();
        systemData.linkObjects.Clear();
        foreach(Transform transform in mapObjectRoot)
        {
            // 商品のポイントの場合
            if(transform.GetComponent<Production>() != null)
            {
                Production production = transform.GetComponent<Production>();
                production.productionInfoPanel.SaveProductionData();
                ProductionObject productionObject = new ProductionObject();
                //Debug.Log(production.productionName + ", " + productionObject.productionData);
                productionObject.productionData.productionName = production.productionName;
                productionObject.productionData.productionMetaData = production.metaData;
                productionObject.productionId = production.productionId;
                //Debug.Log(productionObject.productionData.productionName);
                //Debug.Log(productionObject.productionData.productionMetaData);
                RectTransform rectTransform = production.GetComponent<RectTransform>();
                Vector2 pos = rectTransform.anchoredPosition;
                //Debug.Log(pos);
                productionObject.position = pos.x + "," + pos.y;
                systemData.productionObjects.Add(productionObject);
            }
            else if(transform.GetComponent<Link>() != null)
            {
                Link link = transform.GetComponent<Link>();
                LinkObject linkObject = new LinkObject();
                linkObject.firstProductionId = link.firstProduction.productionId;
                linkObject.secondProductionId = link.secondProduction.productionId;
                systemData.linkObjects.Add(linkObject);
            }
        }

        WriteSystemDatas();
    }

    public void ReadIDPosData()
    {
        // ファイル名が拡張子付きのときは拡張子を外したものに変更
        if (idPosDataFileName.Contains("."))
        {
            idPosDataFileName = idPosDataFileName.Split('.')[0];
        }

        // ID-POSデータファイルを読み込み
        using(StreamReader reader = new StreamReader(Application.dataPath + "/PosDatas/" + idPosDataFileName +  ".csv"))
        {
            int lineCount = 0;

            Dictionary<string, int> rowDatas = new Dictionary<string, int>();
            IDPosData iDPosData = new IDPosData();
            iDPosData.kakouCode = "";
            int customerCount = 0;
            int year, month, day, hour;
            while (reader.Peek() > 0)
            {
                
                if(lineCount == 0)
                {
                    // 1行目ではRowデータの取得を行う
                    string[] rowData = reader.ReadLine().Split(',');
                    //Debug.Log(rowData);
                    for(int i = 0; i < rowData.Length; i++)
                    {
                        rowDatas.Add(rowData[i], i);
                        //Debug.Log(rowData[i]);
                    }
                }
                else
                {
                    string[] data = reader.ReadLine().Split(',');
                    //Debug.Log(reader.EndOfStream);
                    string kakouCode = data[rowDatas["加工ｺｰﾄﾞ"]];
                    string metaData = data[rowDatas["部門"]] + "," + data[rowDatas["AU"]] + "," + data[rowDatas["ﾗｲﾝ"]] + "," + data[rowDatas["ｸﾗｽ"]] ;//+ "," + data[rowDatas["商品名"]];
                    string productionName = data[rowDatas["商品名"]];
                    //Debug.Log(kakouCode);
                    // 最初はそのままデータを格納
                    if(iDPosData.kakouCode == "")
                    {
                        iDPosData.kakouCode = kakouCode;
                        iDPosData.productionDatas.Add(new ProductionData(productionName, metaData));
                        string date = data[rowDatas["日"]];
                        string y = date.Substring(0, 4);
                        string m = date.Substring(4, 2);
                        if (m[0] == '0')
                            m = m[1].ToString();
                        string d = date.Substring(6, 2);
                        if (d[0] == '0')
                            d = d[1].ToString();
                        string h = data[rowDatas["時台"]];
                        // シミュレーションの開始時の時刻を設定
                        // 時間単位
                        simulationManager.currentTime = y + ":" + m + ":" + d + ":" + h + ":0";
                        // 分単位
                        //simulationManager.currentTime = y + ":" + m + ":" + d + ":" + h + ":0:0";
                    }
                    // それ以降，同じ人のデータなら商品リストに追加
                    else if(iDPosData.kakouCode == kakouCode)
                    {
                        iDPosData.productionDatas.Add(new ProductionData(productionName, metaData));
                        if (reader.EndOfStream)
                        {

                        }
                    }
                    // 違う人のデータなら新しくPOSのクラスを作成してそれに登録
                    if(iDPosData.kakouCode != kakouCode || reader.EndOfStream == true)
                    {
                        // ここまでに貯めたデータをルートのリストに格納
                        //iDPosDataRoot.datas.Add(iDPosData);
                        iDPosData.purchaseTime = data[rowDatas["日"]] + data[rowDatas["時台"]];

                        // 入店時刻のデータを保存

                        // 日データの取り出し
                        string date = data[rowDatas["日"]];
                        // 年部分の取り出し
                        year = int.Parse(date.Substring(0, 4));
                        // 月部分の取り出し
                        string monthStr = date.Substring(4, 2);
                        //month;
                        // 月が1桁の場合
                        if(monthStr[0] == '0')
                        {
                            month = int.Parse(monthStr[1].ToString());
                        }
                        else
                        {
                            month = int.Parse(monthStr);
                        }

                        //Debug.Log("month : " + month);
                        // 日部分の取り出し
                        string dayStr = date.Substring(6, 2);
                        //day;
                        if(dayStr[0] == '0')
                        {
                            day = int.Parse(dayStr[1].ToString());
                        }
                        else
                        {
                            day = int.Parse(dayStr);
                        }
                        // 時刻データの取り出し
                        hour = int.Parse(data[rowDatas["時台"]]);

                        // コメント化理由：ID-POSデータのルートオブジェクトをメソッドの引数が渡す形式に変更したため
                        // 年月日時のデータを保存してから1人分のID-POSデータを保存
                        iDPosDataRoot.yearDatas[0].year = year;
                        iDPosDataRoot.yearDatas[0].monthDatas[month - 1].month = month;
                        iDPosDataRoot.yearDatas[0].monthDatas[month - 1].dayDatas[day - 1].day = day;
                        iDPosDataRoot.yearDatas[0].monthDatas[month - 1].dayDatas[day - 1].hourDatas[hour - 1].hour = hour;
                        // 最後に出口のデータを入れる
                        iDPosData.productionDatas.Add(new ProductionData("exit", "0,0,0,0"));
                        // ここでID-POSデータを保存
                        iDPosDataRoot.yearDatas[0].monthDatas[month - 1].dayDatas[day - 1].hourDatas[hour - 1].iDPosDatas.Add(iDPosData);

                        //// 年月日時のデータを保存してから1人分のID-POSデータを保存
                        //root.yearDatas[0].year = year;
                        //root.yearDatas[0].monthDatas[month - 1].month = month;
                        //root.yearDatas[0].monthDatas[month - 1].dayDatas[day - 1].day = day;
                        //root.yearDatas[0].monthDatas[month - 1].dayDatas[day - 1].hourDatas[hour - 1].hour = hour;
                        //// 最後に出口のデータを入れる
                        //iDPosData.productionDatas.Add(new ProductionData("exit", "0,0,0,0"));
                        //// ここでID-POSデータを保存
                        //root.yearDatas[0].monthDatas[month - 1].dayDatas[day - 1].hourDatas[hour - 1].iDPosDatas.Add(iDPosData);


                        if (reader.EndOfStream == false)
                        {
                            // 新しくPOSデータのインスタンスを作成
                            iDPosData = new IDPosData();
                            iDPosData.kakouCode = kakouCode;
                            iDPosData.productionDatas.Add(new ProductionData(productionName, metaData));
                        }

                        customerCount++;
                    }
                }
                lineCount++;
            }
            //Debug.Log(lineCount);s
        }


        SetEntranceTime();
    }

    /// <summary>
    /// 入店時刻の割り出し
    /// </summary>
    private void SetEntranceTime()
    {
        foreach(IdPosYear idPosYear in iDPosDataRoot.yearDatas)
        {
            foreach(IdPosMonth idPosMonth in idPosYear.monthDatas)
            {
                foreach(IdPosDay idPosDay in idPosMonth.dayDatas)
                {
                    foreach(IdPosHour idPosHour in idPosDay.hourDatas)
                    {
                        // 入店者の人数を取得
                        int enterCount = idPosHour.iDPosDatas.Count;
                        if (enterCount == 0)
                            continue;
                        //Debug.Log(idPosHour.hour + " : " + enterCount);

                        // 1か月60分
                        // 1日2分ー＞120秒
                        // 1時間5秒ー＞300フレーム
                        // 1分5フレーム

                        // 出現間隔を割り出し　単位は分
                        float baseTime = 60f / enterCount;
                        //Debug.Log(baseTime + " , " + enterCount);

                        for(int i = 0; i < idPosHour.iDPosDatas.Count; i++)
                        {
                            idPosHour.iDPosDatas[i].entranceTime = "" + idPosYear.year + ":" 
                                + idPosMonth.month + ":"
                                + idPosDay.day + ":"
                                + idPosHour.hour + ":"
                                + (int)(baseTime * i);
                            //Debug.Log(idPosHour.iDPosDatas[i].entranceTime);
                        }

                    }
                }
            }
        }
    }

    /// <summary>
    /// シミュレーションシステムのデータをファイルに書き込む
    /// </summary>
    private void WriteSystemDatas()
    {
        string data = JsonUtility.ToJson(systemData);

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/SystemDatas");
        if(directoryInfo.Exists == false)
        {
            directoryInfo.Create();
        }

        string systemDataFileUser = null;

#if UNITY_EDITOR
        systemDataFileUser = GetGitBranchName();
#endif

        FileInfo fileInfo;// = new FileInfo(Application.dataPath + "/SystemDatas/SystemData.json");
        if(systemDataFileUser != null)
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_" + systemDataFileUser + ".json");
        }
        else
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData.json");
        }


        if(fileInfo.Exists == false)
        {
            fileInfo.Create();
        }


        if(systemDataFileUser == null)
        {
            using(StreamWriter writer = new StreamWriter(Application.dataPath + "/SystemDatas/SystemData.json"))
            {
                writer.Write(data);
            }
        }
        else
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + "/SystemDatas/SystemData_" + systemDataFileUser + ".json"))
            {
                writer.Write(data);
            }
        }
    }

    /// <summary>
    /// Unityエディタで操作中のときに，ブランチ名に応じてデータファイルを作るため，ブランチ名を取得する
    /// </summary>
    /// <returns>現在のブランチ名</returns>
    protected string GetGitBranchName()
    {
        using(StreamReader streamReader = new StreamReader(Application.dataPath + "/../.git/HEAD"))
        {
            string[] data = streamReader.ReadToEnd().Split('/');
            string returnData = data[data.Length - 1];
            if (returnData.Contains("_"))
            {
                data = returnData.Split('_');
                returnData = data[0];
            }
            returnData = returnData.Replace("\n", "");
            return returnData;
        }
    }

    /// <summary>
    /// シミュレーションシステムのデータを読み込む
    /// </summary>
    private void ReadSystemDatas()
    {
        string systemDataFileUser = null;
#if UNITY_EDITOR
        // Unityエディタで操作中のときは，現在のブランチの名前に応じてマップデータの保存ファイル名を変える
        systemDataFileUser = GetGitBranchName();
#endif

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/SystemDatas");
        if(directoryInfo.Exists == false)
        {
            return;
        }

        FileInfo fileInfo;//= new FileInfo(Application.dataPath + "/SystemDatas/SystemData.json");

        // エディタではないとき
        if (systemDataFileUser != null)
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData_" + systemDataFileUser + ".json");
        }
        else
        {
            fileInfo = new FileInfo(Application.dataPath + "/SystemDatas/SystemData.json");
        }

        if(fileInfo.Exists == false)
        {
            return;
        }

        using(StreamReader reader = new StreamReader(fileInfo.FullName))
        {
            string data = reader.ReadToEnd();
            systemData = JsonUtility.FromJson<SystemData>(data);
        }

        RestoreSystem();
    }


    private void RestoreSystem()
    {
        if (systemData == null)
        {
            systemData = new SystemData();
            return;
        }
        if (systemData.productionObjects == null)
        {
            return;
        }
        if (systemData.linkObjects == null)
        {
            return;
        }
        foreach(ProductionObject productionObject in systemData.productionObjects)
        {
            mapManager.InstantiateProduction(productionObject);
        }

        foreach(LinkObject linkObject in systemData.linkObjects)
        {
            mapManager.InstantiateLink(linkObject);
        }
    }

    private void OnApplicationQuit()
    {
        //WriteSystemDatas();
    }
}
