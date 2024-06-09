using Avalonia.Interactivity;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class DungeonTask
{
	public static MoveDirection[] FindShortestPath(Map map)
	{        
        Chest exit = new (map.Exit, 0);
        var fromStartToExit = BfsTask.FindPaths(map, map.InitialPosition, new[] { exit }).FirstOrDefault();
        if(fromStartToExit == null)
        {
            return new MoveDirection[0];
        }

        if (map.Chests.Length == 0)
        {
            return ConvertPointsToSteps(fromStartToExit);
        }

        Dictionary<Point, Chest> chests = map.Chests.Select(chest => chest).ToDictionary(k => k.Location);

        var fromStartToChest = BfsTask.FindPaths(map, map.InitialPosition, map.Chests);
        var fromChestToExit = BfsTask.FindPaths(map, map.Exit, map.Chests);

        var fromStartToChestToExit = fromStartToChest
            .Join(fromChestToExit, first => first.Value, second => second.Value, (first, second) => new
            {
                StartChest = first, ChestExit = second, Length = first.Length + second.Length -1
            }).ToList();

        if(fromStartToChestToExit.Count == 0)
        {
            return ConvertPointsToSteps(fromStartToExit);
        }
        var bestRoute = fromStartToChestToExit
            .OrderBy(item => item.Length)
            .ThenByDescending(item => chests[item.ChestExit.Value].Value)
            .First();

        MoveDirection[] result = ConvertPointsToSteps(MakePath(bestRoute.StartChest, bestRoute.ChestExit));
        return result;
    }

    private static MoveDirection[] ConvertPointsToSteps(SinglyLinkedList<Point> route)
    {
        SinglyLinkedList<Point> currentPosition = route;
        route = route.Previous;
        List<MoveDirection> directions = new ();
        while (route != null)
        {
            directions.Add(Walker.ConvertOffsetToDirection(currentPosition.Value - route.Value));
            currentPosition = route;
            route = route.Previous;
        }

        directions.Reverse();
        return directions.ToArray();
    }

	private static SinglyLinkedList<Point> MakePath(SinglyLinkedList<Point> toChest, SinglyLinkedList<Point> toExit)
	{
		SinglyLinkedList<Point> result = toChest;
		while(toExit.Previous != null)
		{
			toExit = toExit.Previous;
			result = new(toExit.Value, result);
		}
		return result;
	}
}