# 必要なものリスト

## 必要な機能

### ダイクストラ法による経路探索

* ID-POSデータから購入した商品のリストを取得
  * GetProductsList() : List string
* リストに対して
  * 開始地点と終了地点を設定して経路探索
    * GetRoute(string start, string goal) : List Vector2
* 探索した経路に沿って移動する

//1120	124	7	2	J0283562000000	カナダ三元豚ロースカツ用
1120	124	1	2	J0283566000000	カナダ三元豚ロースしゃぶしゃぶ用
1110	110	2	3	J0400100013489	人参
1150	162	2	1	J4901160016404	おかめ納豆  つゆたっぷり納豆
1210	228	1	1	J4901815888059	スドー  紙カップいちごジャム
1210	228	1	1	J4901815888073	スドー  紙カップチョコレート
1110	110	4	2	J4908301334519	ピーマン
1150	173	1	2	J4903110003205	ヤマザキメロンパンの皮３
1210	211	1	5	J4901340016743	アサヒ  濃いめのカルピス



