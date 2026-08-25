using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
	
	public static GridManager Instance;
	
	public Dictionary<int, Dictionary<int, Vector3>> locations = new Dictionary<int, Dictionary<int, Vector3>>();
	public HashSet<Vector3> occupiedPositions = new();
	public Dictionary<Vector3, List<int>> firstArrowBlock = new Dictionary<Vector3, List<int>>();
	public Dictionary<string, List<BlockData>> arrowDict = new Dictionary<string, List<BlockData>>();
	public Dictionary<Transform, List<GameObject>> gameObjectReference = new Dictionary<Transform, List<GameObject>>();
	
	/*
	//Infinite loop guard
	private int currentIteration = 0;
	private int maxIteration = 10000;*/
	
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
		for(int i = 0; i < 100; i++)
		{
			setBlocks.SpawnBlock();
		
			setArrows.LayArrows();
			
			exitChecker.CheckExit();
		}
		
		blockManager.GetBlocks();
	}
}
