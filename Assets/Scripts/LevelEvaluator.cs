using UnityEngine;
public class LevelEvaluator : MonoBehaviour
{
    public RandomGenerator generator;
    public ReachabilityValidator validator;
    private void Start()
    {

    }
    public void EvaluateLevel()
    {
        int reachableJumps = 0;
        int unreachableJumps = 0;
        var platforms = generator.GeneratedPlatforms; 

        for (int i = 0; i < platforms.Count - 1; i++)
        {
            bool reachable = validator.IsReachable(platforms[i].position, platforms[i + 1].position);  // we check if the next platform is reachable from the current one
            if (reachable)
            {
                reachableJumps++;
            }
            else
            {
                unreachableJumps++;
            }

            float horizontalDistance = Vector2.Distance(new Vector2(platforms[i].position.x, platforms[i].position.z), new Vector2(platforms[i + 1].position.x, platforms[i + 1].position.z));
            float verticalDifference = platforms[i + 1].position.y - platforms[i].position.y;

            Debug.Log($"Jump {i + 1} | " + $"Distance={horizontalDistance:F2}m | " + $"HeightDiff={verticalDifference:F2}m | " + $"{(reachable ? "Reachable" : "Unreachable")}");
        }
        Debug.Log($"Reachable Jumps: {reachableJumps}");
        Debug.Log($"Unreachable Jumps: {unreachableJumps}");
    }
}
