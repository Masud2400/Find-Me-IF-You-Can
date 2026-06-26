using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SetBlocks : MonoBehaviour
{
	[Header("Game Objects")]
	[SerializeField] private GameObject prefabToSpawn; 
    [SerializeField] private Image targetImage;       
    [SerializeField] private Transform spawnParent;
	
	private Vector3Int localRandomPosition;
	private RectTransform imageRectTransform;

	void Awake()
	{
		imageRectTransform = targetImage.rectTransform;
		
		//PlaceRandomBlock();
	}

    private void SetRandomLocation()
    {	
        Rect rect = imageRectTransform.rect;

        float randomX = Random.Range(rect.xMin, rect.xMax);
        float randomY = Random.Range(rect.yMin, rect.yMax);
		
        localRandomPosition = Vector3Int.RoundToInt(new Vector3(randomX, randomY, 0f));
    }
	
	private void SpawnBlock()
	{
		//occupiedPositions.Add(localRandomPosition);

        Vector3 worldPosition = imageRectTransform.TransformPoint(localRandomPosition);
        GameObject spawnedObject = Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, spawnParent);
	}
	
	public void PlaceRandomBlock()
	{
		SetRandomLocation();
		/*
		if(occupiedPositions.Contains(localRandomPosition))
		{
			Debug.Log("You can't put random block here");
			return;
		}*/
		SpawnBlock();
	}
}
