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
     }
}
