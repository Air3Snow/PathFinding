using System;
using System.Collections.Generic;
using System.Text;

namespace AStarProj
{
    /// <summary>
    /// 网格对象
    /// </summary>
    public class GridCell 
    {
        public int X { get; set; }
        public int Y { get; set; }

        // G = 从起点 A 移动到指定方格的移动代价，沿着到达该方格而生成的路径。
        // H = 从指定的方格移动到终点 B 的估算成本(不计算障碍物)
        // F = G+H
        public int G { get; set; }
        public int H { get; set; }
        public int F { get; set; }
        
        // 是否可走
        public Boolean isWalkable;

        // 父节点
        public GridCell parent;

        // 是否直的 
        public bool isStright;

        public GridCell(int x, int y, bool isWalkable)
        {
            X = x;
            Y = y;
            this.isWalkable = isWalkable;
        }

        public GridCell(int x, int y)
        {
            X = x;
            Y = y;
        }

        // 重写比较方法
        public override bool Equals(object obj)
        {

            return obj is GridCell cell &&
                   X == cell.X &&
                   Y == cell.Y;
        }


        // 重写hashcode,为啥.net 4.7没有这个方法
        public override int GetHashCode()
        {
            Console.WriteLine("调用hashcode方法");
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return "( X:" + X + " , " + " Y:" + Y + " , " + " G:" + G + " , " + " H:" + H + " , " + " F:" + F + " )";
        }
    }
}
