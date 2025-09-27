using System.IO;
using System.Text.Json;

namespace SEA1OnTop.Settings
{
     public class AppSettings
     {
          public int BarHeight { get; set; } = 40;                  
          public string BackgroundColor { get; set; } = "#FF0000";  
          public string Text { get; set; } = "Kronichiwa!";
          public bool ScrollText { get; set; } = false;
          public int MonitorIndex { get; set; } = 0;
          public string FontFamilyName { get; set; } = "Segoe UI";
          public int FontSize { get; set; } = 16;

     }

     public static class SettingsManager
     {
          private static readonly string path = Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
              "SEA1OnTop",
              "settings.json");

          public static AppSettings Load()
          {
               if (!File.Exists(path))
               {
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    var defaultSettings = new AppSettings();
                    File.WriteAllText(path, JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true }));
                    return defaultSettings;
               }

               return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path))!;
          }

          public static void Save(AppSettings settings)
          {
               Directory.CreateDirectory(Path.GetDirectoryName(path)!);
               File.WriteAllText(path, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
          }
     }


}
