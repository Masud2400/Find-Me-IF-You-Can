using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class SetArrows : MonoBehaviour
{
    private Data gameData;

	private Dictionary<Vector2Int, GridCell> locations;
	private Dictionary<string, List<VectorData>> arrowDict;
	private HashSet<Vector3> occupiedPositions;
	private Dictionary<Vector3, FirstBlock> firstArrowBlock;
	
	private List<Vector2Int> indices = new List<Vector2Int>();
	private Vector2Int lastIndex;
	
	void Start()
	{
		gameData = AssetManager.Instance.GameData;
		
		locations = gameData.locations;
		arrowDict = gameData.arrowDict;
		occupiedPositions = gameData.occupiedPositions;
		firstArrowBlock = gameData.firstArrowBlock;
	}
	
	private KeyValuePair<string, List<VectorData>> GetLastArrow()
	{
		return arrowDict.Last();
	}
	
	private KeyValuePair<Vector3, FirstBlock> GetFirstBlock()
	{
		return firstArrowBlock.Last();
	}
	
	private GridCell GetCell(Vector2Int index)
	{
		return locations[index];
	}
	
	private int GetLength()
	{
		return Random.Range(10, 20);
	}
	
	private int GetOppositeAngle(int angle)
	{
		return (angle + 180) % 360;
	}
	
	private bool CheckIsOccupied(Vector2Int index)
	{	
		int layer = locations[index].layer;
		Vector3 position = locations[index].position;
		
		if(gameData.currentLayer != layer)
			return true;
		
		if(occupiedPositions.Contains(position))
			return true;
		
		return false;
	}
	
	private int[] TryGetNextDirection(int value)
	{
		int[] angles = { 270, 90, 0, 180 };
		
		return angles.Where(angle => angle != value).ToArray();
	}
	
	private void GetBlocks(Vector2Int block, int angle)
	{	
		int arrowLength = GetLength();
		
		Vector2Int index;
		
		for(int i = 1; i <= arrowLength; i++)
		{	
			switch(angle)
			{
				case 270:
					index = new Vector2Int(block.x + i, block.y);
					if(CheckIsOccupied(index)) return;
					indices.Add(index);
					break;
				case 90:
					index = new Vector2Int(block.x - i, block.y);
					if(CheckIsOccupied(index)) return;
					indices.Add(index);
					break;
				case 0:
					index = new Vector2Int(block.x, block.y + i);
					if(CheckIsOccupied(index)) return;
					indices.Add(index);
					break;
				case 180:
					index = new Vector2Int(block.x, block.y - i);
					if(CheckIsOccupied(index)) return;
					indices.Add(index);
					break;
			}
		}
	}
	
	private void AddToArrowDict(Vector2Int block, int angle)
	{
		GetBlocks(block, angle);
		
		var arrow = GetLastArrow();
		
		foreach(Vector2Int index in indices)
		{
			GridCell cell = GetCell(index);
			
			arrow.Value.Add(new VectorData {
				position = cell.position,
				rotation = Quaternion.Euler( 0, 0, angle )
			});
			
			occupiedPositions.Add(cell.position);
		}
		
		if (indices.Count > 0) 
		{
			lastIndex = indices[^1];
		}

		indices.Clear();
	}
	
	public void LayArrows()
	{
		var blockData = GetFirstBlock();
		Vector2Int block = new Vector2Int(blockData.Value.row, blockData.Value.col);
		int angle = blockData.Value.angle;
		
		lastIndex = block; // In case there is no last index assigned
		
		AddToArrowDict(block, angle);
		
		if(lastIndex == block)
		{
			angle = GetOppositeAngle(angle); // Prevents lonely first block from changing its angle
		}
		
		int[] result = TryGetNextDirection(angle);
		
		for(int i = 0; i < result.Length; i++)
		{	
			angle = result[i];
			AddToArrowDict(lastIndex, angle);
		}
	}
}
