using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SetBlocks : MonoBehaviour
{	
	private Data gameData;

	private Dictionary<Vector2Int, GridCell> locations;
	private Dictionary<string, List<VectorData>> arrowDict;
	private HashSet<Vector3> occupiedPositions;
	private Dictionary<Vector3, FirstBlock> firstArrowBlock;
	
	private Vector3 randomVector;
	private int counter = 0;
	
	void Start()
	{
		gameData = AssetManager.Instance.GameData;
		
		locations = gameData.locations;
		arrowDict = gameData.arrowDict;
		occupiedPositions = gameData.occupiedPositions;
		firstArrowBlock = gameData.firstArrowBlock;
	}

    private void SetRandomLocation()
    {	
		var availableVectors = locations.Values
			.Where(cell => !occupiedPositions.Contains(cell.position))
			.OrderByDescending(cell => cell.layer)
			.Select(cell => cell.position)
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
			arrowDict[arrowName] = new List<VectorData>();
		}
	}
	
	private void ChooseAngle(out int randomAngle) // This needs refinement
	{
		int[] angles = { 270, 180, 90, 0 };
		randomAngle = Random.Range(0, 4);
	}
	
	private void SaveFirstBlockData(Vector3 randomVector)
	{
		ChooseAngle(out int randomAngle);
		
		// FirstArrowData save
		Vector2Int index = GetIndex.GetGridIndex(randomVector);
		
		if(!firstArrowBlock.ContainsKey(randomVector))
		{
			FirstBlock firstBlock = new FirstBlock
			{
				row = index.x,
				col = index.y,
				angle = randomAngle
			};
			
			firstArrowBlock.Add(randomVector, firstBlock);
		}
		
		
		// Arrow Dictionary save
		SpawnParent(out string arrowName);
		
		Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);

		arrowDict[arrowName].Add(new VectorData { position = randomVector, rotation = rotation });
	}
	
	public void SpawnBlock()
	{
		SetRandomLocation();
		
		SaveFirstBlockData(randomVector);
	}
}
