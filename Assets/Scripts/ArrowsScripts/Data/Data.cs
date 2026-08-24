using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct GridCell
{
	public Vector3 position;
	public int layer;
}

[System.Serializable]
public class VectorData
{
	public Vector3 position;
	public Quaternion rotation = Quaternion.identity;
}

[System.Serializable]
public class FirstBlock
{
	public int row;
	public int col;
	public int angle;
}

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Data")]
public class Data : ScriptableObject
{	
	public Dictionary<Vector2Int, GridCell> locations = new Dictionary<Vector2Int, GridCell>();
	public HashSet<Vector3> occupiedPositions = new();
	public Dictionary<string, List<VectorData>> arrowDict = new Dictionary<string, List<VectorData>>();
	public Dictionary<Vector3, FirstBlock> firstArrowBlock = new Dictionary<Vector3, FirstBlock>();
	public Dictionary<Transform, List<GameObject>> gameObjectReference = new Dictionary<Transform, List<GameObject>>();
	
	public int currentLayer;
}
