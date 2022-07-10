using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathNode
{
    public Vector3 position { get; }
    public PathNode parent { get; set; }
    public Block block { get; }
    public int weigth { get; set; }
    public PathNode(PathNode parent, Block block, Vector3 position)
    {
        this.parent = parent;
        this.block = block;
        this.position = position;
        weigth = 0;
    }
};

public class Pathfinder
{
    private Dictionary<Vector3, Block> field;
    private PathNode startNode, finishNode;
    private List<PathNode> nodes = new List<PathNode>();

    public void SetField(Dictionary<Vector3, Block> blocks)
    {
        field = blocks;
    }

    public List<Vector3> EstimatePath(Vector3 start, Vector3 target)
    {
        List<Vector3> path = new List<Vector3>();
        if (!field.ContainsKey(start) && !field.ContainsKey(target))
        {
            Debug.Log($"Поле не содержит ячеек: ({start}) и ({target})");
            return path;
        }
        finishNode = null;
        nodes.Clear();
        startNode = new PathNode(null, field[start], start);
        nodes.Add(startNode);

        PathNode current = null;
        HashSet<PathNode> openSet = new HashSet<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();
        openSet.Add(startNode);

        while (openSet.Count() != 0)
        {
            current = openSet.First();

            foreach(var node in openSet)
            {
                if (node.weigth < current.weigth)
                {
                    current = node;
                }
            }
            closedSet.Add(current);
            openSet.Remove(current);

            // Условие достижения нужной ячейки
            if (current.position == target)
            {
                finishNode = current;
                break;
            }

            foreach (var direction in current.block.PossibleDirections)
            {
                Vector3 newPoint = current.position + direction;
                // Проверяем не вычислена ли точка ранее
                if (closedSet.FirstOrDefault(x => x.position == newPoint) != null)
                {
                    continue;
                }

                // Проверка что в текущем направлении существует ячейка
                if (!field.ContainsKey(newPoint)) continue;
                // Проверка что мы не вернулись обратно
                if (newPoint == start) continue;
                // Проверка что в новую ячейку можно попасть с текущего направления
                if (!field[newPoint].PossibleDirections.Contains(-direction)) continue;
                // Если в открытом сете нет ноды с такими координатами
                var newNode = openSet.FirstOrDefault(x => x.position == newPoint);
                
                if (newNode == null)
                {
                    newNode = new PathNode(current, field[newPoint], newPoint);
                    // У направления всегда есть не нулевая координата
                    newNode.weigth = direction.x != 0 && direction.y != 0 ? 14 : 10; // вес направления
                    newNode.weigth += (int)(Mathf.Abs(target.x - newPoint.x) * 10); // эвристический вес
                    openSet.Add(newNode);
                }
                else
                {
                    newNode.parent = current;
                    // У направления всегда есть не нулевая координата
                    newNode.weigth = direction.x != 0 && direction.y != 0 ? 14 : 10; // вес направления
                    newNode.weigth += (int)(Mathf.Abs(target.x - newPoint.x) * 10); // эвристический вес
                }
            }
        }

        current = finishNode;
        if(current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }
        return path;
    }
}
