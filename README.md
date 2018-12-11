# AdventCalendar_KUT_20181211

これは[高知工科大 Advent Calendar 2018](https://adventar.org/calendars/2959)の11日目の記事です。

ネタなさ過ぎてどうしようかと思ってたら前日にゲームの記事書いてた人居たので便乗しようかと思います。  
[Pyxelでパックマンぽいゲームを作る 前編](https://qiita.com/stmn/items/4048d4af2a9613594b60)

## 最短経路問題

突然ですが最短経路問題です。
ゲームでは敵が追っかけてくる時とかオート戦闘モード時の自動移動とかで最短経路を求める問題が非常によく出てきます。
昨日の記事のパックマンの敵AIとかもそれ系だったりします。

最短経路問題と言えばダイクストラ法が有名ですが、これは大学の授業でもやりましたし、ゲームではA\*というのがよく使われてるのでこれの紹介でもやろうかと思います。 (情報はA\*も授業でやってるん?)

UnityのNavMeshでも使われてます。  
https://docs.unity3d.com/ja/current/Manual/nav-InnerWorkings.html

## A*
- https://ja.wikipedia.org/wiki/A*
- https://qiita.com/2dgames_jp/items/f29e915357c1decbc4b7
- http://tech.nitoyon.com/ja/blog/2010/01/26/dijkstra-aster-visualize/

説明しようかと思ったけど他のサイトの説明文に勝てる気がしないので詳しくはリンク張ったやつとかを読んでほしい。:innocent:

非常に大雑把に一言説明するとダイクストラ法で次のノードを選ぶ時に"ゴールまでの推定距離"を使っていい感じに進んでいく感じのやつです。

まぁ百聞は一見に如かずってことでA*で遊べるやつ作ったのでこれで頑張ってほしい。  
https://hikarin522.github.io/AdventCalendar_KUT_20181211/

(時間なさ過ぎてバグってそうやからプルリクよろ :innocent:)
