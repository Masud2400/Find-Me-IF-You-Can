using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExitChecker : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private Dictionary<GameObject, List<GameObject>> arrowDict;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	
	private List<GameObject> arrowCycle = new List<GameObject>();
	
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
			/*
			//Debugging
			Debug.Log("The firstBloc: " + firstBlock);
			
			foreach (var pair in arrowDict)
			{
				Debug.Log($"Key: {pair.Key.name}");

				foreach (var obj in pair.Value)
				{
					Debug.Log($"  {obj.name} - {obj.transform.localPosition}");
				}
			}
			
			foreach (var key in firstArrowBlock.Keys)
			{
				Debug.Log($"Key: {key}");
			}*/
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
		arrowCycle.Clear();

		GameObject currentArrow = arrowDict.Last().Key;
		
		/*
		//Debugging
		Debug.Log($"Starting from {currentArrow.name}");
		*/

		while (currentArrow != null)
		{	
			/*
			//Debugging
			Debug.Log($"Current: {currentArrow.name} ({currentArrow.GetInstanceID()})");
			*/
			
			if (arrowCycle.Contains(currentArrow))
			{
				RemoveObjects(currentArrow);
				return false;
			}

			arrowCycle.Add(currentArrow);

			GameObject firstBlock = GetFirstBlock(currentArrow);

			GetTargetPos(firstBlock.transform.localPosition, out Vector3 targetPos);

			currentArrow = GetKeyByPosition(targetPos);
			
			/*
			// Debugging
			Debug.Log(firstBlock.transform.localPosition);
			Debug.Log(targetPos);*/
			
			/*
			// Debugging
			Debug.Log(currentArrow == null
				? "Next: null"
				: $"Next: {currentArrow.name} ({currentArrow.GetInstanceID()})");
			*/
			
			if (currentArrow == null)
				return true;
		}
		return true;
	}
}
