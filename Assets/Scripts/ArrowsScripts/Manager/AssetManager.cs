using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance { get; private set; }
	
	[SerializeField] private RectTransform targetImage;
	[SerializeField] private Data gameData;
	[SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnParent;
	
	public RectTransform TargetImage => targetImage;
	public Data GameData => gameData;
	public GameObject PrefabToSpawn => prefabToSpawn;
	public Transform SpawnParent => spawnParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
