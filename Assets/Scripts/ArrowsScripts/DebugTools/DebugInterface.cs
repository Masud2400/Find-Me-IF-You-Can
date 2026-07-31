using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class DebugInterface : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	
	[SerializeField] private Button prefabToSpawn;        
    [SerializeField] private Transform spawnParent;
	
	private Color pressedColor = Color.red;
	
	void Start()
	{
		locations = GridManager.Instance.locations;
	}
	
	public void MakeInterface()
	{
		foreach (var key in locations)
		{
			foreach (var value in key.Value)
			{
				Vector3 spawnPosition = value.Value;
				
				Button spawnedObj = Instantiate(prefabToSpawn, spawnParent);
				spawnedObj.transform.localPosition = spawnPosition;
				
				Image img = spawnedObj.GetComponent<Image>();
				spawnedObj.onClick.AddListener(() => img.color = pressedColor);
			}
		}
	}
}
