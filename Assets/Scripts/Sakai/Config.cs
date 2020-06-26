using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Config : MonoBehaviour
{
    /// <summary>
    /// シミュレーション速度調整用のスライダー
    /// </summary>
    [SerializeField, Header("シミュレーション速度調整用スライダー")]
    private Slider simulationSpeedSlider;

    /// <summary>
    /// シミュレーション速度の最小値
    /// </summary>
    [SerializeField, Header("シミュレーション速度の最小値")]
    private float minSimulationSpeed;

    /// <summary>
    /// シミュレーション速度の最大値
    /// </summary>
    [SerializeField, Header("シミュレーション速度の最大値")]
    private float maxSimulationSpeed;

    /// <summary>
    /// シミュレーション速度調整用InputField
    /// </summary>
    [SerializeField, Header("シミュレーション速度調整用InputField")]
    private InputField simulationSpeedInputField;


    private SimulationManager simulationManager;

    /// <summary>
    /// ID-POSデータファイルを選択するDropdown
    /// </summary>
    [SerializeField, Header("ファイル選択用のドロップダウン")]
    private Dropdown fileSelectDropdown;

    void Start()
    {
        //----ここから他オブジェクトのコンポーネントの取得----

        simulationManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationManager>();

        //----ここまで他オブジェクトのコンポーネントの取得----

        simulationSpeedSlider.maxValue = maxSimulationSpeed;
        simulationSpeedSlider.minValue = minSimulationSpeed;
        simulationSpeedSlider.onValueChanged.AddListener(ValidateSliderValue);

        simulationSpeedInputField.onEndEdit.AddListener(ValidateInputFieldValue);
        simulationSpeedInputField.text = "" + 1;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// シミュレーション速度調整用Sliderの値を適用する
    /// </summary>
    /// <param name="value">Sliderの値</param>
    private void ValidateSliderValue(float value)
    {
        simulationManager.simulationSpeed = value;
        //simulationSpeedInputField.placeholder.GetComponent<Text>().text = "" + value;
        simulationSpeedInputField.text = "" + value;
    }

    /// <summary>
    /// シミュレーション速度調整用InputFieldの値を適用する
    /// </summary>
    /// <param name="value">InputFieldのテキストの値</param>
    private void ValidateInputFieldValue(string value)
    {
        simulationManager.simulationSpeed = float.Parse(value);
        simulationSpeedSlider.value = float.Parse(value);
    }

    private void OnApplicationFocus(bool focus)
    {
        // フォーカスされたとき
        if(focus)
        {
            ReadIDPosDataFiles();
        }
    }

    /// <summary>
    /// ID-POSデータファイルを読み込んでfileSelectDropDownのアイテムに追加
    /// </summary>
    private void ReadIDPosDataFiles()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/PosDatas");
        if(directoryInfo.Exists == false)
        {
            directoryInfo.Create();
        }

        FileInfo[] fileInfos = directoryInfo.GetFiles();
        List<string> fileNames = new List<string>();
        foreach(FileInfo fileInfo in fileInfos)
        {
            if (fileInfo.Name.Contains("meta"))
                continue;
            fileNames.Add(fileInfo.Name);
        }
        fileSelectDropdown.AddOptions(fileNames);
    }


    //private string GetNewDataFile()
    //{
    //    OpenFileDialog openFileDialog = new OpenFileDialog();

    //}
}
