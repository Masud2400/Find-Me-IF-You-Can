using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SetArrows : MonoBehaviour
{
	[Header("Game Objects")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Image targetImage;
	
    private const float GRID_SIZE = 30f;
	private Transform parentTransform;
	
    void Start()
    {
		parentTransform = targetImage.transform;
		Generate();
    }
	
	private void PlaceBlocks(Vector3 direction)
    {
        int childCount = parentTransform.childCount;
		
		if (childCount == 0) return;
				
		Transform lastChild = parentTransform.GetChild(childCount - 1);
	
		Vector3[] directions =
        {
            direction,
            Vector3.up * GRID_SIZE,
            Vector3.right * GRID_SIZE,
            Vector3.down * GRID_SIZE,
            Vector3.left * GRID_SIZE
        };
		
		foreach (Vector3 dir in directions)
        {
            Vector3Int targetPosition = Vector3Int.RoundToInt(lastChild.localPosition + dir);

            //if (occupiedPositions.Contains(targetPosition))
            //    continue;
			
			RectTransform parentRect = parentTransform.GetComponent<RectTransform>();
			if (!parentRect.rect.Contains(new Vector2(targetPosition.x, targetPosition.y)))
				continue;

            GameObject spawnedObject = Instantiate(prefabToSpawn, parentTransform);
			spawnedObject.transform.localPosition = targetPosition;
		
			//occupiedPositions.Add(targetPosition);
			
            return;
        }
    }
    
    private void SetRandomBlocks()
    {
        Vector3[] directions =
        {
            Vector3.up * GRID_SIZE,
            Vector3.down * GRID_SIZE,
            Vector3.left * GRID_SIZE,
            Vector3.right * GRID_SIZE
        };
		
        Vector3 randomDirection = directions[Random.Range(0, directions.Length)];      
		
        int blocksCount = Random.Range(1, 21);
		
		for(int i = 0; i < blocksCount; i++)
		{
			PlaceBlocks(randomDirection);
		}
    }
	
	public void Generate()
	{	
		int randomGen = Random.Range(1, 6);
		
		for(int i = 0; i < randomGen; i++)
		{
			SetRandomBlocks();
		}
	}
}
