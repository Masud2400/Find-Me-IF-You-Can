using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MoveArrow : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
    private HashSet<Vector3> occupiedPositions;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	private Dictionary<GameObject, List<GameObject>> arrowDict;
	
	[Header("Settings")]
	[SerializeField] private float movementSpeed = 300f;
	[SerializeField] private float offscreenOffset = 15f;
	
	private Vector3 targetPos = Vector3.zero;
	private bool isMoving = false;

    void Start()
    {
        locations = GridManager.Instance.locations;
        occupiedPositions = GridManager.Instance.occupiedPositions;
		firstArrowBlock = GridManager.Instance.firstArrowBlock;
		arrowDict = GridManager.Instance.arrowDict;
    }
	
	void Update()
	{
		if(isMoving)
		{
			float step = movementSpeed * Time.deltaTime;

			if(targetPos == Vector3.zero) return;
			
			List<GameObject> arrowList = FindListByLocalPosition(transform.localPosition);
			
			SetBlockMovement(arrowList, step);
		}
	}
	
	private void SetBlockMovement(List<GameObject> arrowList, float step)
	{
		GameObject head = arrowList[0];
		GameObject lastBlock = arrowList[arrowList.Count - 1];
		
		// This removes the positions to clear the way
		occupiedPositions.Remove(head.transform.localPosition);
		
		Vector3 previousPosition = head.transform.localPosition;
		Quaternion previousRotation = head.transform.localRotation;
		
		head.transform.localPosition = Vector3.MoveTowards(head.transform.localPosition, targetPos, step);

		for (int i = 1; i < arrowList.Count; i++)
		{
			GameObject currentBlock = arrowList[i];
			
			// This removes the positions to clear the way
			occupiedPositions.Remove(arrowList[i].transform.localPosition);
			
			Vector3 nextPreviousPos = currentBlock.transform.localPosition;
			Quaternion nextPreviousRotation = currentBlock.transform.localRotation;
			
			currentBlock.transform.localPosition = Vector3.MoveTowards(currentBlock.transform.localPosition, previousPosition, step);
			currentBlock.transform.localRotation = previousRotation;
			
			previousPosition = nextPreviousPos;
			previousRotation = nextPreviousRotation;
		}
		
		if (Vector3.Distance(lastBlock.transform.localPosition, targetPos) < 0.001f)
		{
			lastBlock.transform.localPosition = targetPos;
			isMoving = false;
		}
	}
	
	private List<GameObject> FindListByLocalPosition(Vector3 targetLocalPosition)
	{
		foreach (List<GameObject> arrowList in arrowDict.Values)
		{
			foreach (GameObject arrow in arrowList)
			{
				if (arrow != null && arrow.transform.localPosition == targetLocalPosition)
				{
					return arrowList; 
				}
			}
		}

		return null;
	}
	
	public void MoveBlocks()
	{
		if(isMoving) return;
		isMoving = true;
		
		Debug.Log("Button pressed");
		
		Vector3 currentPos = transform.localPosition;
		
		if (!firstArrowBlock.TryGetValue(currentPos, out List<int> values) || values.Count < 3)
			return;

		int row = values[0];
		int col = values[1];
		int angle = values[2];
		
		int finalRow = locations.Count - 1;
		int finalCol = locations.Last().Value.Last().Key; 

		switch (angle)
		{
			case 270: // Up
				for (int i = row - 1; i >= 0; i--)
				{
					if (occupiedPositions.Contains(locations[i][col]))
						return;
				}
				targetPos = locations[0][col] + new Vector3(0, offscreenOffset, 0);
				break;

			case 90: // Down
				for (int i = row + 1; i <= finalRow; i++)
				{
					if (occupiedPositions.Contains(locations[i][col]))
						return;
				}
				targetPos = locations[finalRow][col] + new Vector3(0, -offscreenOffset, 0);
				break;

			case 0: // Left
				for (int i = col - 1; i >= 0; i--)
				{
					if (occupiedPositions.Contains(locations[row][i]))
						return;
				}
				targetPos = locations[row][0] + new Vector3(-offscreenOffset, 0, 0);
				break;

			case 180: // Right
				for (int i = col + 1; i <= finalCol; i++)
				{
					if (occupiedPositions.Contains(locations[row][i]))
						return;
				}
				targetPos = locations[row][finalCol] + new Vector3(offscreenOffset, 0, 0);
				break;
		}
	}
}
