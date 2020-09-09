using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ProductionInfoPanel : MonoBehaviour
{
    /// <summary>
    /// 部門のInputField
    /// </summary>
    [SerializeField]
    private InputField bumonInput;

    /// <summary>
    /// AUのInputField
    /// </summary>
    [SerializeField]
    private InputField auInput;
    
    /// <summary>
    /// ラインのInputField
    /// </summary>
    [SerializeField]
    private InputField lineInput;

    /// <summary>
    /// クラスのInputField
    /// </summary>
    [SerializeField]
    private InputField classInput;
    
    /// <summary>
    /// 商品名のInputField
    /// </summary>
    [SerializeField]
    private InputField nameInput;


    [SerializeField]
    private Production production;

    /// <summary>
    /// マウスが上にあるかどうか
    /// </summary>
    private bool mouseIn;


    void Start()
    {
        nameInput.onEndEdit.AddListener(EndEditProductionName);
        bumonInput.onEndEdit.AddListener(EndEditProductionMetaData);
        auInput.onEndEdit.AddListener(EndEditProductionMetaData);
        lineInput.onEndEdit.AddListener(EndEditProductionMetaData);
        classInput.onEndEdit.AddListener(EndEditProductionMetaData);

        //nameInput.
        mouseIn = false;
        //gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(mouseIn == false)
            {
                gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable selectable = null;
            if (bumonInput.isFocused)
            {
                selectable = auInput.GetComponent<Selectable>();
            }else if (auInput.isFocused)
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
    /// 商品のデータを情報パネルに設定する
    /// </summary>
    /// <param name="bumon">部門</param>
    /// <param name="au">AU</param>
    /// <param name="line">ライン</param>
    /// <param name="_class">クラス</param>
    /// <param name="name">商品名</param>
    public void SetValueToInputField(string bumon, string au, string line, string _class, string name)
    {
        //Debug.Log("bumon : " + bumon);
        //bumonInput.textComponent.text = bumon;
        bumonInput.text = bumon;
        //Debug.Log(bumonInput.text);
        //auInput.textComponent.text = au;
        auInput.text = au;
        //lineInput.textComponent.text = line;
        lineInput.text = line;
        //classInput.textComponent.text = _class;
        classInput.text = _class;
        //nameInput.textComponent.text = name;
        nameInput.text = name;
        //gameObject.SetActive(false);
        //Debug.Log("値の設定");
    }

    private void EndEditProductionName(string data)
    {
        production.productionName = nameInput.text;
        if (production.productionName.Contains("\r"))
            production.productionName.Replace("\r", "");
    }

    private void EndEditProductionMetaData(string data)
    {
        // 部門・AU・ライン・クラスが一致して，商品名が異なるものがあったので，
        // 商品名も含めてメタデータにしようとしたが，商品名の最後になぜか改行コードが入るので，いったんとりやめ
        production.metaData = bumonInput.text + "," + auInput.text + "," + lineInput.text + "," + classInput.text;// + "," + nameInput.text;
        if (production.metaData.Contains("\r"))
            production.metaData.Replace("\r", "");
    }

    public void SaveProductionData()
    {
        production.productionName = nameInput.text;
        if (production.productionName.Contains("\r"))
            production.productionName.Replace("\r", "");
        production.metaData = bumonInput.text + "," + auInput.text + "," + lineInput.text + "," + classInput.text;// + "," + nameInput.text;
        if (production.metaData.Contains("\r"))
            production.metaData.Replace("\r", "");
    }

    public void OnPointerEnter()
    {
        mouseIn = true;
    }

    public void OnPointerExit()
    {
        mouseIn = false;
    }


}
