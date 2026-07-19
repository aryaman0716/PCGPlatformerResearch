using UnityEngine;
using System.Collections.Generic;
public class ConstraintGenerator : MonoBehaviour, ILevelGenerator
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

    public int CurrentSeed { get; private set; }
    private List<Transform> generatedPlatforms = new List<Transform>();
    public ReachabilityValidator validator;

    public Difficulty difficulty = Difficulty.Medium;  // default
    private void GetDifficultyRanges(out float minGap, out float maxGap, out float minHeight, out float maxHeight)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                minGap = maxJumpDistance * 0.25f;
                maxGap = maxJumpDistance * 0.50f;
                minHeight = 0f;
                maxHeight = maxJumpHeight * 0.30f;
                break;
            case Difficulty.Medium:
                minGap = maxJumpDistance * 0.50f;
                maxGap = maxJumpDistance * 0.75f;
                minHeight = maxJumpHeight * 0.30f;
                maxHeight = maxJumpHeight * 0.60f;
                break;
            default:  // Difficulty.Hard
                minGap = maxJumpDistance * 0.75f;
                maxGap = maxJumpDistance * 0.95f;
                minHeight = maxJumpHeight * 0.60f;
                maxHeight = maxJumpHeight * 0.90f;
                break;
        }
    }
    public void GenerateLevel(int? seed = null)
    {
        if (seed.HasValue)
        {
            CurrentSeed = seed.Value;
        }
        else
        {
            CurrentSeed = Random.Range(int.MinValue, int.MaxValue);
        }
        Random.InitState(CurrentSeed);
        GetDifficultyRanges(out float minGap, out float maxGap, out float minHeight, out float maxHeight);
        ClearLevel();
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
            const int maxAttempts = 1000;
            int attempts = 0;
            while (!validPlatform && attempts < maxAttempts)
            {
                attempts++;
                float sectionStartZ = levelStart.position.z + (sectionLength * i);
                float randomZ = sectionStartZ + Random.Range(-sectionLength * 0.3f, sectionLength * 0.3f);
                float randomX = Random.Range(-maxSideOffset, maxSideOffset);
                float randomY = Random.Range(Mathf.Max(minPlatformHeight, previousPosition.y - maxJumpHeight), Mathf.Min(maxPlatformHeight, previousPosition.y + maxJumpHeight));
                candidatePosition = new Vector3(randomX, randomY, randomZ);
                float minZ = levelStart.position.z + sectionLength * i - sectionLength * 0.3f;
                float maxZ = levelStart.position.z + sectionLength * i + sectionLength * 0.3f;
                if (candidatePosition.z < minZ || candidatePosition.z > maxZ)
                {
                    continue;
                }
                candidatePosition.x = Mathf.Clamp(candidatePosition.x, -maxSideOffset, maxSideOffset);

                GameObject tempPlatform = Instantiate(platformPrefab, candidatePosition, Quaternion.identity);
                Collider previousCollider = generatedPlatforms[generatedPlatforms.Count - 1].GetComponent<Collider>();
                Collider currentCollider = tempPlatform.GetComponent<Collider>();
                Bounds previousBounds = previousCollider.bounds;
                Bounds currentBounds = currentCollider.bounds;
                Vector3 previousEdge = previousBounds.ClosestPoint(currentBounds.center);
                Vector3 currentEdge = currentBounds.ClosestPoint(previousBounds.center);
                float horizontalGap = Vector2.Distance(new Vector2(previousEdge.x, previousEdge.z), new Vector2(currentEdge.x, currentEdge.z));
                float verticalDifference = Mathf.Abs(currentBounds.max.y - previousBounds.max.y);
                bool physicsDistance = horizontalGap <= maxJumpDistance;
                bool physicsHeight = verticalDifference <= maxJumpHeight;
                bool difficultyDistance = horizontalGap >= minGap && horizontalGap <= maxGap;
                bool difficultyHeight = verticalDifference >= minHeight && verticalDifference <= maxHeight;
                Destroy(tempPlatform);
                if (physicsDistance && physicsHeight && difficultyDistance && difficultyHeight)
                {
                    validPlatform = true;
                }
            }

            //if (!validPlatform)
            //{
            //    float sectionStartZ = levelStart.position.z + (sectionLength * i);
            //    candidatePosition = new Vector3(Random.Range(-maxSideOffset, maxSideOffset), previousPosition.y + Random.Range(minHeight,maxHeight), sectionStartZ);
            //}
            if (!validPlatform)
            {
                float sectionStartZ = levelStart.position.z + sectionLength * i;
                candidatePosition = previousPosition + new Vector3(Random.Range(-1.5f, 1.5f), 0f, maxJumpDistance * 0.6f);
            }
            GameObject platform = Instantiate(platformPrefab, candidatePosition, Quaternion.identity);
            generatedPlatforms.Add(platform.transform);
            previousPosition = candidatePosition;
        }

        // end platform generation
        bool validEndPlatform = false;
        Vector3 endPosition = Vector3.zero;
        const int maxEndAttempts = 1000;
        int endAttempts = 0;
        while (!validEndPlatform && endAttempts < maxEndAttempts)
        {
            endAttempts++;
            float randomZ = levelEnd.position.z + Random.Range(-sectionLength * 0.3f, sectionLength * 0.3f);
            float randomX = Random.Range(-maxSideOffset, maxSideOffset);
            float randomY = Random.Range(Mathf.Max(minPlatformHeight, previousPosition.y - maxJumpHeight), Mathf.Min(maxPlatformHeight, previousPosition.y + maxJumpHeight));
            endPosition = new Vector3(randomX, randomY, randomZ);
            GameObject tempEnd = Instantiate(endPlatformPrefab, endPosition, Quaternion.identity);
            Collider previousCollider = generatedPlatforms[generatedPlatforms.Count - 1].GetComponent<Collider>();
            Collider endCollider = tempEnd.GetComponent<Collider>();
            Bounds previousBounds = previousCollider.bounds;
            Bounds endBounds = endCollider.bounds;
            Vector3 previousEdge = previousBounds.ClosestPoint(endBounds.center);
            Vector3 endEdge = endBounds.ClosestPoint(previousBounds.center);
            float horizontalGap = Vector2.Distance(new Vector2(previousEdge.x, previousEdge.z), new Vector2(endEdge.x, endEdge.z));
            float verticalDifference = Mathf.Abs(endBounds.max.y - previousBounds.max.y);
            bool physicsDistance = horizontalGap <= maxJumpDistance;
            bool physicsHeight = verticalDifference <= maxJumpHeight;
            bool difficultyDistance = horizontalGap >= minGap && horizontalGap <= maxGap;
            bool difficultyHeight = verticalDifference >= minHeight && verticalDifference <= maxHeight;
            Destroy(tempEnd);
            if (physicsDistance && physicsHeight && difficultyDistance && difficultyHeight)
            {
                validEndPlatform = true;
            }
        }
        GameObject end = Instantiate(endPlatformPrefab, endPosition, Quaternion.identity);
        generatedPlatforms.Add(end.transform);
    }
    public void ClearLevel()
    {
        foreach (Transform platform in generatedPlatforms)
        {
            if (platform != null)
            {
                Destroy(platform.gameObject);
            }
        }
        generatedPlatforms.Clear();
    }
    public List <Transform> GeneratedPlatforms
    {
        get { return generatedPlatforms; }
    }
}
