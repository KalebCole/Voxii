// adapted from https://github.com/Lyx52/PiperSharp/blob/master/PiperSharp/Models/PiperConfiguration.cs
#nullable enable
using System.Collections.Generic;

namespace PiperSharp
{
    public class PiperConfiguration
    {
        public string? ExecutableLocation { get; set; }
        public string? WorkingDirectory { get; set; }
        public VoiceModel? Model { get; set; }
        /// <summary>
        /// The speaking rate, lower value is faster, higher value is slower
        /// </summary>
        public float SpeakingRate { get; set; } = 1f;
        public bool UseCuda { get; set; } = false;

        public string BuildArguments()
        {
            if (Model == null || ExecutableLocation == null || WorkingDirectory == null) { throw new System.Exception("Arguments are null"); }
            var args = new List<string>()
            {
                "--quiet",
                "--output-raw",
                $"--model {Model.GetModelLocation()}"
            };
            if (SpeakingRate != 1f)
            {
                var lengthScaleStr = SpeakingRate.ToString("0.00").Replace(',', '.');
                args.Add($"--length_scale {lengthScaleStr}");
            }
            if (UseCuda) args.Add("--use-cuda");
            return string.Join(" ", args);
        }
    }
}
