using UnityEngine;
[System.Serializable]
public class LevelMetrics
{
    public int totalJumps;
    public int reachableJumps;
    public int unreachableJumps;
    public float reachabilityPercentage;
    public float averageGap;
    public float averageHeightDifference;
    public bool levelCompletable;
}
