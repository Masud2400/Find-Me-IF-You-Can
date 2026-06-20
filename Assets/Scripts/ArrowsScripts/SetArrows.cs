using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SetArrows : MonoBehaviour
{
	[SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Image targetImage;
	
	void Update()
	{
		SetCount();
	}
	
	private void PlaceArrow(float leftRight, float upDown)
    {
        Transform parentTransform = targetImage.transform;
        int childCount = parentTransform.childCount;
		
        if (childCount > 0)
        {    
            Transform lastChild = parentTransform.GetChild(childCount - 1);
			
            float x = lastChild.localPosition.x + leftRight;
            float y = lastChild.localPosition.y + upDown;
            Vector3 targetLocalPosition = new Vector3(x, y, 0);
			
            GameObject spawnedObject = Instantiate(prefabToSpawn, parentTransform);
            
            spawnedObject.transform.localPosition = targetLocalPosition;
        }
    }
	
	private void SetCount()
	{
		List<string> direction = new List <string> {"up", "down", "left", "right"};
		int randomIndex = Random.Range(0, direction.Count);
		
		int randomBlocksCount = Random.Range(0, 21);
		string randomDirection = direction[randomIndex];
		
		float randomUpDown = 0f;
		float randomRightLeft = 0f;
		
		switch(randomDirection)
		{
			case "up":
				randomUpDown = 30f;
				break;
			case "down":
				randomUpDown = -30f;
				break;
			case "right":
				randomRightLeft = 30f;
				break;
			case "left":
				randomRightLeft = -30f;
				break;
		}
		
		for (int i = 0; i < randomBlocksCount; i++)
		{
			PlaceArrow(randomRightLeft, randomUpDown);
		}
	}
}
