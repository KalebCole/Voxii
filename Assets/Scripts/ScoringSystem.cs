using System.Diagnostics;
using UnityEngine;

/**
Scoring system for the conversation evaluation task.
*/
public static class ScoringSystem
{
    public static int CalculatePoints(ScoreResult scoreResult)
    {
        UnityEngine.Debug.Log("Calculating points...");
        UnityEngine.Debug.Log(scoreResult);
        
        int points = 0;

        // Example scoring logic
        points += (10 - scoreResult.NumberOfErrors) * 5; // Fewer errors, more points
        points += scoreResult.Accuracy * 10; // Higher accuracy, more points
        // points += (int)((10 - scoreResult.AverageResponseTime) * 2); // Faster responses, more points

        return points;
    }
}