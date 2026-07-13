using UnityEngine;
public class LevelReplayer : MonoBehaviour
{
    public RandomGenerator randomGenerator;
    public ConstraintGenerator constraintGenerator;
    public int replaySeed;
    //private void Start()
    //{
    //    ReplayRandomLevel();
    //}
    public void ReplayRandomLevel()
    {
        randomGenerator.GenerateLevel(replaySeed);
        Debug.Log($"Replaying Random Level with seed {replaySeed}");
    }
    public void ReplayConstraintLevel()
    {
        constraintGenerator.GenerateLevel(replaySeed);
        Debug.Log($"Replaying Constraint Level with seed {replaySeed}");
    }
}
