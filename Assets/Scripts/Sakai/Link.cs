using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Link : MonoBehaviour
{
    /// <summary>
    /// リンクをつなげる商品のうち最初の商品
    /// </summary>
    //[HideInInspector]
    public Production firstProduction;

    /// <summary>
    /// 最初の商品の移動直前の位置
    /// 商品の位置が変わっても，Linkが追従するようにする
    /// </summary>
    private Vector2 beforePosFirstProduction;

    /// <summary>
    /// リンクをつなげる商品のうち2つ目の商品
    /// </summary>
    //[HideInInspector]
    public Production secondProduction;

    /// <summary>
    /// 2つ目の商品の移動直前の位置
    /// </summary>
    private Vector2 beforePosSecondProduction;

    /// <summary>
    /// 自分のRectTransform
    /// </summary>
    [HideInInspector]
    public RectTransform rectTransform;

    void Start()
    {
        beforePosFirstProduction = firstProduction.rectTransform.anchoredPosition;
        beforePosSecondProduction = secondProduction.rectTransform.anchoredPosition;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        //Debug.Log(firstProduction.rectTransform.anchoredPosition + ", " + beforePosFirstProduction);
        
        // 商品とつながっていないときはreturn
        // returnとはー＞Config.csの192行目
        if (firstProduction == null)
            return;
        if (secondProduction == null)
            return;

        // 商品とつながっていても，移動していなければreturn
        if (firstProduction.rectTransform.anchoredPosition == beforePosFirstProduction
            && secondProduction.rectTransform.anchoredPosition == beforePosSecondProduction)
            return;

        // 始点と終点のどちらかの商品の位置が変化したら再描画
        DrawLink(firstProduction, secondProduction);
        beforePosFirstProduction = firstProduction.rectTransform.anchoredPosition;
        beforePosSecondProduction = secondProduction.rectTransform.anchoredPosition;
    }

    /// <summary>
    /// 描画
    /// </summary>
    /// <param name="first">始点の商品</param>
    /// <param name="second">終点の商品</param>
    public void DrawLink(Production first, Production second)
    {
        // 始点と終点の商品の位置座標のベクトルの真ん中の地点を計算
        Vector2 centerPos = (first.rectTransform.anchoredPosition + second.rectTransform.anchoredPosition) / 2;
        
        // 始点と終点の距離を計算
        float dist = Vector2.Distance(first.rectTransform.anchoredPosition, second.rectTransform.anchoredPosition);
        
        // Linkの角度を計算
        Vector2 dt = second.rectTransform.anchoredPosition - first.rectTransform.anchoredPosition;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        // 計算結果を用いてLinkの位置と角度を調整
        rectTransform.anchoredPosition = centerPos;
        Vector3 angle = new Vector3(0, 0, degree);
        rectTransform.rotation = Quaternion.Euler(angle);
        Vector2 size = rectTransform.sizeDelta;
        size.x = dist;
        rectTransform.sizeDelta = size;
    }

    /// <summary>
    /// 引数と反対のProductionを返す
    /// </summary>
    /// <param name="production">商品オブジェクト</param>
    /// <returns></returns>
    public Production GetOpponent(Production production)
    {
        return (production == firstProduction) ? secondProduction : firstProduction;
    }

    /// <summary>
    /// Linkの削除
    /// </summary>
    public void DeleteLink()
    {
        StartCoroutine(DeleteLinkCor());
    }

    /// <summary>
    /// リンクの削除をするコルーチン
    /// コルーチンってなんやねん↓
    /// https://www.sejuku.net/blog/83712
    /// </summary>
    /// <returns></returns>
    public IEnumerator DeleteLinkCor()
    {
        // 1フレーム待機
        yield return null;
        Destroy(gameObject);
    }
}
