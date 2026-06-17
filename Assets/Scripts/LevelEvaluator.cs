using UnityEngine;
public class LevelEvaluator : MonoBehaviour
{
    public RandomGenerator generator;
    public ReachabilityValidator validator;
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
        int totalJumps = reachableJumps + unreachableJumps;
        float averageGap = totalJumps > 0 ? totalGap / totalJumps : 0f;
        float averageHeightDifference = totalJumps > 0 ? totalHeightDifference / totalJumps : 0f;
        float reachabilityPercentage = totalJumps > 0 ? ((float)reachableJumps / totalJumps) * 100f : 0f;
        bool levelCompletable = unreachableJumps == 0; 
        Debug.Log($"Total Jumps: {totalJumps}");
        Debug.Log($"Reachable Jumps: {reachableJumps}");
        Debug.Log($"Unreachable Jumps: {unreachableJumps}");
        Debug.Log($"Reachability: {reachabilityPercentage:F2}%"); 
        Debug.Log($"Average Gap: {averageGap:F2}m");
        Debug.Log($"Average Height Difference: {averageHeightDifference:F2}m");
        Debug.Log($"Level Completable: {levelCompletable}");
    }
}
