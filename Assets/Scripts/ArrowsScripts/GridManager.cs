using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridGen gridGen;
	[SerializeField] private SetBlocks setBlocks;
	[SerializeField] private SetArrows setArrows;
	[SerializeField] private ExitChecker exitChecker;
	//For simulating the arrows and debugging
	[SerializeField] private DebugArrowMaker debugArrowMaker;
	[SerializeField] private DebugInterface debugInterface;
	
	public static GridManager Instance;
	public Dictionary<int, Dictionary<int, Vector3>> locations = new Dictionary<int, Dictionary<int, Vector3>>();
	public HashSet<Vector3> occupiedPositions = new();
	public Dictionary<Vector3, List<int>> firstArrowBlock = new Dictionary<Vector3, List<int>>();
	public Dictionary<GameObject, List<GameObject>> arrowDict = new Dictionary<GameObject, List<GameObject>>();
	
	void Awake()
	{
		Instance = this;
	}
	
	void Start()
	{
		gridGen.GenerateGrid();
	}
	
	//Debugging
	public void CreateInterface()
	{
		debugInterface.MakeInterface();
	}
	
	public void makeArrows()
	{
		//setBlocks.SpawnBlock();
		//setArrows.setArrowLength();
		debugArrowMaker.FillArrows();
		Debug.Log(exitChecker.CheckExit());
	}
}
