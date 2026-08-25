using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MoveArrow : MonoBehaviour
{
	private Data gameData;

	private Dictionary<Vector2Int, GridCell> locations;
	private HashSet<Vector3> occupiedPositions;
	private Dictionary<string, List<VectorData>> arrowDict;
	private Dictionary<Vector3, FirstBlock> firstArrowBlock;
	public Dictionary<Transform, List<GameObject>> gameObjectReference;
	
	[Header("Settings")]
	[SerializeField] private float movementSpeed = 300f;
	[SerializeField] private float offscreenOffset = 15f;
	
	private Vector3 targetPos = Vector3.zero;
	private bool isMoving = false;

    void Start()
    {
        gameData = AssetManager.Instance.GameData;
		
		locations = gameData.locations;
		arrowDict = gameData.arrowDict;
		occupiedPositions = gameData.occupiedPositions;
		firstArrowBlock = gameData.firstArrowBlock;
		gameObjectReference = gameData.gameObjectReference;
    }
	
	/*
	void Update()
	{	
		if(isMoving)
		{	
			float step = movementSpeed * Time.deltaTime;

			if(targetPos == Vector3.zero) return; //Possible bug
			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, step);
			
			if (Vector3.Distance(transform.localPosition, targetPos) < 0.001f)
			{
				transform.localPosition = targetPos;
				isMoving = false;
			}
		}
	}*/
	
	//Debugging
	private void DestroyObjects()
	{
		var entry = gameObjectReference
			.FirstOrDefault(x => x.Value.Any(obj =>
				obj != null &&
				obj.transform.localPosition == transform.localPosition));

		if (entry.Key == null)
			return;

		foreach (var obj in entry.Value)
		{
			if (obj != null)
				Destroy(obj);
		}

		gameObjectReference.Remove(entry.Key);
		
		string foundKey = arrowDict
			.FirstOrDefault(x => x.Value.Any(v => v.position == transform.localPosition))
			.Key;
		
		foreach(var loc in arrowDict[foundKey])
		{
			occupiedPositions.Remove(loc.position);
		}
	}
	
	public void MoveBlocks()
	{	
		//Debugging
		//Debug.Log("Arrow clicked");
		
		//if(isMoving) return;
		//isMoving = true;
		
		Vector3 previousPos = targetPos;
		
		Vector3 currentPos = transform.localPosition;
		
		if (!firstArrowBlock.TryGetValue(currentPos, out FirstBlock values))
			return;

		int row = values.row;
		int col = values.col;
		int angle = values.angle;
		
		int finalRow = locations.Last().Key.x;
		int finalCol = locations.Last().Key.y;

		Vector2Int index;
		Vector3 position = Vector3.zero;

		switch (angle)
		{
			case 270: // Up
				for (int i = row - 1; i >= 0; i--)
				{
					index = new Vector2Int(i, col);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						return;
				}
				targetPos = position + new Vector3(0, offscreenOffset, 0);
				break;

			case 90: // Down
				for (int i = row + 1; i <= finalRow; i++)
				{
					index = new Vector2Int(i, col);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						return;
				}
				targetPos = position + new Vector3(0, -offscreenOffset, 0);
				break;

			case 0: // Left
				for (int i = col - 1; i >= 0; i--)
				{
					index = new Vector2Int(row, i);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						return;
				}
				targetPos = position + new Vector3(-offscreenOffset, 0, 0);
				break;

			case 180: // Right
				for (int i = col + 1; i <= finalCol; i++)
				{
					index = new Vector2Int(row, i);
					position = locations[index].position;
					if (occupiedPositions.Contains(position))
						return;
				}
				targetPos = position + new Vector3(offscreenOffset, 0, 0);
				break;
		}
		
		if(previousPos == targetPos)
		{
			Debug.Log("No Change");
			return;
		}
		
		DestroyObjects();
	}
}
