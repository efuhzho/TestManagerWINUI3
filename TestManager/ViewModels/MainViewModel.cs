using CommunityToolkit. Mvvm. ComponentModel;
using Windows. UI. Popups;
using WinUIEx. Messaging;
using System. Windows;
using Microsoft. UI. Xaml. Controls;
using Windows. Foundation. Metadata;
using CommunityToolkit. Mvvm. Input;
using System. Windows. Input;
using TestManager. Core. Contracts. Services;
using TestManager. Core. Services;
using TestManager. Core. Models;
using System. IO. Ports;
using Windows. Devices. SerialCommunication;
using DKCommunicationNET;

namespace TestManager. ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel ()
    {
        port = ports[0];
        baudRate = baudRates[2];
    }
    [ObservableProperty]
    private Dandick? dS;

    [ObservableProperty]
    private string? dsModel;

    [ObservableProperty]
    private string? port;

    [ObservableProperty]
    private int baudRate;

    #region ComboBox初始化

    [ObservableProperty]
    private string[] dsModels = Enum. GetNames(typeof(DKCommunicationNET. Models));

    [ObservableProperty]
    private string[]? ports = SerialPort. GetPortNames();

    [ObservableProperty]
    private int[]? baudRates = new int[] { 9600 , 19200 , 115200 };

    [ObservableProperty]
    private string[] paritys = Enum. GetNames(typeof(Parity));

    [ObservableProperty]
    private string[] stopBits = Enum. GetNames(typeof(StopBits));

    [ObservableProperty]
    private string[] handShakes = Enum. GetNames(typeof(Handshake));

    [ObservableProperty]
    private int[] dataBits = new int[] { 5 , 6 , 7 , 8 };

    [ObservableProperty]
    private string? model;
    #endregion
    /// <summary>
    /// 指示打开串口后禁用的状态
    /// </summary>
    [ObservableProperty]
    private bool disableSerialPortEdit = true;

    [RelayCommand]
    private void ToggleSwitch_Toggled (bool isOn)
    {
        try
        {
            switch ( isOn )
            {
                case true:
                    DisableSerialPortEdit = false;
                    if ( dsModel != null && port != null )
                    {
                        DS = new Dandick(( DKCommunicationNET. Models )Enum. Parse(typeof(DKCommunicationNET. Models) , dsModel));
                        DS. SerialPortInni(port , baudRate);
                        DS. Open();

                        Model = DS. Model;
                    }
                    break;

                case false:
                    DisableSerialPortEdit = true;
                    if ( DS != null )
                    {
                        DS. Close();
                    }
                    break;
            }
        }
        catch ( Exception ex )
        {

        }

    }

}
