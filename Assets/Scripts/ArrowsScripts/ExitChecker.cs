using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExitChecker : MonoBehaviour
{
	private Data gameData;

	private Dictionary<Vector2Int, GridCell> locations;
	private Dictionary<string, List<VectorData>> arrowDict;
	private HashSet<Vector3> occupiedPositions;
	private Dictionary<Vector3, FirstBlock> firstArrowBlock;
	
	private Dictionary<string, HashSet<string>> arrowConnections = new Dictionary<string, HashSet<string>>();
	
    void Start()
	{
		gameData = AssetManager.Instance.GameData;
		
		locations = gameData.locations;
		arrowDict = gameData.arrowDict;
		occupiedPositions = gameData.occupiedPositions;
		firstArrowBlock = gameData.firstArrowBlock;
	}
	
	private void GetFirstBlockData(
		Vector3 position,
		out int row,
		out int col,
		out int angle
	)
	{
		row = -1;
		col = -1;
		angle = -1;
		
		if(firstArrowBlock.TryGetValue(position, out FirstBlock block))
		{
			row = block.row;
			col = block.col;
			angle = block.angle;
		}
	}
	
	private void GetTargetPos(Vector3 firstBlock, out HashSet<Vector3> targetPositions)
	{	
		targetPositions = new HashSet<Vector3>();
		
		GetFirstBlockData(firstBlock, out int row, out int col, out int angle);
		
		int finalRow = locations.Last().Key.x;
		int finalCol = locations.Last().Key.y;
		
		Vector2Int index;
		Vector3 position;
		
		switch (angle)
		{
			case 270: // Up
				for (int i = row - 1; i >= 0; i--)
				{
					index = new Vector2Int(i, col);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						targetPositions.Add(position);
				}
				break;

			case 90: // Down
				for (int i = row + 1; i <= finalRow; i++)
				{
					index = new Vector2Int(i, col);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						targetPositions.Add(position);
				}
				break;

			case 0: // Left
				for (int i = col - 1; i >= 0; i--)
				{
					index = new Vector2Int(row, i);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						targetPositions.Add(position);
				}
				break;

			case 180: // Right
				for (int i = col + 1; i <= finalCol; i++)
				{
					index = new Vector2Int(row, i);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						targetPositions.Add(position);
				}
				break;
		}
	}
	
	private string GetKeyByPosition(Vector3 targetPos)
	{
		foreach (var pair in arrowDict)
		{
			foreach (VectorData obj in pair.Value)
			{
				if ((obj.position - targetPos).sqrMagnitude < 0.0001f) 
				{
					return pair.Key;
				}
			}
		}
		return null;
	}
	
	private VectorData GetFirstBlock(string parentArrow)
	{
		return arrowDict[parentArrow][0];
	}
	
	private void SaveAllConnections()
	{
		foreach(var kvp in arrowDict)
		{
			var key = kvp.Key;
			
			VectorData firstBlock = GetFirstBlock(key);
			
			GetTargetPos(firstBlock.position, out HashSet<Vector3> targetPositions);
			
			if (!arrowConnections.ContainsKey(key))
			{
				arrowConnections[key] = new HashSet<string>();
			}
			
			foreach(var pos in targetPositions)
			{
				var target = GetKeyByPosition(pos);
				arrowConnections[key].Add(target);
			}
		}
	}
	
	private bool DetectCycleBFS(string startNode)
	{	
		Queue<string> toVisit = new Queue<string>();
		HashSet<string> visited = new HashSet<string>();

		toVisit.Enqueue(startNode);
		visited.Add(startNode);

		while (toVisit.Count > 0)
		{	
			string current = toVisit.Dequeue();

			if (arrowConnections.TryGetValue(current, out var neighbors))
			{
				foreach (string neighbor in neighbors)
				{
					// Found a connection pointing back to start
					if (neighbor == startNode)
					{
						return true;
					}

					if (!visited.Contains(neighbor))
					{
						visited.Add(neighbor);
						toVisit.Enqueue(neighbor);
					}
				}
			}
		}

		return false;
	}
	
	public void CheckExit()
	{
		int[] angles = { 270, 90, 0, 180 };
		string currentArrow = arrowDict.Last().Key;
		
		SaveAllConnections();
		
		bool detectCycle = DetectCycleBFS(currentArrow);
		
		if (!detectCycle)
			return;

		VectorData vectorData = GetFirstBlock(currentArrow);
		
		foreach (int angle in angles)
		{
			if(arrowConnections.ContainsKey(currentArrow))
				arrowConnections[currentArrow].Clear();
			
			vectorData.rotation = Quaternion.Euler(0, 0, angle);
			
			if (firstArrowBlock.TryGetValue(vectorData.position, out FirstBlock block))
				block.angle = angle;

			SaveAllConnections();

			detectCycle = DetectCycleBFS(currentArrow);

			if (!detectCycle)
				return;
		}
	}
}
