using UnityEngine;
public class ReachabilityValidator : MonoBehaviour 
{
    [Header("Player Constraints")]
    public float maxJumpDistance = 6.4f;
    public float maxJumpHeight = 2f;
    public bool IsReachable(Vector3 startPos, Vector3 targetPos)
    {
        float horizontalDistance = Vector2.Distance(new Vector2(startPos.x, startPos.z), new Vector2(targetPos.x, targetPos.z));
        float verticalDifference = targetPos.y - startPos.y;
        if (horizontalDistance > maxJumpDistance)
        {
            return false;
        }
        if (verticalDifference > maxJumpHeight)
        {
            return false;
        }
        return true;
    }
}
