using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dijkstra : MonoBehaviour
{
    [SerializeField]
    private Button buttonPrefab;

    [SerializeField]
    private RectTransform parent;

    private List<DijkstraCell> cells;

    [HideInInspector]
    public DijkstraCell startCell, goalCell;


    void Start()
    {
        SpawnButtons();
    }

    void Update()
    {
        
    }



    private void SpawnButtons()
    {
        cells = new List<DijkstraCell>();

        for(int i = 1; i <= 9; i++)
        {
            for(int j = 1; j <= 16; j++)
            {
                GameObject obje = Instantiate(buttonPrefab.gameObject, parent);
                RectTransform rect = obje.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                Vector2 pos = rect.anchoredPosition;
                pos.x = rect.sizeDelta.x * j - (rect.sizeDelta.x / 2);
                pos.y = rect.sizeDelta.y * i - (rect.sizeDelta.y / 2);
                rect.anchoredPosition = pos;
                cells.Add(obje.GetComponent<DijkstraCell>());
            }
        }
    }

    public void ExecuteDijkstra()
    {
        // 始点に０を書き込み，それ以外はintの最大値を代入
        foreach(DijkstraCell cell in cells)
        {
            if (cell == startCell)
                cell.cost = 0;
            else
                cell.cost = int.MaxValue;
        }

        //DijkstraCell minCell = null;
        //while((minCell = GetMinCell(cells)) != null)
        //{
        //    UpdateCostNearCells(minCell);
        //}

        //DijkstraCell searchCell = goalCell;
        //while(searchCell != null)
        //{
        //    searchCell.button.image.color = Color.red;
        //    searchCell = searchCell.beforeCell;
        //}
        StartCoroutine(ExecuteDijkstraCor());

    }

    private IEnumerator ExecuteDijkstraCor()
    {
        yield return null;
        DijkstraCell minCell = null;
        while ((minCell = GetMinCell(cells)) != null)
        {
            yield return StartCoroutine( UpdateCostNearCells(minCell) );
        }

        DijkstraCell searchCell = goalCell;
        while (searchCell != null)
        {
            //searchCell.button.image.color = Color.red;
            //searchCell = searchCell.beforeCell;
            //yield return new WaitForSeconds(0.3f);
        }

    }

    /// <summary>
    /// 未確定のセルのうち，最小コストのセルを探し，そのセルを確定のセルとする
    /// </summary>
    /// <param name="dijkstraCells">すべてのセルのリスト</param>
    /// <returns>最小のコストを持つセル</returns>
    private DijkstraCell GetMinCell(List<DijkstraCell> dijkstraCells)
    {
        DijkstraCell minCell = null;
        foreach(DijkstraCell cell in dijkstraCells)
        {
            if (cell.isConst)
                continue;
            if (minCell == null)
            {
                minCell = cell;
                continue;
            }
            if(cell.cost < minCell.cost)
            {
                minCell = cell;
            }
        }

        if(minCell != null)
            minCell.isConst = true;

        return minCell;
    }

    /// <summary>
    /// コストと経路の更新
    /// </summary>
    /// <param name="cell">探索するセル</param>
    private IEnumerator UpdateCostNearCells(DijkstraCell cell)
    {
        foreach(DijkstraCell nearCell in cell.nearDijkstraCells)
        {
            // セルがなければスキップ
            if (nearCell == null)
                continue;
            
            // 確定していたらスキップ
            if (nearCell.isConst)
                continue;

            // 保存されているコストより小さいコストで移動可能なら，コストと道を更新
            if(GetCost(cell, nearCell) < nearCell.cost)
            {
                //nearCell.beforeCell = cell;
                //nearCell.cost = GetCost(cell, nearCell);
                //nearCell.button.image.color = Color.blue;
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    /// <summary>
    /// 2地点間の移動コスト（距離）を計算する
    /// </summary>
    /// <param name="start">開始セル</param>
    /// <param name="end">終了セル</param>
    /// <returns>移動コスト</returns>
    private float GetCost(DijkstraCell start, DijkstraCell end)
    {
        return Vector2.Distance(start.GetComponent<RectTransform>().anchoredPosition, end.GetComponent<RectTransform>().anchoredPosition);
    }

}
