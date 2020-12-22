using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStarProj
{
    class AStarPathFinding
    {

        public  List<GridCell> openList = new List<GridCell>();
        public  List<GridCell> closeList = new List<GridCell>();
        GridCell startNode;
        GridCell endNode;

        // 存储结果路径的栈
        public Stack<GridCell> pathStack = new Stack<GridCell>();

        // GridCell格式地图
        public  GridCell[,] nodes;

        /// <summary>
        /// 动态传入x,y,修改网格地图
        /// 如果该路径在 pathList 里,需要重新寻路.否则不变
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isWalkable"></param>
        public void ModifyXY(int x, int y, bool isWalkable)
        {
            GridCell modifyGrid = new GridCell(x,y);
            if (pathStack.Contains(modifyGrid))
            {
                pathStack.Clear();
                openList.Clear();
                closeList.Clear();
                pathStack = PathFinding(startNode, endNode, nodes);
            }
            nodes[x, y].isWalkable = isWalkable;
        }


        /// <summary>
        /// 初始化生成地图 
        /// </summary>
        /// <param name="mapCells">传入二维数组</param>
        /// <returns></returns>
        public GridCell[,] InitGridMap(int[,] mapCells)
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

        public Stack<GridCell> PathFinding(GridCell start, GridCell end, GridCell[,] nodes)
        {
            // 循环处理当前节点
            int i = 0;
            while (!openList.Contains(end))
            {
                // 如果openList 为空,说明不可到达end
                if (openList.Count == 0)
                {
                    // Console.WriteLine("end不可到达");
                    return null;

                }
                // Console.WriteLine("第"+i+++"次循环");
                HandleCurrentNode(nodes, ref openList, ref closeList, start, end);
            }

            // 将结果存进栈(先进后出)
            GridCell findNode = openList.Find(c => c.GetHashCode() == end.GetHashCode());
            // 起点不加入栈
            while (findNode.parent != null)
            {
                pathStack.Push(findNode);

                findNode = findNode.parent;
            }
            return pathStack;

        }

        /// <summary>
        /// 开始寻路,传入star,end
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="mapCells">地图</param>
        /// <returns></returns>
        public Stack<GridCell> PathFinding(GridCell start, GridCell end, int[,] mapCells)
        {
            // 初始化
            pathStack.Clear();
            openList.Clear();
            closeList.Clear();
            this.startNode = start;
            this.endNode = end;
            nodes = InitGridMap(mapCells);

            // 起点加入openList

            start.G = 0;
            start.F = 0;
            openList.Add(start);
            GridCell node;

            return  PathFinding(startNode, endNode, nodes);


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
        private bool HandleCurrentNode(GridCell[,] nodes, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end)
        {
            try
            {
                // 遍历openList,查找F值最小的节点
                GridCell currentNode = openList.Aggregate((g1, g2) => g1.F < g2.F ? g1 : g2);
                //Console.WriteLine("F值最小的节点:" + currentNode);

                #region 处理该节点

                closeList.Add(currentNode);
                openList.Remove(currentNode);

                // 处理他的邻居:
                // 1.获取邻居   2.好邻居计算他的F值,并加入openList 3.
                HandleNeighbors(currentNode, nodes, ref openList, ref closeList, start, end);

                //Console.WriteLine("openList里还有多少数据:" + openList.Count);
                #endregion
                return true;
            }
            catch (Exception)
            {

                return false;
            }

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
        private void HandleNeighbors(GridCell currentNode, GridCell[,] nodes, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end)
        {

            int x = currentNode.X;
            int y = currentNode.Y;


            // 把地图边缘都设为不可到达,省略数组越界判断  
            GoodNeighbor(nodes[x - 1, y - 1], currentNode, ref openList, ref closeList, start, end, false);
            GoodNeighbor(nodes[x, y - 1], currentNode, ref openList, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y - 1], currentNode, ref openList, ref closeList, start, end, false);
            GoodNeighbor(nodes[x - 1, y], currentNode, ref openList, ref closeList, start, end, true);
            GoodNeighbor(nodes[x - 1, y + 1], currentNode, ref openList, ref closeList, start, end, false);
            GoodNeighbor(nodes[x, y + 1], currentNode, ref openList, ref closeList, start, end, true);
            GoodNeighbor(nodes[x + 1, y], currentNode, ref openList, ref closeList, start, end, true);
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
        private void GoodNeighbor(GridCell neighbor, GridCell currentNode, ref List<GridCell> openList, ref List<GridCell> closeList, GridCell start, GridCell end, bool isStright)
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
                        if (currentNode.G + 10 < neighbor.G)
                        {
                            neighbor.G = currentNode.G + 10;
                            neighbor.parent = currentNode;
                            // 更改原先的方向
                            neighbor.isStright = isStright;
                        }
                    }
                    else
                    {
                        if (currentNode.G + 14 < neighbor.G)
                        {
                            neighbor.G = currentNode.G + 14;
                            neighbor.parent = currentNode;
                            neighbor.isStright = isStright;
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
        private void CalculateGHF(ref GridCell neighbor, GridCell start, GridCell end)
        {
            // 计算G
            if (!neighbor.isStright)
            {
                neighbor.G = neighbor.parent.G + 14;

            }
            else
            {
                neighbor.G = neighbor.parent.G + 10;
            }

            // 计算H
            neighbor.H = (System.Math.Abs(neighbor.X - end.X) + System.Math.Abs(neighbor.Y - end.Y)) * 10;
            // 计算F
            neighbor.F = neighbor.G + neighbor.H;


        }





        /// <summary>
        /// 返回openList中最小的F值的节点的index坐标.
        /// 废弃了,使用openList.Aggregate((g1,g2) => g1.F < g2.F ?g1 : g2 ); 更短小精悍
        /// </summary>
        /// <param name="openList"></param>
        /// <returns></returns>
        private int FindMinF(List<GridCell> openList)
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
