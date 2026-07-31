using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExitChecker : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private Dictionary<GameObject, List<GameObject>> arrowDict;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	
	//Infinite loop guard
	private int maxIterations = 100;
	private int currentIteration = 0;
	
    void Start()
	{
		locations = GridManager.Instance.locations;
		arrowDict = GridManager.Instance.arrowDict;
		firstArrowBlock = GridManager.Instance.firstArrowBlock;
	}
	
	private void GetTargetPos(Vector3 firstBlock, out Vector3 targetPos)
	{
		targetPos = Vector3.zero;
		
		if (!firstArrowBlock.TryGetValue(firstBlock, out List<int> values) || values.Count < 3)
		{
			return;
		}

		int row = values[0];
		int col = values[1];
		int angle = values[2];
		
		switch (angle)
		{
			case 270: // Up
				targetPos = locations[row - 1][col];
				break;

			case 90: // Down
				targetPos = locations[row + 1][col];
				break;

			case 0: // Left
				targetPos = locations[row][col - 1];
				break;

			case 180: // Right
				targetPos = locations[row][col + 1];
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
	
	public bool CheckExit()
	{	
		GameObject currentArrow = arrowDict.Last().Key;
		GameObject nextArrow = null;
		
		GameObject firstBlock = GetFirstBlock(currentArrow);
		GetTargetPos(firstBlock.transform.localPosition, out Vector3 targetPos);
		nextArrow = GetKeyByPosition(targetPos);

		while (nextArrow != null)
		{	
			if (++currentIteration > maxIterations)
			{
				Debug.LogError("Infinite loop detected! Aborting loop.");
				break;
			}
			
			//Debugging
			Debug.Log("Current Arrow: " + currentArrow);
			Debug.Log("Next Arrow: " + nextArrow);
			
			firstBlock = GetFirstBlock(nextArrow);

			GetTargetPos(firstBlock.transform.localPosition, out targetPos);

			nextArrow = GetKeyByPosition(targetPos);
			
			if(nextArrow == currentArrow)
			{
				//Debugging
				Debug.Log(nextArrow);
				
				RemoveObjects(currentArrow);
				return false;
			}
			
			if (nextArrow == null)
			{
				//Debugging
				Debug.Log(nextArrow);
				
				return true;
			}
		}
		return true;
	}
}
