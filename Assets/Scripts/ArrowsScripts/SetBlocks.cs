using UnityEngine;
using UnityEngine.UI;

public class SetBlocks : MonoBehaviour
{
	[SerializeField] private GameObject prefabToSpawn; 
    [SerializeField] private Image targetImage;       
    [SerializeField] private Transform spawnParent;

	private bool spawned = false;

	void Update()
	{
		if(!spawned)
		{
			SpawnPrefabAtRandomLocation();
		}
	}

    private void SpawnPrefabAtRandomLocation()
    {	
		spawned = true;
	
        RectTransform imageRectTransform = targetImage.rectTransform;
        Rect rect = imageRectTransform.rect;

        float randomX = Random.Range(rect.xMin, rect.xMax);
        float randomY = Random.Range(rect.yMin, rect.yMax);
        Vector3 localRandomPosition = new Vector3(randomX, randomY, 0f);

        Vector3 worldPosition = imageRectTransform.TransformPoint(localRandomPosition);

        GameObject spawnedObject = Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, spawnParent);
    }
}
