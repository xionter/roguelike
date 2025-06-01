using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// Строит путь для комнаты от startGridPosition до endGridPosition и добавляет
    /// шаги движения в возвращаемый стек. Возвращает null, если путь не найден.
    /// </summary>
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // Корректировка позиций по нижним границам
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // Создание открытого списка и хэш-набора закрытых узлов
        var openNodeList = new List<Node>();
        var closedNodeHashSet = new HashSet<Node>();

        
        // Создание узлов сетки для поиска пути
        var gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        var startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        var targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        var endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet,
            room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    /// <summary>
    /// Находит кратчайший путь - возвращает конечный узел, если путь найден, иначе возвращает null.
    /// </summary>
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // Добавить стартовый узел в открытый список
        openNodeList.Add(startNode);

        // Перебор открытого списка, пока он не станет пустым
        while (openNodeList.Count > 0)
        {
            // Сортировка списка
            openNodeList.Sort();

            // Текущий узел = узел с наименьшей fCost в открытом списке
            var currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // Если текущий узел = целевой узел, то завершить
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            // Добавить текущий узел в закрытый список
            closedNodeHashSet.Add(currentNode);

            // Оценить fCost для каждого соседа текущего узла
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet,
                instantiatedRoom);
        }

        return null;
    }

    /// <summary>
    /// Создаёт Stack<Vector3>, содержащий путь движения.
    /// </summary>
    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        var movementPathStack = new Stack<Vector3>();

        var nextNode = targetNode;

        // Получить середину ячейки
        var cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // Преобразовать позицию сетки в мировую позицию
            var worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(
                nextNode.gridPosition.x + room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y, 0));

            // Установить мировую позицию в середину ячейки сетки
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    /// <summary>
    /// Оценить соседние узлы.
    /// </summary>
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes,
        List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        var currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // Перебор всех направлений
        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j,
                    gridNodes, closedNodeHashSet, instantiatedRoom);

                if (validNeighbourNode != null)
                {
                    // Вычислить новый gCost для соседа
                    int newCostToNeighbour;

                    // Получить штраф за движение
                    // Непроходимые пути имеют значение 0. Штраф за движение по умолчанию задаётся в
                    // Settings и применяется к другим ячейкам сетки.
                    var movementPenaltyForGridSpace =
                        instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x,
                            validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) +
                                         movementPenaltyForGridSpace;

                    var isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Возвращает расстояние между nodeA и nodeB.
    /// </summary>
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        var dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        var dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return
                14 * dstY + 10 *
                (dstX - dstY); // 10 используется вместо 1, а 14 - это приближение Пифагора SQRT(10*10 + 10*10), чтобы избежать использования float
        return 14 * dstX + 10 * (dstY - dstX);
    }

    /// <summary>
    /// Оценить соседний узел по позициям neighbourNodeXPosition и neighbourNodeYPosition, используя
    /// указанные gridNodes, closedNodeHashSet и instantiatedRoom. Возвращает null, если узел недействителен.
    /// </summary>
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition,
        GridNodes gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // Если позиция соседнего узла выходит за пределы сетки, вернуть null
        if (neighbourNodeXPosition >=
            instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x ||
            neighbourNodeXPosition < 0 ||
            neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y -
            instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        // Получить соседний узел
        var neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // Проверить наличие препятствия в этой позиции
        var movementPenaltyForGridSpace =
            instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        // Проверить наличие перемещаемого препятствия в этой позиции
        //int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];

        // Если сосед является препятствием или находится в закрытом списке, пропустить
        if (movementPenaltyForGridSpace == 0 || /* itemObstacleForGridSpace == 0  || */
            closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
}