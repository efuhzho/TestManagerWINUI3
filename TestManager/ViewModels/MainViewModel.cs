using System. IO. Ports;
using CommunityToolkit. Mvvm. ComponentModel;
using CommunityToolkit. Mvvm. Input;
using DKCommunicationNET;
using Microsoft. UI. Xaml. Controls;

namespace TestManager. ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel ()
    {
        portName = ports[0];
        baudRate = baudRates[2];
    }

    #region 源串口打开Infobar绑定
    /// <summary>
    /// 串口打开进度
    /// </summary>
    [ObservableProperty]
    private bool isInfobarShow;

    [ObservableProperty]
    private string? infobarTitle;

    [ObservableProperty]
    private string? infobarMessage;

    [ObservableProperty]
    private InfoBarSeverity infoBarSeverity = InfoBarSeverity. Success;

    #endregion

    //丹迪克设备对象
    [ObservableProperty]
    private Dandick? sS;

    //用户选择的设备型号
    [ObservableProperty]
    private string? sS_Model;

    [ObservableProperty]
    private string? portName;

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
        switch ( isOn )
        {
            case true:
                {
                    DisableSerialPortEdit = false;

                    if ( SS_Model != null && PortName != null )
                    {
                        //实例化对象
                        SS = new Dandick(( DKCommunicationNET. Models )Enum. Parse(typeof(DKCommunicationNET. Models) , SS_Model));

                        //初始化串口参数
                        SS. SerialPortInni(PortName , baudRate);

                        //打开串口并发送握手报文

                        try
                        {
                            SS. Open();
                            if ( !SS. IsOpen() )
                            {
                                InfoBarSeverity = InfoBarSeverity. Error;
                                IsInfobarShow = true;
                                InfobarTitle = "Failed";
                                InfobarMessage = "串口打开失败。";
                                return;
                            }
                            InfoBarSeverity = InfoBarSeverity. Success;
                            IsInfobarShow = true;
                            InfobarTitle = "Success";
                            InfobarMessage = "串口打开成功。";
                            Model = SS. Model;
                        }
                        catch ( Exception ex )
                        {
                            InfoBarSeverity = InfoBarSeverity. Warning;
                            IsInfobarShow = true;
                            InfobarTitle = "Warning";
                            InfobarMessage = ex. Message;
                        }
                    }
                    break;
                }

            case false:
                {
                    DisableSerialPortEdit = true;

                    if ( SS != null )
                    {
                        SS. Close();
                    }
                    break;
                }
        }
    }

    #region 自动界面处理方法

    #endregion

}
