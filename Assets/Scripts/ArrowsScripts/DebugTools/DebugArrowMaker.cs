using UnityEngine;
using System.Collections.Generic;

public class DebugArrowMaker : MonoBehaviour
{
	private Dictionary<int, Dictionary<int, Vector3>> locations;
    private Dictionary<GameObject, List<GameObject>> arrowDict;
	private Dictionary<Vector3, List<int>> firstArrowBlock;
	
	private int counter = 0;
	
	void Start()
	{
		locations = GridManager.Instance.locations;
		arrowDict = GridManager.Instance.arrowDict;
		firstArrowBlock = GridManager.Instance.firstArrowBlock;
	}
	
	public void FillArrows()
	{	
		GameObject leftLookingArrow = new GameObject("Left Looking Arrow");
		leftLookingArrow.transform.localPosition = locations[0][2];
		
		GameObject leftLookingArrowTwo = new GameObject("Left Looking Arrow Two");
		leftLookingArrowTwo.transform.localPosition = locations[1][2];

		//GameObject rightLookingArrow = new GameObject("Right Looking Arrow");
		//rightLookingArrow.transform.localPosition = locations[0][0];
		
		if(counter == 0)
		{
			GameObject arrowOne = new GameObject("ArrowOne");
			
			arrowDict[arrowOne] = new List<GameObject>();
			arrowDict[arrowOne].Add(leftLookingArrow);
			
			firstArrowBlock[leftLookingArrow.transform.localPosition] = new List<int> { 0, 2, 90 };
			
			counter += 1;
			return;
		}
		
		if(counter == 1)
		{
			GameObject arrowTwo = new GameObject("ArrowTwo");
			
			arrowDict[arrowTwo] = new List<GameObject>();
			arrowDict[arrowTwo].Add(leftLookingArrowTwo);
			
			firstArrowBlock[leftLookingArrowTwo.transform.localPosition] = new List<int> { 1, 2, 270 };
			
			counter += 1;
			return;
		}
		
		/*
		if(counter == 2)
		{
			GameObject arrowThree = new GameObject("ArrowThree");
			
			arrowDict[arrowThree] = new List<GameObject>();
			arrowDict[arrowThree].Add(rightLookingArrow);
			
			firstArrowBlock[rightLookingArrow.transform.localPosition] = new List<int> { 0, 0, 180 };
		}*/
	}
}
