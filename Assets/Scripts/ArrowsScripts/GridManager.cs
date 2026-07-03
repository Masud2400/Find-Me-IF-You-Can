using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridGen gridGen;
	[SerializeField] private SetBlocks setBlocks;
	[SerializeField] private SetArrows setArrows;
	
	public static GridManager Instance;
	public Dictionary<int, Dictionary<int, Vector3>> locations = new Dictionary<int, Dictionary<int, Vector3>>();
	public HashSet<Vector3> occupiedPositions = new();
	
	void Awake()
	{
		Instance = this;
	}
	
	void Start()
	{
		gridGen.GenerateGrid();
		setBlocks.SpawnBlock();
		setArrows.setArrowLength();
	}
}
