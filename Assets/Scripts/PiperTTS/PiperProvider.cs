// Adapted from https://github.com/Lyx52/PiperSharp/blob/master/PiperSharp/PiperProvider.cs

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace PiperSharp
{
    public class PiperProvider
    {
        public PiperConfiguration Configuration { get; set; }

        public PiperProvider(PiperConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static Process ConfigureProcess(PiperConfiguration configuration)
        {
            if (configuration.Model is null)
                throw new ArgumentNullException(nameof(PiperConfiguration.Model), "VoiceModel not configured!");

            var exeLoc = configuration.ExecutableLocation?.AddPathQuotesIfRequired() ?? throw new ArgumentNullException(nameof(configuration.ExecutableLocation));

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = exeLoc,
                    Arguments = configuration.BuildArguments(),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = configuration.WorkingDirectory,
                },
            };
        }

        public async Task<byte[]> InferAsync(string text, AudioOutputType outputType = AudioOutputType.Wav, CancellationToken token = default(CancellationToken))
        {
            var process = ConfigureProcess(Configuration);
            process.Start();

            using (var inputWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8))
            {
                await inputWriter.WriteLineAsync(text.ToUtf8());
                await inputWriter.FlushAsync();
            }

            using (var ms = new MemoryStream())
            {
                await process.StandardOutput.BaseStream.CopyToAsync(ms, 81920, token); // Buffer size added for .NET 4.7.2 compatibility
                await process.WaitForExitAsync(token);
                ms.Seek(0, SeekOrigin.Begin);

                using (var fs = new RawSourceWaveStream(ms, new WaveFormat((int)(16000), 1)))
                {
                    return await ConvertToArray(fs, outputType, token);
                }
            }
        }

        private async Task<byte[]> ConvertToArray(RawSourceWaveStream stream, AudioOutputType outputType, CancellationToken token)
        {
            using (var output = new MemoryStream())
            {
                switch (outputType)
                {
                    case AudioOutputType.Mp3:
                        {
                            await stream.FlushAsync(token);
                            MediaFoundationEncoder.EncodeToMp3(stream, output);
                        }
                        break;
                    case AudioOutputType.Raw:
                        {
                            await stream.CopyToAsync(output, 81920, token); // Buffer size added for .NET 4.7.2 compatibility
                            await stream.FlushAsync(token);
                        }
                        break;
                    case AudioOutputType.Wav:
                    default:
                        {
                            using (var waveStream = new WaveFileWriter(output, stream.WaveFormat))
                            {
                                await stream.CopyToAsync(waveStream, 81920, token); // Buffer size added for .NET 4.7.2 compatibility
                                await stream.FlushAsync(token);
                                await waveStream.FlushAsync(token);
                            }
                        }
                        break;
                }
                await output.FlushAsync(token);
                return output.ToArray();
            }
        }
    }
}
