using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour
{
    public Dictionary<string, List<VectorData>> arrowDict;
	public Dictionary<Transform, List<GameObject>> gameObjectReference;
	
	private GameObject prefabToSpawn;        
    private Transform spawnParent; // Parent of arrowParent
	private Transform arrowParent; // Parent prefab
	private Sprite arrowHead;
	private Data gameData;
	
	private int counter = 0;
	
	void Start()
	{
		prefabToSpawn = AssetManager.Instance.PrefabToSpawn;
		spawnParent = AssetManager.Instance.SpawnParent;
		arrowParent = AssetManager.Instance.ArrowParent;
		arrowHead = AssetManager.Instance.ArrowHead;
		gameData = AssetManager.Instance.GameData;
		
		arrowDict = gameData.arrowDict;
		gameObjectReference = gameData.gameObjectReference;
	}
	
	public void GetBlocks()
	{
		foreach(var kvp in arrowDict)
		{
			Transform spawnedParent = Instantiate(arrowParent, spawnParent);
			spawnedParent.name = kvp.Key;
			
			if(!gameObjectReference.ContainsKey(spawnedParent))
			{
				gameObjectReference[spawnedParent] = new List<GameObject>();
			}
			
			VectorData obj = kvp.Value[0];
			
			GameObject spawnedObj = Instantiate(prefabToSpawn, spawnedParent);
			spawnedObj.transform.localPosition = obj.position;
			spawnedObj.transform.localRotation = obj.rotation;
			spawnedObj.name = counter.ToString();
			counter++;
			
			gameObjectReference[spawnedParent].Add(spawnedObj);
			
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
			
			CreateOtherBlocks(kvp, spawnedParent);
		}
	}
	
	private void CreateOtherBlocks(KeyValuePair<string, List<VectorData>> kvp, Transform spawnedParent)
	{
		for (int i = 1; i < kvp.Value.Count; i++)
		{
			VectorData obj = kvp.Value[i];

			GameObject spawnedObj = Instantiate(prefabToSpawn, spawnedParent);
			spawnedObj.transform.localPosition = obj.position;
			spawnedObj.transform.localRotation = obj.rotation;
			
			gameObjectReference[spawnedParent].Add(spawnedObj);
		}
	}
}
