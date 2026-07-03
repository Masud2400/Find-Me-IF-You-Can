using UnityEngine;
using System.Collections.Generic;

public class GridGen : MonoBehaviour
{
    [SerializeField] private RectTransform targetImage;
	[SerializeField] private GameObject prefabToSpawn;
	
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private const float GRID_SIZE = 30f;
	
	void Start()
	{
		locations = GridManager.Instance.locations;
	}
	
	public void GenerateGrid()
	{
		float width = targetImage.rect.width;
		float height = targetImage.rect.height;
		
		int widthCount = Mathf.RoundToInt(width / GRID_SIZE);
		int heightCount = Mathf.RoundToInt(height / GRID_SIZE);
		
		float startX = (-width / 2f) + (GRID_SIZE / 2f);
		float startY = (height / 2f) - (GRID_SIZE / 2f);
		
		for(int i = 0; i < heightCount; i++)
		{
			float currentY = startY - (i * GRID_SIZE);
			
			if(!locations.ContainsKey(i))
			{
				locations[i] = new Dictionary<int, Vector3>();
			}
			
			for(int k = 0; k < widthCount; k++)
			{
				float currentX = startX + (k * GRID_SIZE);
				
				Vector3 spawnPosition = new Vector3(currentX, currentY, 0);
				
				locations[i][k] = spawnPosition;
			}
		}
	}
}
