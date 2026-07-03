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
	
	private int counter = 0;

    void Start()
    {
        locations = GridManager.Instance.locations;
        occupiedPositions = GridManager.Instance.occupiedPositions;
    }

    private void PlaceArrows()
    {	
        if (arrowContainer.childCount == 0)
            return;

        Transform lastChild = arrowContainer.GetChild(arrowContainer.childCount - 1);

        Vector3 randomDirection = GetRandomDirection();
        int blockCount = Random.Range(0, 20);

        GetGridPosition(lastChild.localPosition, out int currentColumn, out int currentRow);
        GetDirectionStep(randomDirection, out int colStep, out int rowStep);

        for (int i = 0; i < blockCount; i++)
        {
            if (!TryGetNextPosition(
                    ref currentColumn,
                    ref currentRow,
                    colStep,
                    rowStep,
                    out Vector3 targetPosition))
                return;

            SpawnBlock(targetPosition);
			
			RotateBlock(colStep, rowStep);
        }
    }

    private Vector3 GetRandomDirection()
    {
        Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        return directions[Random.Range(0, directions.Length)];
    }

    private void GetGridPosition(Vector3 position, out int column, out int row)
    {
        var matchedPair = locations.FirstOrDefault(dict => dict.Value.ContainsValue(position));

        column = matchedPair.Key;
        row = matchedPair.Value.FirstOrDefault(inner => inner.Value == position).Key;
    }

    private void GetDirectionStep(Vector3 direction, out int colStep, out int rowStep)
    {
        colStep = 0;
        rowStep = 0;

        if (direction == Vector3.up)
            colStep = 1;
        else if (direction == Vector3.down)
            colStep = -1;
        else if (direction == Vector3.left)
            rowStep = -1;
        else if (direction == Vector3.right)
            rowStep = 1;
    }

    private bool IsValidAndEmpty(int col, int row)
    {
        if (!locations.ContainsKey(col))
            return false;

        if (!locations[col].ContainsKey(row))
            return false;

        return !occupiedPositions.Contains(locations[col][row]);
    }

    private bool TryGetNextPosition(
        ref int currentColumn,
        ref int currentRow,
        int colStep,
        int rowStep,
        out Vector3 targetPosition)
    {
        int nextColumn = currentColumn + colStep;
        int nextRow = currentRow + rowStep;

        if (IsValidAndEmpty(nextColumn, nextRow))
        {
            currentColumn = nextColumn;
            currentRow = nextRow;
            targetPosition = locations[currentColumn][currentRow];
            return true;
        }

        int[] dCol = { 0, 0, -1, 1 };
        int[] dRow = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int neighborCol = currentColumn + dCol[i];
            int neighborRow = currentRow + dRow[i];

            if (IsValidAndEmpty(neighborCol, neighborRow))
            {
                currentColumn = neighborCol;
                currentRow = neighborRow;
                targetPosition = locations[currentColumn][currentRow];
                return true;
            }
        }

        targetPosition = Vector3.zero;
        return false;
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
		
		if(arrowContainer.childCount <= 2)
		{
			if(colStep == 1 || colStep == -1)
			{
				previousBlock.localRotation = Quaternion.Euler(0, 0, 90);
			}
			
			if(rowStep == 1 || rowStep == -1)
			{
				previousBlock.localRotation = Quaternion.Euler(0, 0, 0);
			}
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

    public void setArrowLength()
    {
		ReturnArrowContainer();
		
        for (int i = 0; i <= 4; i++)
        {
            PlaceArrows();
        }
    }
}
