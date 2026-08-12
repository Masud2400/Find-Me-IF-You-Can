using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour
{
    public Dictionary<string, List<BlockData>> arrowDict;
	public Dictionary<Transform, List<GameObject>> gameObjectReference;
	
	[Header("Game Objects")]
	[SerializeField] private GameObject prefabToSpawn;        
    [SerializeField] private Transform spawnParent; // Parent of arrowParent
	[SerializeField] private Transform arrowParent; // Parent prefab
	
	[Header("Sprite")]
	[SerializeField] private Sprite arrowHead;
	
	void Start()
	{
		arrowDict = GridManager.Instance.arrowDict;
		gameObjectReference = GridManager.Instance.gameObjectReference;
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
			
			BlockData obj = kvp.Value[0];
			
			GameObject spawnedObj = Instantiate(
				prefabToSpawn, 
				obj.position, 
				obj.rotation, 
				spawnedParent);
			
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
	
	private void CreateOtherBlocks(KeyValuePair<string, List<BlockData>> kvp, Transform spawnedParent)
	{
		for (int i = 1; i < kvp.Value.Count; i++)
		{
			BlockData obj = kvp.Value[i];

			GameObject spawnedObj = Instantiate(
				prefabToSpawn, 
				obj.position, 
				obj.rotation, 
				spawnedParent);
			
			gameObjectReference[spawnedParent].Add(spawnedObj);
		}
	}
}
