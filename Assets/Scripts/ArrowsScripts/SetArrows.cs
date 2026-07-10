using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SetArrows : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnParent;
	private Transform arrowContainer;

    private Dictionary<int, Dictionary<int, Vector3>> locations;
    private HashSet<Vector3> occupiedPositions;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	
	private int counter = 0;
	private Vector3 firstBlockPos;
	private int _directionIndex = 0;

    void Start()
    {
        locations = GridManager.Instance.locations;
        occupiedPositions = GridManager.Instance.occupiedPositions;
		firstArrowBlock = GridManager.Instance.firstArrowBlock;
    }

    private void PlaceArrows()
    {	
        if (arrowContainer.childCount == 0)
            return;

        Transform lastChild = arrowContainer.GetChild(arrowContainer.childCount - 1);

        int blockCount = Random.Range(0, 20);

        GetGridPosition(lastChild.localPosition, out int currentColumn, out int currentRow);
		
		GetFirstDirection(out int colStep, out int rowStep);

        for (int i = 0; i < blockCount; i++)
        {
			//Debug.Log("ColStep: " + colStep);
			//Debug.Log("RowStep: " + rowStep);
			
			Vector3 targetPosition = ReturnTargetLocation(
				ref currentColumn, 
				ref currentRow,
				ref colStep,
				ref rowStep,
				out int targetColStep,
				out int targetRowStep);
			
			if(targetPosition == Vector3.zero)
			{
				return;
			}
			
            SpawnBlock(targetPosition);
			
			RotateBlock(targetColStep, targetRowStep);
        }
    }

    private void GetGridPosition(Vector3 position, out int column, out int row)
    {
        var matchedPair = locations.FirstOrDefault(dict => dict.Value.ContainsValue(position));

        column = matchedPair.Key;
        row = matchedPair.Value.FirstOrDefault(inner => inner.Value == position).Key;
    }

    private bool IsValidAndEmpty(int col, int row)
    {
        if (!locations.ContainsKey(col))
            return false;

        if (!locations[col].ContainsKey(row))
            return false;
		
		if (firstArrowBlock.TryGetValue(firstBlockPos, out List<int> values) && values.Count >= 3)
		{
			int firstBlockCol = values[0];
			int firstBlockRow = values[1];
			int angle = values[2];
			
			if ((firstBlockCol == col && ((firstBlockRow < row && angle == 270) || (firstBlockRow > row && angle == 90))) ||
				(firstBlockRow == row && ((firstBlockCol < col && angle == 0)   || (firstBlockCol > col && angle == 180)))) {
				return false;
			}
		}

        return !occupiedPositions.Contains(locations[col][row]);
    }
	
	private void GetFirstDirection(out int colStep, out int rowStep)
	{
		int[] dCol = { 0,  0, -1, 1 };
		int[] dRow = { 1, -1,  0, 0 };
		
		int randomIndex = Random.Range(0, 4);

		colStep = dCol[randomIndex];
		rowStep = dRow[randomIndex];
	}

    private void TryGetNextDirection(out int neighborCol, out int neighborRow)
    {
        int[] dCol = { 0, 0, -1, 1 };
		int[] dRow = { 1, -1, 0, 0 };

		if (_directionIndex >= dCol.Length)
		{
			_directionIndex = 0;
		}

		neighborCol = dCol[_directionIndex];
		neighborRow = dRow[_directionIndex];
		
		_directionIndex++;
    }
	
	private Vector3 ReturnTargetLocation(
		ref int currentColumn, 
		ref int currentRow,
		ref int colStep,
		ref int rowStep,
		out int targetColStep,
		out int targetRowStep)
	{
		int nextColumn = currentColumn + colStep;
        int nextRow = currentRow + rowStep;
		
		targetColStep = 0;
		targetRowStep = 0;
		
		Vector3 targetPosition = Vector3.zero;
		
		if(IsValidAndEmpty(nextColumn, nextRow))
		{	
			currentColumn = nextColumn;
			currentRow = nextRow;
			
			targetPosition = locations[nextColumn][nextRow];
			
			targetColStep = colStep;
			targetRowStep = rowStep;
			
			return targetPosition;
		}
		
		for (int i = 0; i < 4; i++)
        {
			TryGetNextDirection(out int neighborCol, out int neighborRow);
			
            nextColumn = currentColumn + neighborCol;
            nextRow = currentRow + neighborRow;
			
			if (IsValidAndEmpty(nextColumn, nextRow))
            {	
				currentColumn = nextColumn;
				currentRow = nextRow;
				
                targetPosition = locations[nextColumn][nextRow];
				
				targetColStep = neighborCol;
				targetRowStep = neighborRow;
				
				colStep = neighborCol;
				rowStep = neighborRow;
				
				return targetPosition;
			}
		}
		
		return targetPosition;
	}

    private void SpawnBlock(Vector3 position)
    {
        occupiedPositions.Add(position);

        GameObject spawnedObj = Instantiate(prefabToSpawn, arrowContainer);
        spawnedObj.transform.localPosition = position;
    }
	
	private void ReturnArrowContainer()
	{	
		arrowContainer = spawnParent.GetChild(counter);
		
		counter += 1;
	}
	
	private void RotateBlock(int colStep, int rowStep)
	{
		Transform lastChild = arrowContainer.GetChild(arrowContainer.childCount - 1);
		Transform previousBlock = arrowContainer.GetChild(arrowContainer.childCount - 2);
		
		if(arrowContainer.childCount == 2)
		{
			int angle = 0;

			if (colStep != 0)
			{
				angle = colStep == 1 ? 270 : 90; // colStep one is down and 270 is up
			}
			else if (rowStep != 0)
			{
				angle = rowStep == 1 ? 0 : 180; // rowStep one is right and 0 is left
			}
			
			SaveFirstBlock(angle);
			
			previousBlock.localRotation = Quaternion.Euler(0, 0, angle);
		}
		
		if(lastChild.localPosition.y != previousBlock.localPosition.y)
		{
			lastChild.localRotation = Quaternion.Euler(0, 0, 90);
		}
		
		if(lastChild.localPosition.x != previousBlock.localPosition.x)
		{
			lastChild.localRotation = Quaternion.Euler(0, 0, 0);
		}
	}
	
	private void SaveFirstBlock(int angle)
	{
		Transform firstBlock = arrowContainer.GetChild(0);
		firstBlockPos = firstBlock.localPosition;
		
		GetGridPosition(firstBlockPos, out int currentColumn, out int currentRow);
		
		firstArrowBlock[firstBlockPos] = new List<int>();		
		firstArrowBlock[firstBlockPos].Add(currentColumn);
		firstArrowBlock[firstBlockPos].Add(currentRow);
		firstArrowBlock[firstBlockPos].Add(angle);
	}

    public void setArrowLength()
    {
		ReturnArrowContainer();
		
        for (int i = 0; i <= 4; i++)
        {	
            PlaceArrows();
        }
    }
}
