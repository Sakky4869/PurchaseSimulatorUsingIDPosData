using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    /// <summary>
    /// 商品の名前と位置情報を紐づけて保存する辞書配列
    /// </summary>
    private Dictionary<string, Vector2> productPositions;

    /// <summary>
    /// シミュレーション速度（というよりUnityのタイムスケール）
    /// </summary>
    public float simulationSpeed { get { return Time.timeScale; } set { Time.timeScale = value; } }

    [SerializeField]
    private UnityEngine.UI.Button startSimulationButton;



    void Start()
    {
        startSimulationButton.onClick.AddListener(StartSimulation);
    }

    void Update()
    {

    }

    private void StartSimulation()
    {
        StartCoroutine(Simulate());
    }

    private IEnumerator Simulate()
    {
        yield return null;
    }



    /// <summary>
    /// 商品を探し，名前と位置情報の辞書配列を返す
    /// </summary>
    /// <returns>名前と位置情報の辞書配列</returns>
    private Dictionary<string, Vector2> GetProductPositions()
    {
        Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();
        GameObject[] products = GameObject.FindGameObjectsWithTag("Product");
        foreach(GameObject product in products)
        {
            positions.Add(product.GetComponent<Product>().productName, product.GetComponent<RectTransform>().anchoredPosition);
        }
        return positions;
    }

    /// <summary>
    /// 商品の位置を辞書配列から探して返す
    /// </summary>
    /// <param name="productName">商品の名前</param>
    /// <returns>商品の位置情報</returns>
    public Vector2 GetProductPosition(string productName)
    {
        return productPositions[productName];
    }


}
