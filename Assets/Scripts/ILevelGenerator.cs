using UnityEngine;
using System.Collections.Generic;
public interface ILevelGenerator
{
    void GenerateLevel(int? seed = null);
    List<Transform> GeneratedPlatforms { get; }
}
