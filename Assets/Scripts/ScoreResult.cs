using System.Text.RegularExpressions;

public class ScoreResult
{
    public int NumberOfErrors { get; set; }
    public int Accuracy { get; set; }
    // public float AverageResponseTime { get; set; }

    public static ScoreResult Parse(string scoreString)
    {
        var result = new ScoreResult();

        // Regular expressions to extract numbers
        var errorsMatch = Regex.Match(scoreString, @"Number of errors:\s*(\d+)");
        var accuracyMatch = Regex.Match(scoreString, @"Accuracy of understanding and responding:\s*(\d+)");
        // var timeMatch = Regex.Match(scoreString, @"Average time for response:\s*([\d\.]+)");

        if (errorsMatch.Success)
            result.NumberOfErrors = int.Parse(errorsMatch.Groups[1].Value);

        if (accuracyMatch.Success)
            result.Accuracy = int.Parse(accuracyMatch.Groups[1].Value);

        // if (timeMatch.Success)
        //     result.AverageResponseTime = float.Parse(timeMatch.Groups[1].Value);

        return result;
    }
}
