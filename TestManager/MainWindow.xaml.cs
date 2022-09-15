using TestManager.Helpers;

namespace TestManager;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "StoreLogo.scale-400.png"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
    }
   
}
