using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Link : MonoBehaviour
{

    //[HideInInspector]
    public Production firstProduction;

    private Vector2 beforePosFirstProduction;

    //[HideInInspector]
    public Production secondProduction;

    private Vector2 beforePosSecondProduction;

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
        if (firstProduction == null)
            return;
        if (secondProduction == null)
            return;
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
        Vector2 centerPos = (first.rectTransform.anchoredPosition + second.rectTransform.anchoredPosition) / 2;
        float dist = Vector2.Distance(first.rectTransform.anchoredPosition, second.rectTransform.anchoredPosition);
        Vector2 dt = second.rectTransform.anchoredPosition - first.rectTransform.anchoredPosition;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;
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
    /// <param name="production"></param>
    /// <returns></returns>
    public Production GetOpponent(Production production)
    {
        return (production == firstProduction) ? secondProduction : firstProduction;
    }

    public void DeleteLink()
    {
        StartCoroutine(DeleteLinkCor());
    }

    public IEnumerator DeleteLinkCor()
    {
        yield return null;
        Destroy(gameObject);
    }
}
