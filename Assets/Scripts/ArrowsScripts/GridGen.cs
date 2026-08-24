using UnityEngine;
using System.Collections.Generic;

public class GridGen : MonoBehaviour
{
	private Data gameData;
	private RectTransform targetImage;
	
	private const float GRID_SIZE = 30f;
	
	void Start()
	{
		targetImage = AssetManager.Instance.TargetImage;
		gameData = AssetManager.Instance.GameData;
	}
	
	public void GenerateGrid()
	{	
		float width = targetImage.rect.width;
		float height = targetImage.rect.height;
		
		int widthCount = Mathf.RoundToInt(width / GRID_SIZE);
		int heightCount = Mathf.RoundToInt(height / GRID_SIZE);
		
		float startX = (-width / 2f) + (GRID_SIZE / 2f);
		float startY = (height / 2f) - (GRID_SIZE / 2f);
		
		int gap = 1;
		
		for(int i = 0; i < heightCount; i++)
		{	
			float currentY = startY - (i * GRID_SIZE);
			
			for(int k = 0; k < widthCount; k++)
			{	
				float currentX = startX + (k * GRID_SIZE);
				
				int distY = Mathf.Min(i, heightCount - 1 - i);
				int distX = Mathf.Min(k, widthCount - 1 - k);
				int minDist = Mathf.Min(distX, distY);
				
				int assignedLayer = (minDist / gap) + 1;
				
				Vector3 spawnPosition = new Vector3(currentX, currentY, 0);
				
				Vector2Int index = new Vector2Int(i, k);
				GridCell cell = new GridCell
				{
					position = spawnPosition,
					layer = assignedLayer
				};
				
				gameData.locations.Add(index, cell);
			}
		}
	}
}
