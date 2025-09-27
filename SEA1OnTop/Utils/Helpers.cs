using System.Windows.Media;

namespace SEA1OnTop.Utils
{
     public static class Helpers
     {
          public static Screen GetScreen(int monitorIndex)
          {
               var screens = Screen.AllScreens;
               int currentMonitorIndex = Math.Min(monitorIndex, screens.Length - 1);
               Screen? screen = screens[currentMonitorIndex];

               return screen;
          }

          public static System.Windows.Media.Color FromHSV(double hue, double saturation, double value)
          {
               int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
               double f = hue / 60 - Math.Floor(hue / 60);

               value = value * 255;
               byte v = (byte)value;
               byte p = (byte)(value * (1 - saturation));
               byte q = (byte)(value * (1 - f * saturation));
               byte t = (byte)(value * (1 - (1 - f) * saturation));

               return hi switch
               {
                    0 => System.Windows.Media.Color.FromRgb(v, t, p),
                    1 => System.Windows.Media.Color.FromRgb(q, v, p),
                    2 => System.Windows.Media.Color.FromRgb(p, v, t),
                    3 => System.Windows.Media.Color.FromRgb(p, q, v),
                    4 => System.Windows.Media.Color.FromRgb(t, p, v),
                    _ => System.Windows.Media.Color.FromRgb(v, p, q),
               };
          }
     }
}
