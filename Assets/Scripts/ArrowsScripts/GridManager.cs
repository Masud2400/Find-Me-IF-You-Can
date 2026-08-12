using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class BlockData
{
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
}

public class GridManager : MonoBehaviour
{
	[Header("Main scripts")]
    [SerializeField] private GridGen gridGen;
	[SerializeField] private SetBlocks setBlocks;
	[SerializeField] private SetArrows setArrows;
	[SerializeField] private ExitChecker exitChecker;
	[SerializeField] private BlockManager blockManager;
	
	[Header("Debugging Scripts")]
	//For simulating the arrows and debugging
	[SerializeField] private DebugArrowMaker debugArrowMaker;
	[SerializeField] private DebugInterface debugInterface;
	
	public static GridManager Instance;
	
	public Dictionary<int, Dictionary<int, Vector3>> locations = new Dictionary<int, Dictionary<int, Vector3>>();
	public HashSet<Vector3> occupiedPositions = new();
	public Dictionary<Vector3, List<int>> firstArrowBlock = new Dictionary<Vector3, List<int>>();
	public Dictionary<string, List<BlockData>> arrowDict = new Dictionary<string, List<BlockData>>();
	public Dictionary<Transform, List<GameObject>> gameObjectReference = new Dictionary<Transform, List<GameObject>>();
	
	//Infinite loop guard
	private int currentIteration = 0;
	private int maxIteration = 1000;
	
	void Awake()
	{
		Instance = this;
	}
	
	void Start()
	{
		gridGen.GenerateGrid();
	}
	
	public void makeArrows()
	{
		var allVectors = locations.Values.SelectMany(innerDict => innerDict.Values).ToList();
		
		HashSet<Vector3> seenVectors = new HashSet<Vector3>();
	
		while(seenVectors.Count < allVectors.Count)
		{	
			//Infinite Loop Guard
			if(++currentIteration > maxIteration)
			{
				Debug.Log("Infinite loop detected");
				break;
			}
			
			setBlocks.SpawnBlock();
			setArrows.setArrowLength();
			
			foreach(var kvp in arrowDict)
			{
				foreach(BlockData i in kvp.Value)
				{
					seenVectors.Add(i.position);
				}
			}
			
			exitChecker.CheckExit();
		}
		
		if(seenVectors.Count == allVectors.Count)
		{
			Debug.Log("No more ways left");
		}
		
		//LogData.SaveToJson(arrowDict);
	}
}
