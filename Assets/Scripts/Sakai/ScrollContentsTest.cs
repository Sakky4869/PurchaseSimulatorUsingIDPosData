using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollContentsTest : MonoBehaviour
{
    [SerializeField]
    private Image imagePrefab;

    [SerializeField]
    private ScrollRect scrollRect;

    /// <summary>
    /// マウスが上にあるかどうか
    /// </summary>
    protected bool mouseIn;

    [HideInInspector]
    public RectTransform myRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
        mouseIn = false;

    }

    // Update is called once per frame
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddChildToContent();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            //RemoveRandomChild();
        }
    }

    private void AddChildToContent()
    {
        // ScrolViewのContentのRectTransformを取得
        RectTransform rectTransform = scrollRect.content.GetComponent<RectTransform>();
        // roomnodeを新しく生成する
        Image img = Instantiate(imagePrefab, transform.position, Quaternion.identity);
        // roomnodeの親オブジェクトをScrolViewのContentに設定
        img.GetComponent<RectTransform>().SetParent(rectTransform);
        // roomnodeのscaleを調整
        img.GetComponent<RectTransform>().localScale = Vector3.one;

        Text text = img.transform // roomnode
            .GetChild(0) // Button
            .GetChild(0).GetComponent<Text>(); // Text
        text.text = "" + Random.Range(0, 10);

    }

    private void RemoveRandomChild(string productionName)
    {
        for(int j = 0; j < scrollRect.content.childCount; j++)
        {
            Transform child = scrollRect.content.GetChild(j);
            if(child.GetChild(0).GetChild(0).GetComponent<Text>().text == productionName)
            {
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        

    }
}
