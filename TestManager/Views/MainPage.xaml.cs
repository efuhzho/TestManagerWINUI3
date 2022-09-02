using DKCommunicationNET;
using Microsoft. UI. Xaml. Controls;

using TestManager. ViewModels;

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
        Array. Sort(ViewModel. DsModels);
    }

    private void ToggleSwitch_Toggled (object sender , Microsoft. UI. Xaml. RoutedEventArgs e)
    {
        var toggleSwitch = sender as ToggleSwitch;
        if ( toggleSwitch != null )
        {
            switch ( toggleSwitch. IsOn )
            {
                case true:
                    if ( ViewModel. DsModel != null && ViewModel. Port != null )
                    {
                        ViewModel. Ds = new Dandick(( DKCommunicationNET. Models )Enum. Parse(typeof(DKCommunicationNET. Models) , ViewModel. DsModel));

                        ViewModel. Ds. SerialPortInni(ViewModel. Port , ViewModel. BaudRate);

                        ViewModel. Ds. Open();
                    }
                    break;

                case false:
                    if ( ViewModel. Ds != null )
                    {
                        ViewModel. Ds. Close();
                    }
                    break;
            }
        }
    }
}
