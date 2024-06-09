using HarfBuzzSharp;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class BfsTask
{
    public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
    {
        var queue = new Queue<SinglyLinkedList<Point>>();
        queue.Enqueue(new SinglyLinkedList<Point>(start));
        HashSet<Point> chestsLocation = chests.Select(chest => chest.Location).ToHashSet();
        HashSet<Point> visited = new()
        {
           start
        };
        while (queue.Count != 0)
        {
            var currentPosition = queue.Dequeue();
            if (chestsLocation.Contains(currentPosition.Value))
            {
                yield return currentPosition;
            }
            Walk(currentPosition, queue, visited, map);
        }
        yield break;
    }

    static void Walk (SinglyLinkedList<Point> currentPosition, Queue<SinglyLinkedList<Point>> queue, HashSet<Point> visited, Map map)
    {
        Walker walker = new(currentPosition.Value);

        foreach (var direction in Walker.PossibleDirections)
        {
            Walker pathFinder = walker.WalkInDirection(map, Walker.ConvertOffsetToDirection(direction));
            if (pathFinder.PointOfCollision == Point.Null && !visited.Contains(pathFinder.Position))
            {
                queue.Enqueue(new SinglyLinkedList<Point>(pathFinder.Position, currentPosition));
                visited.Add(pathFinder.Position);
            }
        }
    }
}