using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SetArrows : MonoBehaviour
{
    private GameObject prefabToSpawn;
    private Transform spawnParent;
	private Data gameData;
	
	private List<VectorData> arrowContainer;
	private string arrowContainerKey;

    private Dictionary<Vector2Int, GridCell> locations;
    private HashSet<Vector3> occupiedPositions;
	private Dictionary<Vector3, FirstBlock> firstArrowBlock;
	private Dictionary<string, List<VectorData>> arrowDict;
	
	//private int counter = 0;
	private Vector3 firstBlockPos;
	private int _directionIndex = 0;

    void Start()
    {
        locations = gameData.locations;
        occupiedPositions = gameData.occupiedPositions;
		firstArrowBlock = gameData.firstArrowBlock;
		arrowDict = gameData.arrowDict;
		
		prefabToSpawn = AssetManager.Instance.PrefabToSpawn;
		spawnParent = AssetManager.Instance.SpawnParent;
		gameData = AssetManager.Instance.GameData;
    }

    private void PlaceArrows()
    {	
        if (arrowContainer.Count == 0)
            return;

        Vector3 lastChild = arrowContainer[^1].position; // This is the last block

        int blockCount = Random.Range(0, 20);

        //GetGridPosition(lastChild, out int currentRow, out int currentColumn);
		Vector2Int index = GetIndex.GetGridIndex(lastChild);
		int currentRow = index.x;
		int currentColumn = index.y;
		
		GetFirstDirection(out int rowStep, out int colStep);

        for (int i = 0; i < blockCount; i++)
        {
			Vector3 targetPosition = ReturnTargetLocation(
				ref currentRow,
				ref currentColumn,
				ref rowStep,
				ref colStep,
				out int targetRowStep,
				out int targetColStep);
			
			if(targetPosition == Vector3.zero) // Possible bug
			{
				return;
			}
			
            SpawnBlock(targetPosition);
			
			RotateBlock(targetRowStep, targetColStep);
        }
    }
	/*
    private void GetGridPosition(Vector3 position, out int row, out int column)
    {
        var matchedPair = locations.FirstOrDefault(dict => dict.Value.ContainsValue(position));

        row = matchedPair.Key;
        column = matchedPair.Value.FirstOrDefault(inner => inner.Value == position).Key;
    }*/

    private bool IsValidAndEmpty(int row, int col)
    {
		Vector2Int index = new Vector2Int(row, col);
		
        if (!locations.ContainsKey(index))
			return false;
		
		if (firstArrowBlock.TryGetValue(firstBlockPos, out List<int> values) && values.Count >= 3)
		{
			int firstBlockRow = values[0];
			int firstBlockCol = values[1];
			int angle = values[2];
			
			if ((firstBlockCol == col && ((firstBlockRow > row && angle == 270) || (firstBlockRow < row && angle == 90))) ||
				(firstBlockRow == row && ((firstBlockCol < col && angle == 180) || (firstBlockCol > col && angle == 0)))) {
				
				/*
				Debug.Log("FirstBlockCol: " + firstBlockCol);
				Debug.Log("Col: " + col);
				Debug.Log("FirstBlockRow: " + firstBlockRow);
				Debug.Log("Row: " + row);
				Debug.Log("Angle: " + angle);
				*/
				
				return false;
			}
		}

        return !occupiedPositions.Contains(locations[index].position);
    }
	
	private void GetFirstDirection(out int rowStep, out int colStep)
	{
		int[] dRow = { 0,  0, -1, 1 };
		int[] dCol = { 1, -1,  0, 0 };
		
		int randomIndex = Random.Range(0, 4);

		rowStep = dRow[randomIndex];
		colStep = dCol[randomIndex];
	}

    private void TryGetNextDirection(out int neighborRow, out int neighborCol)
    {
        int[] dRow = { 0,  0, -1, 1 };
		int[] dCol = { 1, -1,  0, 0 };

		if (_directionIndex >= dRow.Length)
		{
			_directionIndex = 0;
		}

		neighborRow = dRow[_directionIndex];
		neighborCol = dCol[_directionIndex];
		
		_directionIndex++;
    }
	
	private Vector3 ReturnTargetLocation(
		ref int currentRow,
		ref int currentColumn,
		ref int rowStep,		
		ref int colStep,
		out int targetRowStep,
		out int targetColStep)
	{
		int nextRow = currentRow + rowStep;
		int nextColumn = currentColumn + colStep;
		
		Vector2Int index = new Vector2Int(nextRow, nextColumn);
		
		targetRowStep = 0;
		targetColStep = 0;
		
		Vector3 targetPosition = Vector3.zero;
		
		if(IsValidAndEmpty(nextRow, nextColumn))
		{	
			currentRow = nextRow;
			currentColumn = nextColumn;
			
			targetPosition = locations[index].position;
			
			targetRowStep = rowStep;
			targetColStep = colStep;
			
			return targetPosition;
		}
		
		for (int i = 0; i < 4; i++)
        {
			TryGetNextDirection(out int neighborRow, out int neighborCol);
			
			nextRow = currentRow + neighborRow;
            nextColumn = currentColumn + neighborCol;
			
			index = new Vector2Int(nextRow, nextColumn);
			
			if (IsValidAndEmpty(nextRow, nextColumn))
            {	
				currentRow = nextRow;
				currentColumn = nextColumn;
				
                targetPosition = locations[index].position;
				
				targetRowStep = neighborRow;
				targetColStep = neighborCol;
				
				rowStep = neighborRow;
				colStep = neighborCol;
				
				return targetPosition;
			}
		}
		
		return targetPosition;
	}

    private void SpawnBlock(Vector3 position)
    {
        occupiedPositions.Add(position);
		
		arrowDict[arrowContainerKey].Add(new VectorData { position = position }); // Adding all the other arrow blocks
    }
	
	private void ReturnArrowContainer()
	{	
		arrowContainerKey = arrowDict.Keys.Last(); // Getting the key for the arrow
		arrowContainer = arrowDict[arrowContainerKey]; // Getting the right arrow container
	}
	
	private void RotateBlock(int rowStep, int colStep)
	{
		Quaternion lastChild = arrowContainer[^1].rotation; // This is the last block all the time
		Quaternion previousBlock = arrowContainer[arrowContainer.Count - 2].rotation; // The block before the last block
		
		if(arrowContainer.Count == 2)
		{
			int angle = 0;

			if (rowStep != 0)
			{
				angle = rowStep == 1 ? 270 : 90; // rowStep one is down and 270 is up
			}
			else if (colStep != 0)
			{
				angle = colStep == 1 ? 0 : 180; // colStep one is right and 0 is left
			}
			
			//SaveFirstBlock(angle);
			
			previousBlock = Quaternion.Euler(0, 0, angle);
		}
		
		if(lastChild.y != previousBlock.y)
		{
			lastChild = Quaternion.Euler(0, 0, 90);
		}
		
		if(lastChild.x != previousBlock.x)
		{
			lastChild = Quaternion.Euler(0, 0, 0);
		}
	}
	/*
	private void SaveFirstBlock(int angle)
	{
		firstBlockPos = arrowContainer[0].position; // the first block of the arrow
		
		//GetGridPosition(firstBlockPos, out int currentRow, out int currentColumn);
		Vector2Int index = GetIndex.GetGridIndex(firstBlockPos);
		
		firstArrowBlock[firstBlockPos] = new List<int>();		
		firstArrowBlock[firstBlockPos].Add(index.x);
		firstArrowBlock[firstBlockPos].Add(index.y);
		firstArrowBlock[firstBlockPos].Add(angle);
	}*/

    public void setArrowLength()
    {
		ReturnArrowContainer();
		
		for (int i = 0; i <= 4; i++)
		{	
			PlaceArrows();
		}
    }
}
