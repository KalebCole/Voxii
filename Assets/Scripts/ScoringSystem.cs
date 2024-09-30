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


        // Points for number of errors
        switch (scoreResult.NumberOfErrors)
        {
            case 0:
                points += 100;
                break;
            case 1:
                points += 80;
                break;
            case 2:
                points += 60;
                break;
            case 3:
                points += 40;
                break;
            case 4:
                points += 20;
                break;
            case 5:
                points += 10;
                break;
            default:
                points += 0;
                break;
        }

        // Points for accuracy
        switch (scoreResult.Accuracy)
        {
            case 0:
                points += 0;
                break;
            case 1:
                points += 10;
                break;
            case 2:
                points += 20;
                break;
            case 3:
                points += 30;
                break;
            case 4:
                points += 40;
                break;
            case 5:
                points += 50;
                break;
            case 6:
                points += 60;
                break;
            case 7:
                points += 70;
                break;
            case 8:
                points += 80;
                break;
            case 9:
                points += 90;
                break;
            case 10:
                points += 100;
                break;
            default:
                points += 0;
                break;
        }



        // points += (int)((10 - scoreResult.AverageResponseTime) * 2); // Faster responses, more points

        return points;
    }
}