using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExitChecker : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private Dictionary<GameObject, List<GameObject>> arrowDict;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	private HashSet<Vector3> occupiedPositions;
	
	private Dictionary<GameObject, HashSet<GameObject>> arrowConnections = new Dictionary<GameObject, HashSet<GameObject>>();
	
	//Infinite loop guard
	private int maxIterations = 1000;
	private int currentIteration = 0;
	
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
	
	private GameObject GetKeyByPosition(Vector3 targetPos)
	{
		foreach (var pair in arrowDict)
		{
			foreach (GameObject obj in pair.Value)
			{
				if (obj != null && obj.transform.localPosition == targetPos)
				{
					return pair.Key;
				}
			}
		}
		return null;
	}
	
	private GameObject GetFirstBlock(GameObject parentArrow)
	{
		return arrowDict[parentArrow][0];
	}
	
	private void RemoveObjects(GameObject currentArrow)
	{
		if (arrowDict.TryGetValue(currentArrow, out List<GameObject> children))
		{
			// Destroy all children
			foreach (GameObject child in children)
			{
				if (child != null)
				{
					Destroy(child);
				}
			}

			// Destroy the parent
			if (currentArrow != null)
			{
				Destroy(currentArrow);
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
			
			GameObject firstBlock = GetFirstBlock(key);
			
			GetTargetPos(firstBlock.transform.localPosition, out HashSet<Vector3> targetPositions);
			
			if (!arrowConnections.ContainsKey(key))
			{
				arrowConnections[key] = new HashSet<GameObject>();
			}
			
			foreach(var pos in targetPositions)
			{
				var target = GetKeyByPosition(pos);
				arrowConnections[key].Add(target);
			}
		}
	}
	
	private bool DetectCycleBFS(GameObject startNode)
	{	
		Queue<GameObject> toVisit = new Queue<GameObject>();
		HashSet<GameObject> visited = new HashSet<GameObject>();

		toVisit.Enqueue(startNode);
		visited.Add(startNode);

		while (toVisit.Count > 0)
		{
			if (++currentIteration > maxIterations)
			{
				Debug.LogError("Infinite loop detected! Aborting loop.");
				break;
			}
			
			GameObject current = toVisit.Dequeue();

			if (arrowConnections.TryGetValue(current, out var neighbors))
			{
				foreach (GameObject neighbor in neighbors)
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
	
	public bool CheckExit()
	{
		GameObject currentArrow = arrowDict.Last().Key;
		
		SaveAllConnections();
		
		bool detectCycle = DetectCycleBFS(currentArrow);
		
		if(detectCycle)
			return true;
		
		return false;
	}
}
