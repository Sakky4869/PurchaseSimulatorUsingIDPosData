using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 操作モードのenum
/// enumってなんやねん↓
/// https://ufcpp.net/study/csharp/st_enum.html
/// </summary>
public enum OperationMode
{
    // シミュレーションモード
    SIMULATION = 0,
    
    // 設定モード
    CONFIG
}

/// <summary>
/// 設置モードのenum
/// enumってなんやねん↓
/// https://ufcpp.net/study/csharp/st_enum.html
/// </summary>
public enum InstallMode
{
    // 削除モード
    DELETE = 0,
    
    // 商品設置モード
    POINT,

    // リンクをつなげるモード
    LINK,

    // 商品の位置を調整と商品情報登録モード
    OPERATION
}

/// <summary>
/// 設定関係のUIパーツと機能をまとめたクラス
/// </summary>
public class Config : MonoBehaviour
{
    /// <summary>
    /// SimulationManagerクラスの変数
    /// </summary>
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
    /// スライダーってなんやねん↓
    /// https://squmarigames.com/2018/12/16/unity-beginner-slider/
    /// </summary>
    [SerializeField]
    private Slider installModeSlider;

    /// <summary>
    /// MapManagerクラスの変数
    /// </summary>
    private MapManager mapManager;

    /// <summary>
    /// MapManager3Dクラスの変数
    /// </summary>
    private MapManager3D mapManager3D;

    /// <summary>
    /// DataManagerクラスの変数
    /// </summary>
    private DataManager dataManager;

    /// <summary>
    /// DataManager3Dクラスのインスタンス
    /// </summary>
    private DataManager3D dataManager3D;

    /// <summary>
    /// シミュレーションの開始時刻を指定するかのチェックボックス
    /// Toggleってなんやねん↓
    /// https://tech.pjin.jp/blog/2017/01/29/unity_ugui_toggle/
    /// </summary>
    [SerializeField]
    private Toggle specifiedToggle;

    /// <summary>
    /// シミュレーションで開始時刻を指定するかどうか
    /// </summary>
    public static bool isSpecifiedSimulation;

    /// <summary>
    /// 年を入力するInputField
    /// InputFieldってなんやねん↓
    /// https://www.sejuku.net/blog/83582
    /// </summary>
    [SerializeField]
    private InputField yearInput;

    /// <summary>
    /// 月を入力するInputField
    /// </summary>
    [SerializeField]
    private InputField monthInput;

    /// <summary>
    /// 日を入力するInputField
    /// </summary>
    [SerializeField]
    private InputField dayInput;

    /// <summary>
    /// 時間を入力するInputField
    /// </summary>
    [SerializeField]
    private InputField hourInput;

    /// <summary>
    /// 分を入力するInputField
    /// </summary>
    [SerializeField]
    private InputField minuteInput;


    void Start()
    {
        //----ここから他オブジェクトのコンポーネントの取得----

        // SimulationManagerを取得
        // GameObject.FindGameObjectWithTagでタグを指定してGameObjectを探す
        // 今回の場合はSimulationManagerにGameControllerというタグがついているので，GameControllerを指定
        // GetComponentで，さっき探したGameObjectについているSimulationManagerのコンポーネントをゲット
        simulationManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationManager>();
        
        // MapManagerを探して初期化
        if(GameObject.Find("MapImage"))
            mapManager = GameObject.Find("MapImage").GetComponent<MapManager>();

        // MapManager3Dを探して初期化
        if (GameObject.Find("Stage"))
            mapManager3D = GameObject.Find("Stage").GetComponent<MapManager3D>();

        // DataManagerを探して初期化
        if(GameObject.Find("DataManager"))
            dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        
        // DataManager3Dを探して初期化
        if (GameObject.Find("DataManager3D"))
            dataManager3D = GameObject.Find("DataManager3D").GetComponent<DataManager3D>();

        //----ここまで他オブジェクトのコンポーネントの取得----

        //---- ここからモード選択スライダーの設定 ----

        // これはUnityを見ながらのほうが説明しやすいのでここでは省略
        operationModeSlider.onValueChanged.AddListener(SetOperationMode);
        installModeSlider.onValueChanged.AddListener(SetInstallMode);
		if(specifiedToggle != null)
			specifiedToggle.onValueChanged.AddListener(SetIsSpecifiedMode);
        //---- ここまでモード選択スライダーの設定 ----

        // 操作モードの初期値設定
        operationMode = OperationMode.SIMULATION;

        // 設置モードの初期値設定
        installMode = InstallMode.DELETE;


        isSpecifiedSimulation = false;

    }

