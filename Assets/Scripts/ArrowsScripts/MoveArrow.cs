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
			transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
			
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

		int col = values[0];
		int row = values[1];
		int angle = values[2];
		
		int finalRow = locations.Last().Value.Last().Key;
		int finalCol = locations.Count - 1; 

		switch (angle)
		{
			case 270: // Up
				targetPos = locations[0][row];
				for (int i = col - 1; i > 0; i--)
				{
					if (occupiedPositions.Contains(locations[i][row])) return;
				}
				break;

			case 90: // Down
				targetPos = locations[finalCol][row];
				for (int i = col + 1; i < locations.Count; i++)
				{
					if (occupiedPositions.Contains(locations[i][row])) return;
				}
				break;

			case 0: // Right
				targetPos = locations[col][finalRow];
				for (int i = row + 1; i < finalRow; i++)
				{
					if (occupiedPositions.Contains(locations[col][i])) return;
				}
				break;

			case 180: // Left
				targetPos = locations[col][0];
				for (int i = row - 1; i > 0; i--)
				{
					if (occupiedPositions.Contains(locations[col][i])) return;
				}
				break;

			default:
				return;
		}
	}
}
