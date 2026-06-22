using UnityEngine;
using System.Collections.Generic;
public class ConstraintGenerator : MonoBehaviour
{
    [Header("Platform Prefabs")]
    public GameObject startPlatformPrefab;
    public GameObject platformPrefab;
    public GameObject endPlatformPrefab;

    public int numberOfPlatforms = 15;
    public float maxSideOffset = 7f;
    public float minPlatformHeight = 0f;
    public float maxPlatformHeight = 4f;
    public Transform levelStart;
    public Transform levelEnd;
    public Transform player;

    [Header("Player Constraints")]
    public float maxJumpDistance = 6.4f;
    public float maxJumpHeight = 2f;

    private List<Transform> generatedPlatforms = new List<Transform>();
    private void Start()
    {
        GenerateLevel();
    }
    private void GenerateLevel()
    {
        generatedPlatforms.Clear();
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
        Vector3 previousPosition = levelStart.position;

        for (int i = 1; i <= numberOfPlatforms; i++)
        {
            bool validPlatform = false;
            Vector3 candidatePosition = Vector3.zero;
            int attempts = 0;
            while (!validPlatform && attempts < 100)
            {
                attempts++;
                float sectionStartZ = levelStart.position.z + (sectionLength * i);
                float randomZ = sectionStartZ + Random.Range(-sectionLength * 0.3f, sectionLength * 0.3f);
                float randomX = Random.Range(-maxSideOffset, maxSideOffset);
                float randomY = Random.Range(minPlatformHeight, maxPlatformHeight);
                candidatePosition = new Vector3(randomX, randomY, randomZ);

                float horizontalDistance = Vector2.Distance(new Vector2(previousPosition.x, previousPosition.z), new Vector2(candidatePosition.x, candidatePosition.z));
                float verticalDifference = candidatePosition.y - previousPosition.y;
                bool validDistance = horizontalDistance <= maxJumpDistance;
                bool validHeight = verticalDifference <= maxJumpHeight;
                if (validDistance && validHeight)
                {
                    validPlatform = true;
                }
            }

            if (!validPlatform)
            {
                candidatePosition = previousPosition + new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), maxJumpDistance * 0.8f);
            }
            GameObject platform = Instantiate(platformPrefab, candidatePosition, Quaternion.identity);
            generatedPlatforms.Add(platform.transform);
            previousPosition = candidatePosition;
        }
        Vector3 endPosition = levelEnd.position;
        float endHorizontalDistance = Vector2.Distance(new Vector2(previousPosition.x, previousPosition.z), new Vector2(endPosition.x, endPosition.z));
        if (endHorizontalDistance > maxJumpDistance)
        {
            endPosition = previousPosition + new Vector3(0f, 0f, maxJumpDistance * 0.8f);
        }
        GameObject end = Instantiate(endPlatformPrefab, endPosition, Quaternion.identity);
        generatedPlatforms.Add(end.transform);

        LevelEvaluator evaluator = FindFirstObjectByType<LevelEvaluator>();
        if (evaluator != null)
        {
            evaluator.EvaluateLevel();
        }
    }
    public List <Transform> GeneratedPlatforms
    {
        get { return generatedPlatforms; }
    }
}
