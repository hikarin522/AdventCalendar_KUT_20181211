using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using best_first_search.Shared;
using Microsoft.AspNetCore.Blazor.Components;

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
            var queue = new PriorityQueue<double, (int x, int y)> {
                { this.getDistanceToGoal(this.Board.StartPos.x, this.Board.StartPos.y), (this.Board.StartPos.x, this.Board.StartPos.y) }
            };

            // スタート地点からの距離と親
            var distanceTable = new int[Height, Width];
            var parentTable = new Parent[Height, Width];

            (int x, int y) getParentPos((int x, int y) pos)
            {
                var (x, y) = pos;
                switch (parentTable[y, x])
                {
                    case Parent.Up:
                        return (x, y - 1);
                    case Parent.Down:
                        return (x, y + 1);
                    case Parent.Left:
                        return (x - 1, y);
                    case Parent.Right:
                        return (x + 1, y);
                    default:
                        return (x, y);
                }
            }

            while (queue.Count > 0)
            {
                var (val, (x, y)) = queue.Pop();

                if (this.getFieldFlg(x, y) == FieldFlag.Goal)
                {
                    for (var pos = getParentPos((x, y)); pos != this.Board.StartPos; pos = getParentPos(pos))
                    {
                        this.Board.Field[pos.y, pos.x] = FieldFlag.Path;
                    }
                    break;
                }

                void enqueue(int _x, int _y, Parent parent)
                {
                    var flg = this.getFieldFlg(_x, _y);
                    switch (flg)
                    {
                        case FieldFlag.None:
                        case FieldFlag.Open:
                        case FieldFlag.Goal:
                        {
                            var distance = distanceTable[y, x] + 1;
                            // 新規 or コスト小さくなる場合に更新
                            if (parentTable[_y, _x] == Parent.None || distance < distanceTable[_y, _x])
                            {
                                distanceTable[_y, _x] = distance;
                                parentTable[_y, _x] = parent;
                                queue.Push(distance + this.getDistanceToGoal(_x, _y), (_x, _y));

                                if (flg == FieldFlag.None)
                                {
                                    this.Board.Field[_y, _x] = FieldFlag.Open;
                                }
                            }

                            break;
                        }
                    }
                }

                // ↑
                enqueue(x, y - 1, Parent.Down);

                // →
                enqueue(x + 1, y, Parent.Left);

                // ↓
                enqueue(x, y + 1, Parent.Up);

                // ←
                enqueue(x - 1, y, Parent.Right);

                // 途中経過を塗る
                for (var pos = (x, y); pos != this.Board.StartPos; pos = getParentPos(pos))
                {
                    this.Board.Field[pos.y, pos.x] = FieldFlag.Path;
                }

                this.Board.ReRender();
                await Task.Delay(100);

                // 元に戻す
                for (var pos = (x, y); pos != this.Board.StartPos; pos = getParentPos(pos))
                {
                    this.Board.Field[pos.y, pos.x] = FieldFlag.Open;
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

        private enum Parent
        {
            None,
            Up,
            Down,
            Left,
            Right,
        }
    }

    internal class PriorityQueue<TKey, TValue>: SortedDictionary<TKey, Stack<TValue>>
    {
        public PriorityQueue()
            : base() { }

        public PriorityQueue(IComparer<TKey> comparer)
            : base(comparer) { }

        public void Add(TKey key, TValue value)
        {
            this.Push(key, value);
        }

        private readonly Stack<Stack<TValue>> cache = new Stack<Stack<TValue>>();

        public void Push(TKey key, TValue value)
        {
            if (!this.TryGetValue(key, out var stack))
            {
                stack = this.cache.Count != 0 ? this.cache.Pop() : new Stack<TValue>();
                this.Add(key, stack);
            }
            stack.Push(value);
        }

        public KeyValuePair<TKey, TValue> Pop()
        {
            var (key, stack) = this.First();

            var value = stack.Pop();
            if (stack.Count == 0)
            {
                this.Remove(key);
                this.cache.Push(stack);
            }

            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public KeyValuePair<TKey, TValue> Peek()
        {
            var (key, stack) = this.First();
            return new KeyValuePair<TKey, TValue>(key, stack.Peek());
        }
    }

    internal static class Extensions
    {
        public static void Deconstruct<T, U>(this KeyValuePair<T, U> pair, out T key, out U value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
