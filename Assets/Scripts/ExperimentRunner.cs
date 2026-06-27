using UnityEngine;
public class ExperimentRunner : MonoBehaviour
{
    public RandomGenerator randomGenerator;
    public ConstraintGenerator constraintGenerator;
    public LevelEvaluator evaluator;
    public int numberOfLevels = 5;

    private ExperimentData randomExperiment = new ExperimentData();
    private ExperimentData constraintExperiment = new ExperimentData();

    private void Start()
    {
        RunRandomExperiment();
    }
    private void RunRandomExperiment()
    {
        randomExperiment.generatorName = "Random Generator";
        for (int i = 0; i < numberOfLevels; i++)
        {
            Debug.Log($"Generating Random Level {i + 1}");
            randomGenerator.GenerateLevel();
            LevelMetrics metrics = evaluator.EvaluateLevel(randomGenerator.GeneratedPlatforms);
            metrics.generatorName = "Random Generator";
            metrics.levelNumber = i + 1;
            randomExperiment.levels.Add(metrics);
        }
        Debug.Log("Random Experiment Completed");
    }
}
