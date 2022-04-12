# PurchaseSimulatorUsingIDPosData

同期の研究のシステムの開発依頼が来たので，開発しました．  
POSデータを読み取って，客の購買行動をシミュレーションするシステムです．  
研究倫理の関係により，提出データに含まれているPOSデータは一部のデータのみを使用しているため，シミュレーション可能な時間は短いです．

## URL

同期の研究紹介ページです．

https://web.wakayama-u.ac.jp/~yoshino/lab/research/nakamura_2021/

## 使い方

1. masterブランチをクローン
2. Unityでプロジェクトを開く
   * Unityのバージョンは，2019.2.0f1を推奨
3. UnityのPlayボタンを押す
4. 右上のStartボタンを押す．
   * これでシミュレーションがスタートします

## 開発環境

* Unity
* C#
* NavMeshAgent
  * 客の購買行動のシミュレーションに利用

## 工夫した点

* 購買行動のシミュレーションに用いたNavMeshAgentが，デフォルトに仕様では理想通りに動いてくれなかったので，NavMeshAgentの仕様を調べ，一部カスタマイズしました．
