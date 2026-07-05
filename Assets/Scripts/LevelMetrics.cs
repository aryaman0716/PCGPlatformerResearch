using UnityEngine;
[System.Serializable]
public class LevelMetrics
{
    public int totalJumps;
    public int reachableJumps;
    public int unreachableJumps;
    public int levelNumber;
    public string generatorName;
    public float reachabilityPercentage;
    public float averageGap;
    public float averageHeightDifference;
    public float maximumGap;
    public float maximumHeightDifference;
    public bool levelCompletable;
}
