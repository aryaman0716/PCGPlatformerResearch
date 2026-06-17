using UnityEngine;
public class ReachabilityValidator : MonoBehaviour 
{
    [Header("Player Constraints")]
    public float maxJumpDistance = 6.4f;
    public float maxJumpHeight = 2f;
    public bool IsReachable(Transform startPlatform, Transform targetPlatform)
    {
        Collider startCollider = startPlatform.GetComponent<Collider>();
        Collider targetCollider = targetPlatform.GetComponent<Collider>();
        if (startCollider == null || targetCollider == null)
        {
            Debug.LogWarning("One of the platforms is missing a collider component.");
            return false;
        }
        Bounds startBounds = startCollider.bounds;
        Bounds targetBounds = targetCollider.bounds;

        Vector3 startPoint = startBounds.ClosestPoint(targetBounds.center);
        Vector3 targetPoint = targetBounds.ClosestPoint(startBounds.center);
        float horizontalGap = Vector2.Distance(new Vector2(startPoint.x, startPoint.z), new Vector2(targetPoint.x, targetPoint.z));

        float startTop = startBounds.max.y;
        float targetTop = targetBounds.max.y;
        float verticalDifference = targetTop - startTop;

        bool reachable = horizontalGap <= maxJumpDistance && verticalDifference <= maxJumpHeight; 
        return reachable;
    }
}
