using SEA1OnTop.Settings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SEA1OnTop
{
     public partial class SettingsWindow : Window
     {
          private AppSettings _settings;
          private MainWindow _barWindow;

          public SettingsWindow(MainWindow barWindow)
          {
               InitializeComponent();
               _barWindow = barWindow;

               _settings = SettingsManager.Load();
               BarHeightTextBox.Text = _settings.BarHeight.ToString();
               BackgroundColorTextBox.Text = _settings.BackgroundColor;
               DisplayTextBox.Text = _settings.Text;
               ScrollTextCheckBox.IsChecked = _settings.ScrollText;
               RGBBackgroundCheckBox.IsChecked = _settings.UseRgbBackground;
               FontSizeTextBox.Text = _settings.FontSize.ToString();
               TextColorTextBox.Text = _settings.TextColor.ToString();
               ScrollDirectionComboBox.SelectedIndex = _settings.TextScrollDirection == ScrollDirection.RightToLeft ? 0 : 1;

               PopulateMonitorDropdown();
               PopulateFontComboBox();
          }

          private void SaveButton_Click(object sender, RoutedEventArgs e)
          {
               try
               {
                    if (!int.TryParse(BarHeightTextBox.Text, out int barHeight) || barHeight < 10)
                         throw new Exception("Bar height must be a number >= 10");

                    int oldHeight = _settings.BarHeight;

                    _settings.BarHeight = barHeight;
                    _settings.BackgroundColor = BackgroundColorTextBox.Text;
                    _settings.Text = DisplayTextBox.Text;
                    _settings.ScrollText = ScrollTextCheckBox.IsChecked == true;

                    if (MonitorComboBox.SelectedItem is ComboBoxItem selectedItem)
                    {
                         _settings.MonitorIndex = MonitorComboBox.SelectedIndex;
                    }

                    if (ScrollDirectionComboBox.SelectedItem is ComboBoxItem item &&
                         Enum.TryParse<ScrollDirection>(item.Tag.ToString(), out var direction))
                    {
                         _settings.TextScrollDirection = direction;
                    }

                    _settings.UseRgbBackground = RGBBackgroundCheckBox.IsChecked == true;

                    _settings.FontFamilyName = FontComboBox.Text;
                    _settings.TextColor = TextColorTextBox.Text;

                    if (!int.TryParse(FontSizeTextBox.Text, out int size))
                         size = 16;
                    _settings.FontSize = Math.Max(1, size);

                    SettingsManager.Save(_settings);

                    _barWindow.ApplySettings(_settings);

                    System.Windows.MessageBox.Show("Settings saved successfully!");

                    
               }
               catch (Exception ex)
               {
                    System.Windows.MessageBox.Show($"Error saving settings: {ex.Message}");
               }
          }

          private void ResetButton_Click(object sender, RoutedEventArgs e)
          {
               _settings.BarHeight = 40;
               _settings.BackgroundColor = "#FF0000";
               _settings.Text = "Kronichiwa!";
               _settings.ScrollText = false;
               _settings.MonitorIndex = 0;
               _settings.FontFamilyName = "Segoe UI";
               _settings.FontSize = 16;
               _settings.TextColor = "#FFFFFFFF";
               _settings.TextScrollDirection = ScrollDirection.RightToLeft;
               _settings.UseRgbBackground = false;

               SettingsManager.Save(_settings);
               _barWindow.ApplySettings(_settings);

               BarHeightTextBox.Text = _settings.BarHeight.ToString();
               BackgroundColorTextBox.Text = _settings.BackgroundColor;
               DisplayTextBox.Text = _settings.Text;
               ScrollTextCheckBox.IsChecked = _settings.ScrollText;
               RGBBackgroundCheckBox.IsChecked = _settings.UseRgbBackground;
               FontSizeTextBox.Text = _settings.FontSize.ToString();
               TextColorTextBox.Text = _settings.TextColor.ToString();
               ScrollDirectionComboBox.SelectedIndex = _settings.TextScrollDirection == ScrollDirection.RightToLeft ? 0 : 1;
               
               System.Windows.MessageBox.Show("Successfully reset settings");
          }

          private void CloseButton_Click(object sender, RoutedEventArgs e)
          {
               Close();
          }

          private void ChooseColorButton_Click(object sender, RoutedEventArgs e)
          {
               var dialog = new ColorDialog();

               try
               {
                    var currentColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(BackgroundColorTextBox.Text);
                    dialog.Color = System.Drawing.Color.FromArgb(currentColor.A, currentColor.R, currentColor.G, currentColor.B);
               }
               catch
               {
                    dialog.Color = System.Drawing.Color.Red;
               }

               if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
               {
                    var color = dialog.Color;
                    BackgroundColorTextBox.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
               }
          }

          private void PopulateMonitorDropdown()
          {
               MonitorComboBox.Items.Clear();

               var screens = Screen.AllScreens;
               for (int i = 0; i < screens.Length; i++)
               {
                    var screen = screens[i];
                    string name = screen.DeviceName;
                    string info = $"{name} ({screen.Bounds.Width}x{screen.Bounds.Height})";

                    MonitorComboBox.Items.Add(new ComboBoxItem
                    {
                         Content = info,
                         Tag = screen
                    });
               }
               try { MonitorComboBox.SelectedIndex = _settings.MonitorIndex; }
               catch { MonitorComboBox.SelectedIndex = 0; }
          }

          private void PopulateFontComboBox()
          {
               FontComboBox.Items.Clear();
               foreach (var fontFamily in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
               {
                    FontComboBox.Items.Add(fontFamily.Source);
               }

               FontComboBox.Text = _settings.FontFamilyName;
          }

          private void PickTextColor_Click(object sender, RoutedEventArgs e)
          {
               var cd = new ColorDialog();
               if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
               {
                    var color = cd.Color;
                    TextColorTextBox.Text = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
               }
          }
     }
}
