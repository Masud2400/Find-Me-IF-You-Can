using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; // Only for debugging

public class SetArrows : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Image targetImage;
	
	private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private const float GRID_SIZE = 30f;
	private Transform parentTransform;
	
	private int numberOfBlocks;
	
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
            Vector3 targetPosition = lastChild.localPosition + dir;

            if (occupiedPositions.Contains(targetPosition))
                continue;

            GameObject spawnedObject = Instantiate(prefabToSpawn, parentTransform);
			spawnedObject.transform.localPosition = targetPosition;
		
			occupiedPositions.Add(targetPosition);
			
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
		
		numberOfBlocks += blocksCount; // Later will be deleted
    }
	
	public void Generate()
	{
		occupiedPositions.Clear();
		
		numberOfBlocks = 0; // Will be deleted later
		
		if (targetImage.transform.childCount > 0)
		{
			occupiedPositions.Add(parentTransform.GetChild(0).localPosition);
		}
		
		int randomGen = 3; //This will be used later: Random.Range(1, 6);
		
		for(int i = 0; i < randomGen; i++)
		{
			SetRandomBlocks();
		}
		
		// Only for debugging
		Debug.Log("All generated blocks: " + numberOfBlocks);
		StartCoroutine(DebugChildCount());
	}
	
	// Only for debugging
	IEnumerator DebugChildCount()
	{
		yield return new WaitForSeconds(3f);
		Debug.Log("Blocks in use: " + parentTransform.childCount);
	}
}
