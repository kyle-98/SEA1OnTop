using System.IO;
using System.Text.Json;

namespace SEA1OnTop
{
     public class SettingsHelper
     {
          public int BarHeight { get; set; } = 40;                  
          public string BackgroundColor { get; set; } = "#FF0000";  
          public string Text { get; set; } = "Kronichiwa!";            
     }

     public static class SettingsManager
     {
          private static readonly string path = Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
              "SEA1OnTop",
              "settings.json");

          public static SettingsHelper Load()
          {
               if (!File.Exists(path))
               {
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    var defaultSettings = new SettingsHelper();
                    File.WriteAllText(path, JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true }));
                    return defaultSettings;
               }

               return JsonSerializer.Deserialize<SettingsHelper>(File.ReadAllText(path))!;
          }

          public static void Save(SettingsHelper settings)
          {
               Directory.CreateDirectory(Path.GetDirectoryName(path)!);
               File.WriteAllText(path, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
          }
     }


}
