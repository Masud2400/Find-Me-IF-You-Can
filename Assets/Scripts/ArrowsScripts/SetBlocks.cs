using UnityEngine;
using UnityEngine.UI;

public class SetBlocks : MonoBehaviour
{
	[SerializeField] private GameObject prefabToSpawn; 
    [SerializeField] private Image targetImage;       
    [SerializeField] private Transform spawnParent;
	
	private bool randomPos = false;

	void Awake()
	{
		SpawnPrefabAtRandomLocation();
	}

    public void SpawnPrefabAtRandomLocation()
    {	
		// Deletes old blocks before regeneration
		for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            GameObject child = spawnParent.GetChild(i).gameObject;
            Destroy(child);
        }
	
        RectTransform imageRectTransform = targetImage.rectTransform;
        Rect rect = imageRectTransform.rect;

        float randomX = Random.Range(rect.xMin, rect.xMax);
        float randomY = Random.Range(rect.yMin, rect.yMax);
		
        Vector3 localRandomPosition = new Vector3(randomX, randomY, 0f);
		Vector3 notRandomPos = new Vector3(0f, 0f, 0f);

        Vector3 worldPosition = imageRectTransform.TransformPoint(randomPos ? localRandomPosition : notRandomPos);

        GameObject spawnedObject = Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, spawnParent);
    }
}
