using SEA1OnTop.Settings;
using SEA1OnTop.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SEA1OnTop
{
     public partial class MainWindow : Window
     {
          private double _textPosition = 0;
          private TextBlock _textBlock;
          private Canvas _canvas;
          private AppSettings _currentSettings;
          private DateTime _lastDateUpdate = DateTime.MinValue;
          private DispatcherTimer _dateTimer;
          private NotifyIcon? _trayIcon;


          public MainWindow()
          {
               InitializeComponent();

               WindowStyle = WindowStyle.None;
               ResizeMode = ResizeMode.NoResize;
               Topmost = true;
               FocusVisualStyle = null;
               AllowsTransparency = false;

               Closing += OnClosing;
               MouseDown += Bar_MouseDown;

               _dateTimer = new DispatcherTimer
               {
                    Interval = TimeSpan.FromSeconds(1)
               };
               _dateTimer.Tick += (s, e) => UpdateTextBlockContent();
               _dateTimer.Start();

               _trayIcon = new NotifyIcon
               {
                    Icon = new Icon("Assets/sea1_logo.ico"), 
                    Visible = true,
                    Text = "SEA1OnTop"
               };

               var trayMenu = new ContextMenuStrip();
               trayMenu.Items.Add("Settings", null, (s, e) =>
               {
                    var settings = new SettingsWindow(this)
                    {
                         Owner = this
                    };
                    settings.ShowDialog();
               });
               trayMenu.Items.Add("Exit", null, (s, e) => CloseApplication());
               _trayIcon.ContextMenuStrip = trayMenu;
               ShowInTaskbar = false;
          }


          protected override void OnSourceInitialized(EventArgs e)
          {
               base.OnSourceInitialized(e);

               var settings = SettingsManager.Load();

               ApplySettings(settings);
          }

          private void CloseApplication() => System.Windows.Application.Current.Shutdown();


          private void Bar_MouseDown(object sender, MouseButtonEventArgs e)
          {
               if(e.RightButton == MouseButtonState.Pressed)
               {
                    ShowContextMenu(e);
               }
          }

          private void OnClosing(object? sender, CancelEventArgs e)
          {
               SEAIBarHelper.UnregisterBar(this);
               if (_trayIcon != null)
               {
                    _trayIcon.Visible = false;
                    _trayIcon.Dispose();
                    _trayIcon = null;
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

          
          public void ApplySettings(AppSettings settings)
          {
               _currentSettings = settings;

               try { Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_currentSettings.BackgroundColor); }
               catch { Background = System.Windows.Media.Brushes.Red; }

               _textBlock = new TextBlock
               {
                    Text = _currentSettings.Text,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(_currentSettings.TextColor),
                    FontSize = _currentSettings.FontSize,
                    FontFamily = new System.Windows.Media.FontFamily(_currentSettings.FontFamilyName)
               };
               

               if (_currentSettings.ScrollText)
               {
                    _canvas = new Canvas();
                    _canvas.Children.Add(_textBlock);
                    Content = _canvas;

                    _textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    double textWidth = _textBlock.DesiredSize.Width;

                    if (_currentSettings.TextScrollDirection == ScrollDirection.RightToLeft)
                    {
                         _textPosition = ActualWidth;
                    }
                    else
                    {
                         _textPosition = -textWidth;
                    }

                    // Detatch previous handler to prevent handlers from stacking when changing settings
                    CompositionTarget.Rendering -= ScrollTextFrame;
                    CompositionTarget.Rendering += ScrollTextFrame;
               }
               else
               {
                    Content = _textBlock;
                    _textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    _textBlock.VerticalAlignment = VerticalAlignment.Center;
               }

               UpdateTextBlockContent();

               Screen? screen = Helpers.GetScreen(_currentSettings.MonitorIndex);

               SEAIBarHelper.RegisterBar(this, _currentSettings.BarHeight, screen.Bounds.Left, screen.Bounds.Top, screen.Bounds.Width);
          }



          private void UpdateTextBlockContent()
          {
               string text = _currentSettings.Text ?? string.Empty;

               // Match %date-FORMAT|TIMEZONE%

               /*
                * Examples:
                *   %date-hh:mm tt|Eastern Standard Time%
                *   %date-HH:mm:ss|Pacific Standard Time%
                *   %date-yyyy-MM-dd HH:mm:ss|UTC%
                */
               var matches = System.Text.RegularExpressions.Regex.Matches(text, "%date-(.*?)%");

               foreach (System.Text.RegularExpressions.Match match in matches)
               {
                    string inside = match.Groups[1].Value;
                    string format = inside;
                    string? timeZoneId = null;

                    if (inside.Contains('|'))
                    {
                         var parts = inside.Split('|');
                         format = parts[0];
                         timeZoneId = parts.Length > 1 ? parts[1] : null;
                    }

                    DateTime dateTime = DateTime.Now;

                    if (!string.IsNullOrEmpty(timeZoneId))
                    {
                         try
                         {
                              var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                              dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, tz);
                         }
                         catch { }
                         
                    }

                    string replacement;
                    try
                    {
                         replacement = dateTime.ToString(format);
                    }
                    catch
                    {
                         replacement = match.Value;
                    }

                    text = text.Replace(match.Value, replacement);
               }

               if (text.Contains("%date%"))
               {
                    text = text.Replace("%date%", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
               }

               _textBlock.Text = text;
          }


          private void ScrollTextFrame(object? sender, EventArgs e)
          {
               if (_textBlock == null || _canvas == null) return;

               if ((DateTime.Now - _lastDateUpdate).TotalSeconds >= 1)
               {
                    _lastDateUpdate = DateTime.Now;
                    UpdateTextBlockContent();
               }

               double speed = 2;
               if (_currentSettings.TextScrollDirection == ScrollDirection.RightToLeft)
               {
                    _textPosition -= speed;
                    if (_textPosition < -_textBlock.DesiredSize.Width)
                         _textPosition = ActualWidth;
               }
               else 
               {
                    _textPosition += speed;
                    if (_textPosition > ActualWidth)
                         _textPosition = -_textBlock.DesiredSize.Width;
               }

               Canvas.SetLeft(_textBlock, _textPosition);
               Canvas.SetTop(_textBlock, (ActualHeight - _textBlock.DesiredSize.Height) / 2);
          }
     }
}