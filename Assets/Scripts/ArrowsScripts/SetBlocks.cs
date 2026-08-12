using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SetBlocks : MonoBehaviour
{	
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private HashSet<Vector3> occupiedPositions;
	private Dictionary<string, List<BlockData>> arrowDict;
	private Vector3 randomVector;
	
	private int counter = 0;
	
	void Start()
	{
		locations = GridManager.Instance.locations;
		occupiedPositions = GridManager.Instance.occupiedPositions;
		arrowDict = GridManager.Instance.arrowDict;
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
	
	private void SpawnParent(out string arrowName)
	{
		counter += 1;
		
		arrowName = "Arrow" + counter;
		
		if (!arrowDict.ContainsKey(arrowName))
		{
			arrowDict[arrowName] = new List<BlockData>(); // Adding the key
		}
	}
	
	public void SpawnBlock()
	{
		SetRandomLocation();
		
		SpawnParent(out string arrowName);
		
		arrowDict[arrowName].Add(new BlockData { position = randomVector }); // Adding first block to the arrowDict
	}
}
