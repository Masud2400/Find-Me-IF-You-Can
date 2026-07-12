using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MoveArrow : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
    private HashSet<Vector3> occupiedPositions;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	
	[Header("Settings")]
	[SerializeField] private float movementSpeed = 300f;
	
	private Vector3 targetPos = Vector3.zero;
	private bool isMoving = false;

    void Start()
    {
        locations = GridManager.Instance.locations;
        occupiedPositions = GridManager.Instance.occupiedPositions;
		firstArrowBlock = GridManager.Instance.firstArrowBlock;
    }
	
	void Update()
	{
		if(isMoving)
		{
			float step = movementSpeed * Time.deltaTime;

			if(targetPos == Vector3.zero) return;
			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, step);
			
			if (Vector3.Distance(transform.localPosition, targetPos) < 0.001f)
            {
                transform.localPosition = targetPos;
                isMoving = false;
            }
		}
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
				targetPos = locations[0][col];
				Debug.Log(targetPos);
				break;

			case 90: // Down
				for (int i = row + 1; i <= finalRow; i++)
				{
					if (occupiedPositions.Contains(locations[i][col]))
						return;
				}
				targetPos = locations[finalRow][col];
				break;

			case 0: // Left
				for (int i = col - 1; i >= 0; i--)
				{
					if (occupiedPositions.Contains(locations[row][i]))
						return;
				}
				targetPos = locations[row][0];
				break;

			case 180: // Right
				for (int i = col + 1; i <= finalCol; i++)
				{
					if (occupiedPositions.Contains(locations[row][i]))
						return;
				}
				targetPos = locations[row][finalCol];
				break;
		}
	}
}
