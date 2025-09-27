using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SEA1OnTop
{
     public partial class MainWindow : Window
     {
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

               var settings = SettingsManager.Load();

               SEAIBarHelper.RegisterBar(this, settings.BarHeight);
               try
               {
                    Background = (SolidColorBrush)new BrushConverter().ConvertFromString(settings.BackgroundColor);
               }
               catch
               {
                    Background = System.Windows.Media.Brushes.Red;
               }

               var textBlock = new TextBlock
               {
                    Text = settings.Text,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 16
               };
               Content = textBlock;
          }


          private void Bar_MouseDown(object sender, MouseButtonEventArgs e)
          {
               if(e.RightButton == MouseButtonState.Pressed)
               {
                    ShowContextMenu(e);
               }
          }


          private void ShowContextMenu(MouseButtonEventArgs e)
          {
               var contextMenu = new ContextMenu();

               var settingsItem = new MenuItem { Header = "Settings" };
               settingsItem.Click += (s, args) =>
               {
                    var settings = new SettingsWindow(this)
                    {
                         Owner = this
                    };
                    settings.ShowDialog();
               };
               contextMenu.Items.Add(settingsItem);

               var exitItem = new MenuItem { Header = "Exit" };
               exitItem.Click += (s, args) =>
               {
                    Close();
               };
               contextMenu.Items.Add(exitItem);

               contextMenu.IsOpen = true;
          }

          private void OnClosing(object? sender, CancelEventArgs e)
          {
               SEAIBarHelper.UnregisterBar(this);
          }


          public void ApplySettings(SettingsHelper settings, int? oldBarHeight = null)
          {
               try
               {
                    Background = (SolidColorBrush)new BrushConverter().ConvertFromString(settings.BackgroundColor);
               }
               catch
               {
                    Background = System.Windows.Media.Brushes.Red;
               }

               if (Content is TextBlock textBlock)
               {
                    textBlock.Text = settings.Text;
               }
               else
               {
                    Content = new TextBlock
                    {
                         Text = settings.Text,
                         VerticalAlignment = VerticalAlignment.Center,
                         HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                         Foreground = System.Windows.Media.Brushes.White,
                         FontSize = 16
                    };
               }

               if (oldBarHeight.HasValue && oldBarHeight.Value != settings.BarHeight)
               {
                    System.Windows.MessageBox.Show("Bar height changes require restarting the application to apply.", "Restart Required", MessageBoxButton.OK, MessageBoxImage.Information);
               }
          }
     }
}