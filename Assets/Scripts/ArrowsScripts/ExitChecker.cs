using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExitChecker : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private Dictionary<string, List<BlockData>> arrowDict;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	private HashSet<Vector3> occupiedPositions;
	
	private Dictionary<string, HashSet<string>> arrowConnections = new Dictionary<string, HashSet<string>>();
	
    void Start()
	{
		locations = GridManager.Instance.locations;
		arrowDict = GridManager.Instance.arrowDict;
		firstArrowBlock = GridManager.Instance.firstArrowBlock;
		occupiedPositions = GridManager.Instance.occupiedPositions;
	}
	
	private void GetFirstBlockData(
		Vector3 firstBlock,
		out int row, 
		out int col, 
		out int angle)
	{
		row = -1;
		col = -1;
		angle = -1;
		
		if (!firstArrowBlock.TryGetValue(firstBlock, out List<int> values) || values.Count < 3)
			return;

		row = values[0];
		col = values[1];
		angle = values[2];
	}
	
	private void GetTargetPos(Vector3 firstBlock, out HashSet<Vector3> targetPositions)
	{	
		targetPositions = new HashSet<Vector3>();
		
		GetFirstBlockData(firstBlock, out int row, out int col, out int angle);
		
		int finalRow = locations.Count - 1;
		int finalCol = locations.Last().Value.Last().Key;
		
		switch (angle)
		{
			case 270: // Up
				for (int i = row - 1; i >= 0; i--)
				{
					if (occupiedPositions.Contains(locations[i][col]))
						targetPositions.Add(locations[i][col]);
				}
				break;

			case 90: // Down
				for (int i = row + 1; i <= finalRow; i++)
				{
					if (occupiedPositions.Contains(locations[i][col]))
						targetPositions.Add(locations[i][col]);
				}
				break;

			case 0: // Left
				for (int i = col - 1; i >= 0; i--)
				{
					if (occupiedPositions.Contains(locations[row][i]))
						targetPositions.Add(locations[row][i]);
				}
				break;

			case 180: // Right
				for (int i = col + 1; i <= finalCol; i++)
				{
					if (occupiedPositions.Contains(locations[row][i]))
						targetPositions.Add(locations[row][i]);
				}
				break;
		}
	}
	
	private string GetKeyByPosition(Vector3 targetPos)
	{
		foreach (var pair in arrowDict)
		{
			foreach (BlockData obj in pair.Value)
			{
				if ((obj.position - targetPos).sqrMagnitude < 0.0001f) 
				{
					return pair.Key;
				}
			}
		}
		return null;
	}
	
	private BlockData GetFirstBlock(string parentArrow)
	{
		return arrowDict[parentArrow][0];
	}
	
	private void RemoveObjects(string currentArrow)
	{
		if (arrowDict.TryGetValue(currentArrow, out List<BlockData> children))
		{
			// Destroy all children
			foreach (BlockData child in children)
			{
				if (child != null)
				{
					occupiedPositions.Remove(child.position);
				}
			}

			// Clean up the dictionary
			arrowDict.Remove(currentArrow);
		}
	}
	
	private void SaveAllConnections()
	{
		foreach(var kvp in arrowDict)
		{
			var key = kvp.Key;
			
			BlockData firstBlock = GetFirstBlock(key);
			
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
						RemoveObjects(startNode);
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
	
	public bool CheckExit(out string currentArrow)
	{
		currentArrow = arrowDict.Last().Key;
		
		SaveAllConnections();
		
		bool detectCycle = DetectCycleBFS(currentArrow);
		
		if(detectCycle)
			return true;
		
		return false;
	}
}
