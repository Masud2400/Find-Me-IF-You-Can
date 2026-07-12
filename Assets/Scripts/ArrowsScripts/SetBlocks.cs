using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class SetBlocks : MonoBehaviour
{
	[Header("Game Objects")]
	[SerializeField] private GameObject prefabToSpawn;        
    [SerializeField] private Transform spawnParent;
	[SerializeField] private Transform arrowParent;
	
	[Header("Sprite")]
	[SerializeField] private Sprite arrowHead;
	
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private HashSet<Vector3> occupiedPositions;
	private Vector3 randomVector;
	
	private int counter = 0;
	
	void Start()
	{
		locations = GridManager.Instance.locations;
		occupiedPositions = GridManager.Instance.occupiedPositions;
	}

    private void SetRandomLocation()
    {	
		var allVectors = locations.Values.SelectMany(innerDict => innerDict.Values).ToList();
		
		var availableVectors = allVectors
			.Where(v => !occupiedPositions.Contains(v))
			.ToList();

		if (availableVectors.Count == 0)
		{
			return; 
		}

		randomVector = availableVectors[Random.Range(0, availableVectors.Count)];
		occupiedPositions.Add(randomVector);
    }
	
	private void SpawnParent(out Transform spawnedParent)
	{
		counter += 1;
		
		spawnedParent = Instantiate(arrowParent, spawnParent);
		spawnedParent.name = "Arrow" + counter;
	}
	
	public void SpawnBlock()
	{
		SetRandomLocation();
		
		SpawnParent(out Transform spawnedParent);
		
        GameObject spawnedObj = Instantiate(prefabToSpawn, spawnedParent);
		spawnedObj.transform.localPosition = randomVector;
		
		Image spriteRenderer = spawnedObj.GetComponent<Image>();
		
		if(arrowHead != null)
		{
			spriteRenderer.sprite = arrowHead;
		}
		
		MoveArrow movementScript = spawnedObj.GetComponent<MoveArrow>();
    
		Button btn = spawnedObj.GetComponent<Button>();

		if (btn != null && movementScript != null)
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(movementScript.MoveBlocks);
		}
	}
}
