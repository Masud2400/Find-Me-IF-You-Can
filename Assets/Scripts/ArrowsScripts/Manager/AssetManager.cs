using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance { get; private set; }
	
	[SerializeField] private RectTransform targetImage;
	[SerializeField] private Data gameData;
	[SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnParent; // Parent of arrowParent
	[SerializeField] private Transform arrowParent; // Parent prefab
	[SerializeField] private Sprite arrowHead;
	
	public RectTransform TargetImage => targetImage;
	public Data GameData => gameData;
	public GameObject PrefabToSpawn => prefabToSpawn;
	public Transform SpawnParent => spawnParent;
	public Transform ArrowParent => arrowParent;
	public Sprite ArrowHead => arrowHead;

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
