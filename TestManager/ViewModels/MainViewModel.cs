using System. IO. Ports;
using CommunityToolkit. Mvvm. ComponentModel;
using CommunityToolkit. Mvvm. Input;
using DKCommunicationNET;
using DKCommunicationNET. Module;
using Microsoft. UI. Xaml;
using Microsoft. UI. Xaml. Controls;

namespace TestManager. ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel ()
    {
        portName = portNames[0];
        baudRate = baudRates[2];
    }

    /// <summary>
    /// 用户选择的设备ID
    /// </summary>
    [ObservableProperty]
    private ushort iD;

    /// <summary>
    /// 设备型号
    /// </summary>
    [ObservableProperty]
    private string[] dandickModels = Enum. GetNames(typeof(DKCommunicationNET. Models));

    #region 《数据区

    /// <summary>
    /// 下位机回复的型号
    /// </summary>
    [ObservableProperty]
    private string? model;
    /// <summary>
    /// 下位机回复的交流电压档位集合
    /// </summary>
    [ObservableProperty]
    private float[]? ranges_ACU;
    /// <summary>
    /// 交流电流档位集合
    /// </summary>
    [ObservableProperty]
    private float[]? ranges_ACI;

    [ObservableProperty]
    float uA;

    #endregion 数据区》

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
    private DKStandardSource? dKS;

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

   


    #region ComboBox初始化



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

    /// <summary>
    /// 串口开关打开时，显示超时设置选项
    /// </summary>
    [ObservableProperty]
    private Visibility visibilityFollowToggleSwitch = Visibility. Collapsed;

    #endregion
    /// <summary>
    /// 指示打开串口后禁用的状态
    /// </summary>
    [ObservableProperty]
    private bool disableSerialPortEdit = true;

    /// <summary>
    /// 当点击开关时显示转圈圈
    /// </summary>
    [ObservableProperty]
    bool progressRingWhenSwitching;

    [RelayCommand]
    void Read ()
    {
        DKS?.ACS. ReadData();
        UA = DKS?.ACS. IA ?? 0;
    }

    /// <summary>
    /// 打开串口开关时，设备实例化
    /// </summary>
    /// <param name="isOn"></param>
    public async void ToggleSwitch_Toggled (bool isOn)
    {

        switch ( isOn )
        {
            //正在打开串口
            case true:

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
                DKS = new DKStandardSource(( DKCommunicationNET. Models )Enum. Parse(typeof(DKCommunicationNET. Models) , SS_Model) , PortName , BaudRate , ID);

                //打开串口并发送握手报文               
                var result = DKS. Open();

                if ( result. IsSuccess )
                {
                    InfoBarSeverity = InfoBarSeverity. Success;
                    IsInfobarShow = true;
                    InfobarTitle = "Success";
                    InfobarMessage = "【打开串口成功】";
                    IsOn_PortSwitch = true;
                    //开始转圈圈
                    ProgressRingWhenSwitching = true;

                    //*****异步联机命令
                    var res1 = await Task. Run(() =>
                     {
                         return DKS. HandShake();
                     });
                    if ( res1. IsSuccess )
                    {
                        InfobarMessage += "【获取设备信息成功】";
                    }
                    else
                    {
                        InfoBarSeverity = InfoBarSeverity. Error;
                        InfobarTitle = "Error";
                        InfobarMessage += $"【联机失败，所有功能不可使用：{res1. Message}】";
                    }
                    //*****异步获取交流源档位
                    var res2 = await Task. Run(() =>
                      {
                          return DKS. ACS?.GetRanges();
                      });

                    if ( res2 != null && !res2. IsSuccess )
                    {
                        if ( infoBarSeverity < InfoBarSeverity. Warning )
                        {
                            InfoBarSeverity = InfoBarSeverity. Warning;
                            InfobarTitle = "Warning";
                        }
                        InfobarMessage += "【获取交流源档位失败】";
                    }
                    else if ( res2 != null && res2. IsSuccess )
                    {
                        InfobarMessage += "【获取交流源档位成功】";
                    }

                    //*****异步获取直流源档位
                    var res3 = await Task. Run(() =>
                    {
                        return DKS. DCS?.GetRanges();
                    });

                    if ( res3 != null && !res3. IsSuccess )
                    {
                        if ( infoBarSeverity < InfoBarSeverity. Warning )
                        {
                            InfobarTitle = "Warning";
                            InfoBarSeverity = InfoBarSeverity. Warning;
                        }
                        InfobarMessage += "【获取直流源档位失败】";
                    }
                    else if ( res3 != null && res3. IsSuccess )
                    {
                        InfobarMessage += "【获取直流源档位成功】";
                    }

                    //交流电压档位集合初始化
                    Ranges_ACU = DKS. ACS?.Ranges_ACU;
                    //交流电流档位集合初始化
                    Ranges_ACI = DKS. ACS?.Ranges_ACI;
                    //禁用参数设置控件
                    DisableSerialPortEdit = false;
                    //打开串口超时参数设置控件
                    VisibilityFollowToggleSwitch = Visibility. Visible;
                    //停止转圈圈
                    ProgressRingWhenSwitching = false;

                }
                else
                {
                    InfoBarSeverity = InfoBarSeverity. Error;
                    IsInfobarShow = true;
                    InfobarTitle = "Failed";
                    InfobarMessage = result. Message;
                    IsOn_PortSwitch = false;
                }
                break;

            //正在关闭串口
            case false:
                DisableSerialPortEdit = true;
                VisibilityFollowToggleSwitch = Visibility. Collapsed;
                IsOn_PortSwitch = false;
                DKS?.Close();
                break;
        }
    }


    #region 自动界面处理方法

    #endregion

}
