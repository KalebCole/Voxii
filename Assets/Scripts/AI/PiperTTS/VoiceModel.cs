// Referred from https://github.com/Lyx52/PiperSharp/blob/master/PiperSharp/Models/VoiceModel.cs
#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PiperSharp
{
    public class VoiceModel
    {

        [JsonProperty("files")]
        public Dictionary<string, dynamic> Files { get; set; } = new Dictionary<string, dynamic>();

        [JsonIgnore]
        public string? ModelLocation { get; set; }

        public string GetModelLocation()
        {
            if (ModelLocation == null) throw new FileNotFoundException("Model not downloaded!");
            var modelFileName = Path.GetFileName(Files.Keys.FirstOrDefault(f => f.EndsWith(".onnx")));
            return Path.Combine(ModelLocation, modelFileName).AddPathQuotesIfRequired();
        }

        public static Task<VoiceModel> LoadModelByKey(string modelLoc, string modelKey)
            => LoadModel(Path.Combine(modelLoc, modelKey));

        public static async Task<VoiceModel> LoadModel(string directory)
        {
            if (!Directory.Exists(directory)) throw new DirectoryNotFoundException("Model directory not found!");
            var modelInfoFile = Path.Combine(directory, "model.json");
            if (!File.Exists(modelInfoFile)) throw new FileNotFoundException("model.json file not found!");

            var modelInfo = await Task.Run(() => File.ReadAllText(modelInfoFile));
            var model = JsonConvert.DeserializeObject<VoiceModel>(modelInfo);

            if (model is null) throw new ApplicationException("Could not parse model.json file!");
            model.ModelLocation = directory;
            return model;
        }
    }
}
