using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;  // なんかいらんっぽいけど，記述するのが一般的っぽい

public class production : MonoBehaviour
{

    public GameObject production_name = null; // Textオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // オブジェクトからTextコンポーネントを取得
        Text score_text = production_name.GetComponent<Text>();
        // テキストの表示を入れ替える
        score_text.text = "豚肉";
    }
}
