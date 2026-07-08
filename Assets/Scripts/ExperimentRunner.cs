using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
public class ExperimentRunner : MonoBehaviour
{
    public RandomGenerator randomGenerator;
    public ConstraintGenerator constraintGenerator;
    public LevelEvaluator evaluator;
    public int numberOfLevels = 5;
    public float delayBetweenLevels = 0.5f;

    private ExperimentData randomExperiment = new ExperimentData();
    private ExperimentData constraintExperiment = new ExperimentData();

    [Header("Export")]
    public string csvFileName = "ExperimentResults.csv";
    private void Start()
    {
        StartCoroutine(RunExperiments());
    }
    private IEnumerator RunRandomExperiment()
    {
        randomExperiment.generatorName = "Random Generator";
        for (int i = 0; i < numberOfLevels; i++)
        {
            Debug.Log($"Generating Random Level {i + 1}");
            randomGenerator.GenerateLevel();
            yield return null;   // we wait for a frame to ensure the level is generated before evaluation
            LevelMetrics metrics = evaluator.EvaluateLevel(randomGenerator.GeneratedPlatforms);
            metrics.generatorName = "Random Generator";
            metrics.levelNumber = i + 1;
            randomExperiment.levels.Add(metrics);
            yield return new WaitForSeconds(delayBetweenLevels);
            randomGenerator.ClearLevel();
            yield return null;   // wait for a frame to ensure the level is cleared before generating the next one
        }
        Debug.Log("Random Experiment Completed");
    }
    private IEnumerator RunConstraintExperiment()
    {
        constraintExperiment.generatorName = "Constraint Generator";
        for (int i = 0; i < numberOfLevels; i++)
        {
            Debug.Log($"Generating Constraint Level {i + 1}");
            constraintGenerator.GenerateLevel();
            yield return null;   // we wait for a frame to ensure the level is generated before evaluation
            LevelMetrics metrics = evaluator.EvaluateLevel(constraintGenerator.GeneratedPlatforms);
            metrics.generatorName = "Constraint Generator";
            metrics.levelNumber = i + 1;
            constraintExperiment.levels.Add(metrics);
            yield return new WaitForSeconds(delayBetweenLevels);
            constraintGenerator.ClearLevel();
            yield return null;   // wait for a frame to ensure the level is cleared before generating the next one
        }
        Debug.Log("Constraint Experiment Completed");
    }
    private void PrintSummary(ExperimentData experiment)
    {
        float totalReachability = 0;
        float totalGap = 0;
        float totalHeight = 0;
        int completedLevels = 0;

        foreach(LevelMetrics level in experiment.levels)
        {
            totalReachability += level.reachabilityPercentage;
            totalGap += level.averageGap;
            totalHeight += level.averageHeightDifference;
            if(level.levelCompletable)
            {
                completedLevels++;
            }
        }
        int count = experiment.levels.Count;

        Debug.Log(experiment.generatorName);
        Debug.Log($"Levels: {count}");
        Debug.Log($"Average Reachability: {totalReachability / count:F2}%");
        Debug.Log($"Average Gap: {totalGap / count:F2}");
        Debug.Log($"Average Height Difference: {totalHeight / count:F2}");
        Debug.Log($"Completion Rate = {(float)completedLevels / count * 100f:F2}%");
    }
    private IEnumerator RunExperiments()
    {
        yield return StartCoroutine(RunRandomExperiment());
        PrintSummary(randomExperiment);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(RunConstraintExperiment());
        yield return new WaitForSeconds(1f);
        PrintSummary(constraintExperiment);
        ExportResultsToCSV();
        Debug.Log("All Experiments Completed and Results Exported");
    }
    private void ExportResultsToCSV()
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Generator," + "Level," + "TotalJumps," + "ReachableJumps," + "UnreachableJumps," + "ReachabilityPercentage," + "AverageGap," + "MaximumGap," + "AverageHeightDifference," + "MaximumHeightDifference," + "LevelCompletable");
        WriteExperiment(csv, randomExperiment);
        WriteExperiment(csv, constraintExperiment);
        string filePath = Path.Combine(Application.dataPath, csvFileName);
        File.WriteAllText(filePath, csv.ToString());
        Debug.Log($"Results exported to:\n{filePath}");
    }
    private void WriteExperiment(StringBuilder csv, ExperimentData experiment)
    {
        foreach(LevelMetrics level in experiment.levels)
        {
            csv.AppendLine(
            $"{level.generatorName}," +
            $"{level.levelNumber}," +
            $"{level.totalJumps}," +
            $"{level.reachableJumps}," +
            $"{level.unreachableJumps}," +
            $"{level.reachabilityPercentage:F2}," +
            $"{level.averageGap:F2}," +
            $"{level.maximumGap:F2}," +
            $"{level.averageHeightDifference:F2}," +
            $"{level.maximumHeightDifference:F2}," +
            $"{level.levelCompletable}");
        }
    }
}
