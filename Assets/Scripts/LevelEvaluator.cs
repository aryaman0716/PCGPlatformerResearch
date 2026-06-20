using UnityEngine;
public class LevelEvaluator : MonoBehaviour
{
    public RandomGenerator generator;
    public ReachabilityValidator validator;
    public LevelMetrics currentMetrics;
    public void EvaluateLevel()
    {
        int reachableJumps = 0;
        int unreachableJumps = 0;
        float totalGap = 0f;
        float totalHeightDifference = 0f;
        var platforms = generator.GeneratedPlatforms; 

        for (int i = 0; i < platforms.Count - 1; i++)
        {
            Transform startPlatform = platforms[i];
            Transform targetPlatform = platforms[i + 1];  
            bool reachable = validator.IsReachable(startPlatform, targetPlatform);  // we check if the next platform is reachable from the current one
            if (reachable)
            {
                reachableJumps++;
            }
            else
            {
                unreachableJumps++;
            }

            Collider startCollider = startPlatform.GetComponent<Collider>();
            Collider targetCollider = targetPlatform.GetComponent<Collider>();
            Bounds startBounds = startCollider.bounds;
            Bounds targetBounds = targetCollider.bounds;
            Vector3 startPoint = startBounds.ClosestPoint(targetBounds.center);
            Vector3 targetPoint = targetBounds.ClosestPoint(startBounds.center);

            float horizontalGap = Vector2.Distance(new Vector2(startPoint.x, startPoint.z), new Vector2(targetPoint.x, targetPoint.z));
            float verticalDifference = targetBounds.max.y - startBounds.max.y;
            totalGap += horizontalGap;
            totalHeightDifference += Mathf.Abs(verticalDifference);

            Debug.Log(
               $"Jump {i + 1} | " +
               $"Gap={horizontalGap:F2}m | " +
               $"HeightDiff={verticalDifference:F2}m | " +
               $"{(reachable ? "Reachable" : "Unreachable")}");
        }

        // we are calculating the metrics for the level based on the jumps evaluated and storing them
        currentMetrics = new LevelMetrics();
        currentMetrics.totalJumps = reachableJumps + unreachableJumps;
        currentMetrics.reachableJumps = reachableJumps;
        currentMetrics.unreachableJumps = unreachableJumps;
        currentMetrics.averageGap = currentMetrics.totalJumps > 0 ? totalGap / currentMetrics.totalJumps : 0f;
        currentMetrics.averageHeightDifference = currentMetrics.totalJumps > 0 ? totalHeightDifference / currentMetrics.totalJumps : 0f;
        currentMetrics.reachabilityPercentage = currentMetrics.totalJumps > 0 ? ((float)reachableJumps / currentMetrics.totalJumps) * 100f : 0f;
        currentMetrics.levelCompletable = unreachableJumps == 0;

        Debug.Log($"Total Jumps: {currentMetrics.totalJumps}");
        Debug.Log($"Reachable Jumps: {currentMetrics.reachableJumps}");
        Debug.Log($"Unreachable Jumps: {currentMetrics.unreachableJumps}");
        Debug.Log($"Reachability: {currentMetrics.reachabilityPercentage:F2}%"); 
        Debug.Log($"Average Gap: {currentMetrics.averageGap:F2}m");
        Debug.Log($"Average Height Difference: {currentMetrics.averageHeightDifference:F2}m");
        Debug.Log($"Level Completable: {currentMetrics.levelCompletable}");
    }
}
