using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商品情報登録パネルの3D版
/// ProductionInfoPanelを継承
/// </summary>
public class ProductionInfoPanel3D : ProductionInfoPanel
{
    /// <summary>
    /// 表示対象のProduction3Dオブジェクト
    /// </summary>
    //[SerializeField]
    public Production3D production3D;

    void Start()
    {
        //Debug.Log("call start");
        nameInput.onEndEdit.AddListener(EndEditProductionName);
        bumonInput.onEndEdit.AddListener(EndEditProductionMetaData);
        auInput.onEndEdit.AddListener(EndEditProductionMetaData);
        lineInput.onEndEdit.AddListener(EndEditProductionMetaData);
        classInput.onEndEdit.AddListener(EndEditProductionMetaData);
        myRectTransform = GetComponent<RectTransform>();
        mouseIn = false;
    }

    void Update()
    {
        // マウスがクリックされたとき
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // マウスがパネルの外にあれば
            if (mouseIn == false)
            {
                // パネルが見えないようにする
                gameObject.SetActive(false);
            }
        }

        // Tabキーを押したら選択されているInputFieldが上から順にずれるようにする
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable selectable = null;
            if (bumonInput.isFocused)
            {
                selectable = auInput.GetComponent<Selectable>();
            }
            else if (auInput.isFocused)
            {
                selectable = lineInput.GetComponent<Selectable>();
            }
            else if (lineInput.isFocused)
            {
                selectable = classInput.GetComponent<Selectable>();
            }
            else if (classInput.isFocused)
            {
                selectable = nameInput.GetComponent<Selectable>();
            }
            else if (nameInput.isFocused)
            {
                selectable = bumonInput.GetComponent<Selectable>();
            }
            if (selectable != null)
                selectable.Select();
        }
    }


    /// <summary>
    /// 商品名の編集が完了したときの処理
    /// </summary>
    /// <param name="data">商品名を編集するInputFieldのデータ</param>
    protected void EndEditProductionName(string data)
    {
        //Debug.Log("商品名変更");
        production3D.productionName = nameInput.text;
        if (production3D.productionName.Contains("\r"))
            production3D.productionName.Replace("\r", "");
    }

    /// <summary>
    /// 商品のメタデータの編集が完了したときの処理
    /// </summary>
    /// <param name="data">メタデータを編集するInputFieldのデータ</param>
    protected void EndEditProductionMetaData(string data)
    {
        //Debug.Log("商品のメタデータ変更");
        // 部門・AU・ライン・クラスが一致して，商品名が異なるものがあったので，
        // 商品名も含めてメタデータにしようとしたが，商品名の最後になぜか改行コードが入るので，いったんとりやめ
        production3D.metaData = bumonInput.text + "," + auInput.text + "," + lineInput.text + "," + classInput.text;// + "," + nameInput.text;
        //Debug.Log(bumonInput.text);
        if (production3D.metaData.Contains("\r"))
            production3D.metaData.Replace("\r", "");
    }

    /// <summary>
    /// 商品のデータを保存する
    /// </summary>
    public void SaveProductionData()
    {
        production3D.productionName = nameInput.text;
        if (production3D.productionName.Contains("\r"))
            production3D.productionName.Replace("\r", "");
        production3D.metaData = bumonInput.text + "," + auInput.text + "," + lineInput.text + "," + classInput.text;// + "," + nameInput.text;
        if (production3D.metaData.Contains("\r"))
            production3D.metaData.Replace("\r", "");
    }



}
