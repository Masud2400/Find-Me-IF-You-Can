using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class ArrowWrapper
{
    public List<ArrowEntry> arrows;
}

[Serializable]
public class ArrowEntry
{
    public string name;
    public PositionData position;
	public int row;
	public int col;
	public int angle;
}

[Serializable]
public class PositionData
{
    public float x;
    public float y;
    public float z;
}

public class DebugArrowMaker : MonoBehaviour
{
	[SerializeField] private Data gameData;
	
    private Dictionary<string, List<VectorData>> arrowDict;
	private Dictionary<Vector3, FirstBlock> firstArrowBlock;
	private HashSet<Vector3> occupiedPositions;
	
	private string filePath;
	
	void Awake()
	{
		string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		filePath = Path.Combine(documentsPath, "data.json");
	}
	
	void Start()
	{
		arrowDict = gameData.arrowDict;
		firstArrowBlock = gameData.firstArrowBlock;
		occupiedPositions = gameData.occupiedPositions;
	}
	
	private ArrowWrapper LoadData()
    {
        return JsonUtility.FromJson<ArrowWrapper>(File.ReadAllText(filePath));
    }
	
	public void FillArrows()
	{	
		ArrowWrapper data = LoadData();
		HashSet<string> processedArrows = new HashSet<string>();

		foreach (ArrowEntry entry in data.arrows)
		{	
			string arrow = entry.name;

			if (!arrowDict.TryGetValue(arrow, out List<VectorData> points))
			{
				points = new List<VectorData>();
				arrowDict.Add(arrow, points);
			}

			Vector3 point = Vector3.zero;

			point = new Vector3(
				entry.position.x,
				entry.position.y,
				entry.position.z);

			points.Add(new VectorData
			{
				position = point
			});
			
			occupiedPositions.Add(point);
			
			if (processedArrows.Add(entry.name))
			{
				firstArrowBlock[point] = new FirstBlock
				{
					row = entry.row,
					col = entry.col,
					angle = entry.angle
				};
			}
		}
	}
}
