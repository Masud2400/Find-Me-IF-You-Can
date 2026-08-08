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
    private Dictionary<GameObject, List<GameObject>> arrowDict;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	private HashSet<Vector3> occupiedPositions;
	
	private string filePath;
	
	void Awake()
	{
		string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		filePath = Path.Combine(documentsPath, "data.json");
	}
	
	void Start()
	{
		arrowDict = GridManager.Instance.arrowDict;
		firstArrowBlock = GridManager.Instance.firstArrowBlock;
		occupiedPositions = GridManager.Instance.occupiedPositions;
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
			GameObject arrow = GameObject.Find(entry.name);

			if (arrow == null)
			{
				arrow = new GameObject(entry.name);
			}

			if (!arrowDict.TryGetValue(arrow, out List<GameObject> points))
			{
				points = new List<GameObject>();
				arrowDict.Add(arrow, points);
			}

			GameObject point = new GameObject("Block");
			
			point.transform.SetParent(arrow.transform, false);

			point.transform.localPosition = new Vector3(
				entry.position.x,
				entry.position.y,
				entry.position.z);

			points.Add(point);
			
			occupiedPositions.Add(point.transform.localPosition);
			
			if (processedArrows.Add(entry.name))
			{
				firstArrowBlock[point.transform.localPosition] = new List<int>
				{
					entry.row,
					entry.col,
					entry.angle
				};
			}
		}
	}
}
