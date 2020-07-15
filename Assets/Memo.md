# 必要なものリスト

## 必要な機能

### ダイクストラ法による経路探索

* ID-POSデータから購入した商品のリストを取得
  * GetProductsList() : List string
* リストに対して
  * 開始地点と終了地点を設定して経路探索
    * GetRoute(string start, string goal) : List Vector2
* 探索した経路に沿って移動する
