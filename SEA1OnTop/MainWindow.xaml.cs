using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SEA1OnTop
{
     public partial class MainWindow : Window
     {
          private const int BarHeight = 40;

          public MainWindow()
          {
               InitializeComponent();

               WindowStyle = WindowStyle.None;
               ResizeMode = ResizeMode.NoResize;
               Topmost = true;

               Closing += OnClosing;
               MouseDown += Bar_MouseDown;
          }

          protected override void OnSourceInitialized(EventArgs e)
          {
               base.OnSourceInitialized(e);
               SEAIBarHelper.RegisterBar(this, BarHeight);
          }

          private void Bar_MouseDown(object sender, MouseButtonEventArgs e)
          {
               if (e.MiddleButton == MouseButtonState.Pressed)
               {
                    Close();
               }
               else if(e.RightButton == MouseButtonState.Pressed)
               {
                    var settings = new SettingsWindow
                    {
                         Owner = this
                    };
                    settings.ShowDialog();
               }
          }

          private void OnClosing(object? sender, CancelEventArgs e)
          {
               SEAIBarHelper.UnregisterBar(this);
          }
     }
}