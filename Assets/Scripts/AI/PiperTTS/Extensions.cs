// Take from https://github.com/Lyx52/PiperSharp/blob/master/PiperSharp/Extensions.cs
#nullable enable
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiperSharp
{
    public static class Extensions
    {
        public static string ToUtf8(this string text)
        {
            return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
        }

        public static string AddPathQuotesIfRequired(this string text)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return text;
            var sb = new StringBuilder();
            if (!text.StartsWith("\"")) sb.Append('"');
            sb.Append(text);
            if (!text.EndsWith("\"")) sb.Append('"');

            return sb.ToString();
        }

        /// <summary>
        /// Waits asynchronously for the process to exit.
        /// </summary>
        /// <param name="process">The process to wait for cancellation.</param>
        /// <param name="cancellationToken">A cancellation token. If invoked, the task will return 
        /// immediately as canceled.</param>
        /// <returns>A Task representing waiting for the process to end.</returns>
        public static Task WaitForExitAsync(this Process process,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (process.HasExited) return Task.FromResult(true); // Changed for .NET 4.7.2 compatibility

            var tcs = new TaskCompletionSource<object?>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(tcs.SetCanceled);

            return tcs.Task;
        }
    }
}
