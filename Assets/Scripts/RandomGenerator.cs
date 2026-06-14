using System.Collections.Generic;
using UnityEngine;
public class RandomGenerator : MonoBehaviour
{
    [Header("Platform Prefabs")]
    public GameObject startPlatformPrefab;
    public GameObject platformPrefab;
    public GameObject endPlatformPrefab;

    public int numberOfPlatforms = 15;
    public float maxSideOffset = 15f;
    public float minPlatformHeight = 0f;
    public float maxPlatformHeight = 4f;
    public Transform levelStart;
    public Transform levelEnd;
    public Transform player;

    private List<Transform> generatedPlatforms = new List<Transform>();
    private void Start()
    {
        GenerateLevel();
    }
    private void GenerateLevel()
    {
        generatedPlatforms.Clear(); 

        // spawn the start platform
        GameObject start = Instantiate(startPlatformPrefab, levelStart.position, Quaternion.identity);
        generatedPlatforms.Add(start.transform);
        Transform spawnPoint = start.transform.Find("SpawnPoint");
        if (spawnPoint != null && player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.position = spawnPoint.position;
                controller.enabled = true;
            }
        }
        
        float totalLength = levelEnd.position.z - levelStart.position.z;
        float sectionLength = totalLength / (numberOfPlatforms + 1);  

        // using a for loop to generate platforms with random gaps and offsets
        for (int i = 1; i <= numberOfPlatforms; i++)
        {
            float sectionStartZ = levelStart.position.z + (sectionLength * i); 
            float randomZ = sectionStartZ + Random.Range(-sectionLength * 0.3f, sectionLength * 0.3f);
            float randomX = Random.Range(-maxSideOffset, maxSideOffset);
            float randomY = Random.Range(minPlatformHeight, maxPlatformHeight);

            Vector3 platformPosition = new Vector3(randomX, randomY, randomZ);
            GameObject platform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);
            generatedPlatforms.Add(platform.transform);
        }

        // spawn the end platform
        GameObject end = Instantiate(endPlatformPrefab, levelEnd.position, Quaternion.identity);
        generatedPlatforms.Add(end.transform);
    }
    public List<Transform> GeneratedPlatforms
    {
        get { return generatedPlatforms; }
    }
}