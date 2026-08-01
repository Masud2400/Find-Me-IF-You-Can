using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class ArrowData
{
    public string name;
    public Vector3 position;
	public int row;
	public int col;
	public int angle;
}

[System.Serializable]
public class DataWrapper
{
    public List<ArrowData> arrows = new List<ArrowData>();
}

public class DebugInterface : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
	private string filePath;
	private int arrowCounter = 0;
	
	[SerializeField] private Button prefabToSpawn;        
    [SerializeField] private Transform spawnParent;
	
	private Color pressedColor = Color.red;
	
	void Awake()
	{
		string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		filePath = Path.Combine(documentsPath, "data.json");
	}
	
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
				
				spawnedObj.onClick.AddListener(() => SaveArrowToJson(spawnPosition, key.Key, value.Key));
			}
		}
	}
	
	public DataWrapper LoadData()
    {
        if (!File.Exists(filePath))
        {
            return new DataWrapper();
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<DataWrapper>(json) ?? new DataWrapper();
    }
	
	public void SaveArrowToJson(Vector3 position, int row, int col)
	{
		DataWrapper wrapper = LoadData();
		
        wrapper.arrows.Add(new ArrowData
		{
			name = $"Arrow {arrowCounter}",
			position = position,
			row = row,
			col = col,
		});

        string json = JsonUtility.ToJson(wrapper, prettyPrint: true);
        File.WriteAllText(filePath, json);
	}
	
	public void SetArrow()
	{
		arrowCounter += 1; 
	}
}
