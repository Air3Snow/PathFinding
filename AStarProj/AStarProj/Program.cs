using System;
using System.Collections.Generic;

namespace AStarProj
{
    class Program
    {
        static List<GridCell> openList;
        static List<GridCell> closeList;

        // 0 可走,1 不可走
        public static int[,] mapCells = 
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },	// 0
			{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 0, 1, 1, 0, 0 },   // 4
			{ 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0 }    // 8
			//0				 5			 9
        };

        static void Main(string[] args)
        {
            Console.WriteLine("开始A*测试");
            GridCell start = new GridCell(3,1,true);
            GridCell end = new GridCell(5,8,true);
            // 生成地图
            GridCell[,] nodes = InitGridMap(mapCells);
            Findpathing(start, end, nodes);




            Console.ReadLine();


        }

 

        /// <summary>
        /// 寻路
        /// </summary>
        static void Findpathing(GridCell start, GridCell end, GridCell[,] nodes)
        {
            openList.Clear();
            // 起点加入openList
            // 加入openList之前计算他的F值,起点不需要
            
            start.G = 0;
            start.F = 0;
            openList.Add(start);
            GridCell node;
            


            // 处理当前节点
            GridCell currentNode = null;

            HandleCurrentNode(currentNode, nodes, ref openList, ref closeList, start, end); 
            while (openList.Count > 0)
            {
                
                

            }

        }

        private static void HandleCurrentNode(GridCell currentNode, GridCell[,] nodes, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end)
        {

            // 遍历openList,查找F值最小的节点
            int index = FindMinF(openList);
            currentNode = openList[index];

            #region 处理该节点

            closeList.Add(currentNode);


            // 处理他的邻居:
            // 1.获取邻居   2.好邻居计算他的F值,并加入openList 3.
            List<GridCell> neighbors = HandleNeighbors(currentNode, nodes, ref openList, ref closeList, start, end);
            
            
            
            openList.RemoveAt(index);

            #endregion
        }

        private static List<GridCell> HandleNeighbors(GridCell currentNode, GridCell[,] nodes, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end)
        {

           
            
            
            int x = currentNode.X;
            int y = currentNode.Y;
            List<GridCell> neighbors = null;

            // 传入邻居,判断是好邻居就加入openList,并且计算他的F值
            GoodNeighbor(nodes[x - 1, y - 1], currentNode, ref neighbors, ref closeList, start, end, false);
            GoodNeighbor(nodes[x    , y - 1], currentNode, ref neighbors, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y - 1], currentNode, ref neighbors, ref closeList, start, end, false);
            GoodNeighbor(nodes[x - 1, y    ], currentNode, ref neighbors, ref closeList, start, end, true);
            GoodNeighbor(nodes[x - 1, y + 1], currentNode, ref neighbors, ref closeList, start, end, false);
            GoodNeighbor(nodes[x    , y - 1], currentNode, ref neighbors, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y    ], currentNode, ref neighbors, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y + 1], currentNode, ref neighbors, ref closeList, start, end, false);

            return neighbors;
        }




        /// <summary>
        /// 好邻居就加入
        /// </summary>
        /// <param name="neighbor"></param>
        /// <param name="currentNode"></param>
        /// <param name="neighbors"></param>
        /// <param name="closeList"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="addDistance"></param>
        private static void GoodNeighbor(GridCell neighbor ,GridCell currentNode, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end, bool isStright)
        {
            // 找到该节点的所有邻居,要求可行,且不在closeList中,也不在openList
            // ?这里Contains可能是比较hashcode
            if (neighbor.isWalkable && !closeList.Contains(neighbor) && !openList.Contains(neighbor))
            {
                // 指向父节点,是直的还是斜的
                neighbor.isStright = isStright;
                // 指向currentNode
                neighbor.parent = currentNode;
                // 计算他的G,H,F
                CalculateGHF(ref neighbor, start, end);
                // 加入openList
                openList.Add(currentNode);

            }
        }

        // 根据邻居节点的原节点计算邻居节点的GHF值
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
            neighbor.H = System.Math.Abs(neighbor.X - end.X) + System.Math.Abs(neighbor.Y - end.Y);
            // 计算F
            neighbor.F = neighbor.G + neighbor.H;
            
            
        }



        /// <summary>
        /// 返回openList中最小的F值的节点
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

    }
}
