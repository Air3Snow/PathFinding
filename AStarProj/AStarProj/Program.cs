using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AStarProj
{
    class Program
    {
        static List<GridCell> openList = new List<GridCell>();
        static List<GridCell> closeList = new List<GridCell>();

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
            GridCell start = new GridCell(3,1,true);
            GridCell end = new GridCell(5,8,true);

            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            for (int i = 0; i < 1000; i++)
            {
                // 生成地图
                GridCell[,] nodes = InitGridMap(mapCells);
                bool isSuccess = Findpathing(start, end, nodes);
            }
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
            /*if (isSuccess)
            {
                
                Console.WriteLine("输出结果:");

                mapCells[start.X, start.Y] = 5;
                mapCells[end.X, end.Y] = 5;
                findPathMap = (int[,])mapCells.Clone();

                Console.WriteLine("输出回溯坐标:");
                GridCell findNode = openList.Find(c => c.GetHashCode() == end.GetHashCode());
                while (findNode.parent != null)
                {
                    Console.WriteLine(findNode);
                    findPathMap[findNode.X, findNode.Y] = 5;
                    findNode = findNode.parent;
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

                
            }*/
            #endregion
            Console.ReadLine();


        }



        /// <summary>
        /// 开始寻路
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="nodes">地图</param>
        /// <returns></returns>
        static bool Findpathing(GridCell start, GridCell end, GridCell[,] nodes)
        {
            
            // 起点加入openList
            
            start.G = 0;
            start.F = 0;
            openList.Add(start);
            GridCell node;
            
            // 循环处理当前节点
            int i = 0;
            while (!openList.Contains(end))
            {
                // 如果openList 为空,说明不可到达end
                if (openList.Count == 0)
                {
                    Console.WriteLine("end不可到达");
                    return false;
                    
                }
                // Console.WriteLine("第"+i+++"次循环");
                HandleCurrentNode(nodes, ref openList, ref closeList, start, end);
            }

            return true;

        }

        /// <summary>
        /// 处理当前节点,包括
        /// 1) 遍历openList,查找F值最小的节点作为当前节点 currentNode.
        /// 2) currentNode 加入 closeList, 移出 openList
        /// 3) 处理当前节点的邻居
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="openList"></param>
        /// <param name="closeList"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private static void HandleCurrentNode(GridCell[,] nodes, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end)
        {
            // 遍历openList,查找F值最小的节点
            GridCell currentNode = openList.Aggregate((g1,g2) => g1.F < g2.F ?g1 : g2 );
            //Console.WriteLine("F值最小的节点:" + currentNode);

            #region 处理该节点

            closeList.Add(currentNode);
            openList.Remove(currentNode);

            // 处理他的邻居:
            // 1.获取邻居   2.好邻居计算他的F值,并加入openList 3.
            HandleNeighbors(currentNode, nodes, ref openList, ref closeList, start, end);
            
            
            

            //Console.WriteLine("openList里还有多少数据:" + openList.Count);
            #endregion
        }

        /// <summary>
        /// 处理当前节点的邻居
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="nodes"></param>
        /// <param name="openList"></param>
        /// <param name="closeList"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private static void HandleNeighbors(GridCell currentNode, GridCell[,] nodes, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end)
        {

            int x = currentNode.X;
            int y = currentNode.Y;

           
            // 把地图边缘都设为不可到达,省略数组越界判断  
            GoodNeighbor(nodes[x - 1, y - 1], currentNode, ref openList, ref closeList, start, end, false);
            GoodNeighbor(nodes[x    , y - 1], currentNode, ref openList, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y - 1], currentNode, ref openList, ref closeList, start, end, false);
            GoodNeighbor(nodes[x - 1, y    ], currentNode, ref openList, ref closeList, start, end, true);
            GoodNeighbor(nodes[x - 1, y + 1], currentNode, ref openList, ref closeList, start, end, false);
            GoodNeighbor(nodes[x    , y + 1], currentNode, ref openList, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y    ], currentNode, ref openList, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y + 1], currentNode, ref openList, ref closeList, start, end, false);

        }




        /// <summary>
        /// 处理当前节点的邻居
        /// 1) 筛选所有可行,且不在 closeList 中的邻居
        /// 2) 如果已经在 openList 里,检查这条路径是否更好
        /// 3) 如果已经不在 openList 里,计算GHF,指向 currentNode ,加入 openList
        /// </summary>
        /// <param name="neighbor"></param>
        /// <param name="currentNode"></param>
        /// <param name="openList"></param>
        /// <param name="closeList"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isStright">neighbor 节点是直着指向 currentNode,还是斜着,用于计算G值</param>
        private static void GoodNeighbor(GridCell neighbor ,GridCell currentNode, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end, bool isStright)
        {
            // 找到该节点的所有邻居,要求可行,且不在 closeList 中
            // 这里Contains可能是比较hashcode
            if (neighbor.isWalkable && !closeList.Contains(neighbor))
            {
                // 如果已经在 openList 里面,
                if (openList.Contains(neighbor))
                {
                    // 检查这条路径 ( 即经由当前方格到达它那里 ) 是否更好
                    if (isStright)
                    {
                        if (currentNode.G+10 < neighbor.G)
                        {
                            neighbor.G = currentNode.G + 10;
                            neighbor.parent = currentNode;
                        }
                    } else
                    {
                        if (currentNode.G + 14 < neighbor.G)
                        {
                            neighbor.G = currentNode.G + 14;
                            neighbor.parent = currentNode;
                        }
                    }
                }
                // 不在openList里面,加入openList
                else
                {
                    
                    // 指向父节点,是直的还是斜的
                    neighbor.isStright = isStright;
                    // 指向currentNode
                    neighbor.parent = currentNode;
                    // 计算他的G,H,F
                    CalculateGHF(ref neighbor, start, end);
                    // 加入openList
                    openList.Add(neighbor);
                }
                

            }
        }

        /// <summary>
        ///  根据该节点是直着指向父节点,还是斜着,计算G值
        /// </summary>
        /// <param name="neighbor"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private static void CalculateGHF(ref GridCell neighbor, GridCell start, GridCell end)
        {
            // 计算G
            if (!neighbor.isStright)
            {
                neighbor.G = neighbor.parent.G + 14;

            } else
            {
                neighbor.G = neighbor.parent.G + 10;
            }

            // 计算H
            neighbor.H = (System.Math.Abs(neighbor.X - end.X) + System.Math.Abs(neighbor.Y - end.Y)) * 10;
            // 计算F
            neighbor.F = neighbor.G + neighbor.H;
            
            
        }



        /// <summary>
        /// 初始化生成地图
        /// </summary>
        /// <param name="mapCells"></param>
        /// <returns></returns>
        public static GridCell[,] InitGridMap(int[,] mapCells)
        {
            // 获取多维数组的维度
            int dimension = mapCells.Rank;
            // GetLength,获取指定维度的长度
            GridCell[,] nodes = new GridCell[mapCells.GetLength(0), mapCells.GetLength(1)];

            for (int i = 0; i < mapCells.GetLength(0); i++)
            {
                for (int j = 0; j < mapCells.GetLength(1); j++)
                {
                    GridCell gridCell = new GridCell(i, j, mapCells[i, j] == 0);
                    nodes[i, j] = gridCell;

                }
            }

            return nodes;
        }

        /// <summary>
        /// 返回openList中最小的F值的节点的index坐标.
        /// 废弃了,使用openList.Aggregate((g1,g2) => g1.F < g2.F ?g1 : g2 ); 更短小精悍
        /// </summary>
        /// <param name="openList"></param>
        /// <returns></returns>
        private static int FindMinF(List<GridCell> openList)
        {
            int minFNodeIndex = 0;
            int minF = -1;
            int i = 0;

            foreach (var node in openList)
            {
                if (node.F < minF)
                {
                    minF = node.F;
                    minFNodeIndex = i;
                }
                i++;
            }

            return minFNodeIndex;
        }


    }
}
