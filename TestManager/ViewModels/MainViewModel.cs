using System. IO. Ports;
using CommunityToolkit. Mvvm. ComponentModel;
using CommunityToolkit. Mvvm. Input;
using DKCommunicationNET;
using DKCommunicationNET. Module;
using Microsoft. UI. Xaml. Controls;

namespace TestManager. ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel ()
    {
        portName = portNames[0];
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

    #region 串口开关绑定
    /// <summary>
    /// 开关的状态属性
    /// </summary>
    [ObservableProperty]
    private bool isOn_PortSwitch;
    #endregion

    /// <summary>
    /// 丹迪克设备对象
    /// </summary>
    [ObservableProperty]
    private Dandick? sS;

    /// <summary>
    /// 当前选择的设备型号
    /// </summary>
    [ObservableProperty]
    private string? sS_Model;

    /// <summary>
    /// 当前选择的串口号
    /// </summary>
    [ObservableProperty]
    private string? portName;

    /// <summary>
    /// 当前设置的波特率
    /// </summary>
    [ObservableProperty]
    private int baudRate;

    #region 从实例同步的引用类型数据

    /// <summary>
    /// 下位机回复的型号
    /// </summary>
    [ObservableProperty]
    private string? model;

    /// <summary>
    /// 交流电压档位集合
    /// </summary>
    [ObservableProperty]
    private float[]? ranges_ACU;

    /// <summary>
    /// 交流电压档位集合
    /// </summary>
    [ObservableProperty]
    private float[]? ranges_ACI;
    #endregion


    #region ComboBox初始化

    [ObservableProperty]
    private string[] dandickModels = Enum. GetNames(typeof(DKCommunicationNET. Models));

    [ObservableProperty]
    private string[]? portNames = SerialPort. GetPortNames();

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



    #endregion
    /// <summary>
    /// 指示打开串口后禁用的状态
    /// </summary>
    [ObservableProperty]
    private bool disableSerialPortEdit = true;

    public async void ToggleSwitch_Toggled (bool isOn)
    {
        switch ( isOn )
        {
            //正在打开串口
            case true:

                //禁用参数设置控件
                DisableSerialPortEdit = false;
                //判断是否选择型号和串口号
                if ( SS_Model == null || PortName == null )
                {
                    InfoBarSeverity = InfoBarSeverity. Warning;
                    IsInfobarShow = true;
                    InfobarTitle = "Warning";
                    InfobarMessage = "还没有选择任何设备型号";
                    IsOn_PortSwitch = false;
                    return;
                }
                //实例化对象
                SS = new Dandick(( DKCommunicationNET. Models )Enum. Parse(typeof(DKCommunicationNET. Models) , SS_Model));
                //初始化串口参数
                SS. SerialPortInni(PortName , baudRate);

                //打开串口并发送握手报文
                var result = SS. Open();

                if ( result. IsSuccess )
                {
                    InfoBarSeverity = InfoBarSeverity. Success;
                    IsInfobarShow = false;   //TODO 改为false
                    InfobarTitle = "Success";
                    InfobarMessage = result. Content;
                    IsOn_PortSwitch = true;

                    await Task. Run(() =>
                    {
                        SS. HandShake();

                        //获取交流源档位信息
                        var result_GetRanges = SS. ACS. GetRanges();
                        SS. DCS. GetRanges();
                    });
                    Ranges_ACU = SS. ACS. Ranges_ACU;
                    Ranges_ACI = SS. ACS.Ranges_ACI;
                    return;
                }
                else
                {
                    InfoBarSeverity = InfoBarSeverity. Error;
                    IsInfobarShow = true;
                    InfobarTitle = "Failed";
                    InfobarMessage = result. Message;
                    IsOn_PortSwitch = false;
                    return;
                }

            //正在关闭串口
            case false:

                DisableSerialPortEdit = true;
                IsOn_PortSwitch = false;
                if ( SS != null )
                {
                    SS. Close();
                }
                break;
        }
    }

    #region 自动界面处理方法

    #endregion

}
