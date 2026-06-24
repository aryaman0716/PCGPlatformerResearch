using UnityEngine;
public class GeneratorManager : MonoBehaviour
{
    public RandomGenerator randomGenerator;
    public ConstraintGenerator constraintGenerator;
    public LevelEvaluator evaluator;
    public enum GeneratorType
    {
        Random,
        Constraint
    }
    public GeneratorType generatorType;
    private void Start()
    {
        RunTest();
    }
    private void RunTest()
    {
        ILevelGenerator generator;
        if (generatorType == GeneratorType.Random)
        {
            generator = randomGenerator;
        }
        else
        {
            generator = constraintGenerator;
        }
        generator.GenerateLevel();
        LevelMetrics metrics = evaluator.EvaluateLevel(generator.GeneratedPlatforms);
        Debug.Log($"Reachability: " + $"{metrics.reachabilityPercentage:F2}%");
    }
}
