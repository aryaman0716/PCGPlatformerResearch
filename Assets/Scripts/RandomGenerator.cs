using UnityEngine;
using System.Collections.Generic;
public class RandomGenerator : MonoBehaviour
{
    public GameObject startPlatformPrefab;
    public GameObject platformPrefab;
    public GameObject endPlatformPrefab;
    public int numberOfPlatforms = 10;
    public float minGap = 2f;
    public float maxGap = 8f;
    public float minHeight = -3f;
    public float maxHeight = 3f;
    private List<Transform> generatedPlatforms = new List<Transform>(); 
    private void Start()
    {
        GenerateLevel();
    }
    void GenerateLevel()
    {
        Vector3 currentPosition = Vector3.zero;
        GameObject start = Instantiate(startPlatformPrefab, currentPosition, Quaternion.identity); 
        generatedPlatforms.Add(start.transform);     // we add the start platform to the list of generated platforms
        for (int i = 0; i < numberOfPlatforms; i++) 
        {
            float gap = Random.Range(minGap, maxGap); 
            float height = Random.Range(minHeight, maxHeight);
            currentPosition += new Vector3(0, height, gap);   // the current pos is updated by adding the gap and height to the previous position
            GameObject platform = Instantiate(platformPrefab, currentPosition, Quaternion.identity);
            generatedPlatforms.Add(platform.transform);     // we add the newly generated platform to the list of generated platforms
        }
        currentPosition += new Vector3(0, 0, 5); // ensuring there is some gap before the end platform
        GameObject end = Instantiate(endPlatformPrefab, currentPosition, Quaternion.identity);
        generatedPlatforms.Add(end.transform);     // we add the end platform to the list of generated platforms
    }
}
