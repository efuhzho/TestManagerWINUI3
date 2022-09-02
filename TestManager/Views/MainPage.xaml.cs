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
        Array. Sort(ViewModel. DsModels);
    }

    private void ToggleSwitch_Toggled (object sender , Microsoft. UI. Xaml. RoutedEventArgs e)
    {
        try
        {
            var toggleSwitch = sender as ToggleSwitch;
            if ( toggleSwitch != null )
            {
                switch ( toggleSwitch. IsOn )
                {
                    case true:
                        ViewModel. DisableSerialPortEdit = false;
                        if ( ViewModel. DsModel != null && ViewModel. Port != null )
                        {
                            ViewModel. DS = new Dandick(( DKCommunicationNET. Models )Enum. Parse(typeof(DKCommunicationNET. Models) , ViewModel. DsModel));

                            ViewModel. DS. SerialPortInni(ViewModel. Port , ViewModel. BaudRate);

                            ViewModel. DS. Open();
                        }
                        break;

                    case false:
                        ViewModel. DisableSerialPortEdit = true;
                        if ( ViewModel. DS != null )
                        {
                            ViewModel. DS. Close();
                        }
                        break;
                }
            }
        }
        catch ( Exception ex)
        {

            
        }
      
    }
}
