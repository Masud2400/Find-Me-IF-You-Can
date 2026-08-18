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
	[SerializeField] private Data gameData;
	[SerializeField] private Button prefabToSpawn;        
    [SerializeField] private Transform spawnParent;
	
	private string filePath;
	private int arrowCounter = 0;
	
	private Color pressedColor = Color.red;
	
	private Dictionary<Vector2Int, GridCell> locations;
	
	void Awake()
	{
		string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		filePath = Path.Combine(documentsPath, "data.json");
	}
	
	void Start()
	{
		locations = gameData.locations;
	}
	
	public void MakeInterface()
	{
		foreach(var pair in locations)
		{
			Vector2Int index = pair.Key;
			GridCell cell = pair.Value;
			
			Vector3 spawnPosition = cell.position;
			
			Button spawnedObj = Instantiate(prefabToSpawn, spawnParent);
			spawnedObj.transform.localPosition = spawnPosition;
			
			Image img = spawnedObj.GetComponent<Image>();
			
			float hue = ((cell.layer - 1) * 0.61803398875f) % 1.0f;
			img.color = Color.HSVToRGB(hue, 0.5f, 1.0f);
			
			spawnedObj.onClick.AddListener(() => img.color = pressedColor);
			
			spawnedObj.onClick.AddListener(() => SaveArrowToJson(spawnPosition, index.x, index.y));
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
