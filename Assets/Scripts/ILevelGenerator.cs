using UnityEngine;
using System.Collections.Generic;
public interface ILevelGenerator
{
    void GenerateLevel();
    List<Transform> GeneratedPlatforms { get; }
}
