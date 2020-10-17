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
}
