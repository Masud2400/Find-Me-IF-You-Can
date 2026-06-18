using UnityEngine;

public class FirePlaceLight : MonoBehaviour
{
    [Header("Flicker Settings")]
    [SerializeField] private float speedMultiplier = 5f;
    [SerializeField] private float minIntensity = 15f;
    [SerializeField] private float maxIntensity = 30f;
	
    [Header("Game Objects")]
    [SerializeField] private Light pointLight;
    
    private float randomOffset;
	
    void Start()
    {
        randomOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * speedMultiplier + randomOffset, 0f);
        
        pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
