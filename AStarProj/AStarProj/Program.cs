using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AStarProj
{
    class Program
    {


        // < 1 可走,> 1 不可走 .为了省去查找邻居的时候判断数组越界,四周用1不可到达
        public static int[,] mapCells =
        {
            { 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},  // 0
            { 1 ,0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1},
            { 1 ,0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1},
            { 1 ,0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1},
            { 1 ,0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1},
            { 1 ,0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 1},   // 5
			{ 1 ,0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1},
            { 1 ,0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1},
            { 1 ,0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1},
            { 1 ,0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            { 1 ,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}    // 10
			//0		   3		 5			 9
        };

        // 结果地图
        public static int[,] findPathMap;

        // 初始化接口,接受地图
        // 接口接收x,y: 如果是end变化,重新寻路.如果是障碍物变化,判断是否影响已经生成的寻路坐标,是则重新寻路
        // 测试一下压力,单线程跑1000次
        static void Main(string[] args)
        {
            Console.WriteLine("开始A*测试");
            GridCell start = new GridCell(3, 1, true);
            GridCell end = new GridCell(5, 8, true);
            AStarPathFinding pf = new AStarPathFinding();
            Stack<GridCell> pathStack;

            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            
            pathStack = pf.PathFinding(start, end, mapCells);
            
            stopwatch.Stop(); //  停止监视
            TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            double hours = timespan.TotalHours; // 总小时
            double minutes = timespan.TotalMinutes;  // 总分钟
            double seconds = timespan.TotalSeconds;  //  总秒数
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
            Console.WriteLine(milliseconds);
            Console.WriteLine(seconds);
            Console.WriteLine(minutes);

            #region 成功则打印结果
            if (pathStack != null)
            {

                Console.WriteLine("输出结果:");

                mapCells[start.X, start.Y] = 5;
                mapCells[end.X, end.Y] = 5;
                findPathMap = (int[,])mapCells.Clone();
                GridCell node;

                Console.WriteLine("输出回溯坐标:");
                while (pathStack.Count > 0)
                {
                    node = pathStack.Pop();
                    findPathMap[node.X,node.Y] = 5;
                }

                Console.WriteLine("输出原先地图:");

                for (int x = 0; x < mapCells.GetLength(0); x++)
                {
                    for (int y = 0; y < mapCells.GetLength(1); y++)
                    {
                        Console.Write(" " + mapCells[x, y]);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("输出结果地图:");

                for (int x = 0; x < findPathMap.GetLength(0); x++)
                {
                    for (int y = 0; y < findPathMap.GetLength(1); y++)
                    {
                        Console.Write(" " + findPathMap[x, y]);
                    }
                    Console.WriteLine();
                }


            }
            #endregion
            Console.ReadLine();


        }






    }
}
