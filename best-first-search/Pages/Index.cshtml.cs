using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using best_first_search.Shared;
using System.Linq;

namespace best_first_search.Pages
{
    using static Board;

    public class IndexModel: BlazorComponent
    {
        protected Board Board { get; set; }

        public static int Height => 32;

        public static int Width => 32;

        public async Task AStarAsync()
        {
            // スタート地点追加
            var queue = new List<(double d, int x, int y)> {
                { (this.getDistanceToGoal(this.Board.StartPos.x, this.Board.StartPos.y), this.Board.StartPos.x, this.Board.StartPos.y) }
            };

            // スタート地点からの距離と親
            var costs = new (int d, Parent p)[Height, Width];

            while (queue.Count > 0)
            {
                var val = queue[0];
                var pos = (val.x, val.y);
                queue.RemoveAt(0);

                if (this.getFieldFlg(pos.x, pos.y) == FieldFlag.Goal)
                    break;

                void enqueue(int x, int y, Parent parent)
                {
                    var flg = this.getFieldFlg(x, y);
                    switch (flg)
                    {
                        case FieldFlag.None:
                        case FieldFlag.Open:
                        case FieldFlag.Goal:
                        {
                            var cost = costs[pos.y, pos.x].d;
                            // 新規 or コスト小さくなる場合に更新
                            if (costs[y, x].p == Parent.None || (costs[y, x].p != parent && cost < costs[y, x].d))
                            {
                                costs[y, x] = (cost + 1, parent);

                                // 昇順ソートでインサート
                                var dist = costs[y, x].d + this.getDistanceToGoal(x, y);
                                var i = queue.FindIndex(e => e.d >= dist);
                                queue.Insert(i < 0 ? queue.Count : i, (dist, x, y));

                                if (flg == FieldFlag.None)
                                    this.Board.Field[y, x] = FieldFlag.Open;
                            }

                            break;
                        }
                    }
                }

                // ↑
                enqueue(pos.x, pos.y - 1, Parent.Down);

                // →
                enqueue(pos.x + 1, pos.y, Parent.Left);

                // ↓
                enqueue(pos.x, pos.y + 1, Parent.Up);

                // ←
                enqueue(pos.x - 1, pos.y, Parent.Right);

                this.Board.ReRender();
                await Task.Delay(100);
            }

            for (var pos = this.Board.GoalPos; pos != this.Board.StartPos;)
            {
                if (pos != this.Board.GoalPos)
                    this.Board.Field[pos.y, pos.x] = FieldFlag.Path;

                switch (costs[pos.y, pos.x].p)
                {
                    case Parent.Up:
                        pos = (pos.x, pos.y - 1);
                        break;
                    case Parent.Down:
                        pos = (pos.x, pos.y + 1);
                        break;
                    case Parent.Left:
                        pos = (pos.x - 1, pos.y);
                        break;
                    case Parent.Right:
                        pos = (pos.x + 1, pos.y);
                        break;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// ゴールまでの距離 (マンハッタン餃子)
        /// </summary>
        private double getDistanceToGoal(int x, int y)
        {
            var dx = Math.Abs(this.Board.GoalPos.x - x);
            var dy = Math.Abs(this.Board.GoalPos.y - y);
            return dx + dy;
        }

        private FieldFlag getFieldFlg(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return FieldFlag.Wall;
            }
            return this.Board.Field[y, x];
        }

        enum Parent
        {
            None,
            Up,
            Down,
            Left,
            Right,
        }
    }
}
