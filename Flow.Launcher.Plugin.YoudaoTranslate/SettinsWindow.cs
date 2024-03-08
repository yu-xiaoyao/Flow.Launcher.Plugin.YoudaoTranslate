using System.Windows;
using System.Windows.Input;

namespace Flow.Launcher.Plugin.YoudaoTranslate;

public class SettinsWindow : Window
{
    public SettinsWindow(PluginInitContext context, Settings settings)
    {
        // Width = 840;
        // Height = 260;
        ResizeMode = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = true;
        ShowInTaskbar = false;
        // WindowStyle = WindowStyle.None;
        Content = new SettingsControlPanel(context, settings);
        KeyDown += Esc_Exit_KeyDown;
    }

    private void Esc_Exit_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}