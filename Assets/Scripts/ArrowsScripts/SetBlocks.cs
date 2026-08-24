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
	
	private List<Vector3> availableVectors;
	private List<Vector3> chosenVectors = new List<Vector3>();
	
	private Vector3 randomVector;
	private int counter = 0;
	private int currentLayer;
	private bool layerInitialized = false;
	
	void Start()
	{
		gameData = AssetManager.Instance.GameData;
		
		locations = gameData.locations;
		arrowDict = gameData.arrowDict;
		occupiedPositions = gameData.occupiedPositions;
		firstArrowBlock = gameData.firstArrowBlock;
	}
	
	private void GetCurrentLayer()
	{
		if(!layerInitialized)
		{
			currentLayer = locations.Values.Max(c => c.layer);
			layerInitialized = true;
		}
		
		bool isLayerFull = locations.Values
			.Where(c => c.layer == currentLayer)
			.All(c => occupiedPositions.Contains(c.position));

		if (isLayerFull)
		{
			currentLayer--;
		}
	}
	
	private void GetAvailableVectors()
	{
		availableVectors = locations.Values
			.Where(c => c.layer == currentLayer && !occupiedPositions.Contains(c.position))
			.Select(c => c.position)
			.ToList();
		
		if (availableVectors.Count == 0)
		{
			return; 
		}
	}

    private void SetRandomLocation()
    {	
		int index = Random.Range(0, availableVectors.Count);
		randomVector = availableVectors[index];
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
	
	private int ChooseAngle() // This needs refinement
	{
		int[] angles = { 270, 180, 90, 0 };
		return angles[Random.Range(0, angles.Length)];
	}
	
	private void SaveFirstBlockData()
	{
		int randomAngle = ChooseAngle(); 
		
		// FirstArrowData save
		var match = locations.FirstOrDefault(pair => pair.Value.position == randomVector);
		Vector2Int index = match.Key;
		
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
		
		// Save occupiedPositions
		occupiedPositions.Add(randomVector);
	}
	
	public void SpawnBlock()
	{
		GetCurrentLayer();
		
		GetAvailableVectors();
		
		SetRandomLocation();
		
		SaveFirstBlockData();
		
		gameData.currentLayer = currentLayer;
	}
}
