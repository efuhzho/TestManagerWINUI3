using DKCommunicationNET;
using Microsoft. UI. Xaml. Controls;

using TestManager. ViewModels;
using Windows. UI. Popups;

namespace TestManager. Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }
    public MainPage ()
    {
        ViewModel = App. GetService<MainViewModel>();
        InitializeComponent();
        Array. Sort(ViewModel. DandickModels);
    }


    private void ToggleSwitch_SS_Toggled (object sender , Microsoft. UI. Xaml. RoutedEventArgs e)
    {       
        ViewModel. ToggleSwitch_Toggled(ToggleSwitch_SS. IsOn);
        if ( ViewModel. IsOn_PortSwitch )
        {
            ToggleSwitch_SS. IsOn = true;
        }
        else
        {
            ToggleSwitch_SS. IsOn = false;
        }       
    }
}
