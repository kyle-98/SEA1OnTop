using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace SEA1OnTop
{
     public partial class SettingsWindow : Window
     {
          private SettingsHelper _settings;
          private MainWindow _barWindow;

          public SettingsWindow(MainWindow barWindow)
          {
               InitializeComponent();
               _barWindow = barWindow;

               _settings = SettingsManager.Load();
               BarHeightTextBox.Text = _settings.BarHeight.ToString();
               BackgroundColorTextBox.Text = _settings.BackgroundColor;
               DisplayTextBox.Text = _settings.Text;
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

                    SettingsManager.Save(_settings);

                    _barWindow.ApplySettings(_settings, oldHeight);

                    System.Windows.MessageBox.Show("Settings saved successfully!");
               }
               catch (Exception ex)
               {
                    System.Windows.MessageBox.Show($"Error saving settings: {ex.Message}");
               }
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
     }
}