    void Update()
    {
        // 設定モードでない場合はreturn
        // returnすると，それ以降のプログラムが実行されなくなる
        if (operationMode != OperationMode.CONFIG)
            return;
    }

    /// <summary>
    /// シミュレーション開始時刻を指定する
    /// 現状では使ってない
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 操作モードのスライダーの値が変更されたときにシステムに反映する
    /// </summary>
    /// <param name="value">スライダーの値　小数</param>
    private void SetOperationMode(float value)
    {
        // 操作モードはenumで，中身はfloat（小数）ではなくint（整数）なので，整数に変換
        // 型を変換することをキャストという
        operationMode = (OperationMode)(int)value;

        // DataManagerがnull（変数に中身が入っていない状態）ではないとき
        // つまり，DataManager3Dがあるとき
        if (dataManager3D != null)
        {
            // 3Dでもモードデータを保存
            dataManager3D.SaveModeData3D();
            return;
        }

        // 操作モードがシミュレーションモードのとき
        if (operationMode == OperationMode.SIMULATION)
        {
            // 2Dモードのときのみ実行
            if (mapManager != null)
            {
                // もし商品オブジェクトが選択されている状態だったら
                if(mapManager.selectedProduction != null)
                {
                    // 色を戻す
                    mapManager.selectedProduction.image.color = Color.blue;
                    mapManager.selectedProduction = null;
                }
            }
        }
        // 操作モードが設定モードのとき
        else
        {
            if(mapManager != null)
            {
                // Linkオブジェクトを取得
                // FindObjectsOfTypeでクラス名を指定して探すことが可能
                Link[] links = GameObject.FindObjectsOfType<Link>();
                // 探したすべてのLinkオブジェクトに対して
                foreach(Link link in links)
                {
                    // Linkオブジェクトが見えるようにする
                    link.GetComponent<Image>().enabled = true;
                }
            }
        }
        if(dataManager != null)
            dataManager.SaveModeData();
    }

    /// <summary>
    /// 設置モードを変える
    /// </summary>
    /// <param name="value">設置モード選択用スライダーの値　小数</param>
    private void SetInstallMode(float value)
    {
        // enumにキャスト
        installMode = (InstallMode)(int)value;

        if(dataManager3D != null)
        {
            // 設置モードを保存
            dataManager3D.SaveModeData3D();
            return;
        }

        // 現在の設置モードに応じて別々の処理をする
        switch (installMode)
        {
            // DELETE or OPERATION or POINTモードのとき
            case InstallMode.DELETE:
            case InstallMode.OPERATION:
            case InstallMode.POINT:
                if(mapManager.selectedProduction != null)
                {
                    mapManager.selectedProduction.image.color = Color.blue;
                    mapManager.selectedProduction = null;
                }
                break;
            // それ以外のとき
            default:
                break;
        }
        if(dataManager != null)
            dataManager.SaveModeData();
        //Debug.Log(installMode);
    }

    /// <summary>
    /// 時刻指定モードの切り替え
    /// </summary>
    /// <param name="flag">時刻指定用Toggleの値</param>
    private void SetIsSpecifiedMode(bool flag)
    {
        isSpecifiedSimulation = flag;
    }

    /// <summary>
    /// 結局使ってないので無視で
    /// </summary>
    /// <param name="focus"></param>
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
