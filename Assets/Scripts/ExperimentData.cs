using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ExperimentData
{
    public string generatorName;
    public List<LevelMetrics> levels = new List<LevelMetrics>();
}
