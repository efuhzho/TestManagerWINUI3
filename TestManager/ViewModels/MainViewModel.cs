using System. IO. Ports;
using CommunityToolkit. Mvvm. ComponentModel;
using CommunityToolkit. Mvvm. Input;
using DKCommunicationNET;
using Microsoft. UI. Xaml;
using Microsoft. UI. Xaml. Controls;

namespace TestManager. ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel ()
    {
        portNames. Sort();
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
    private float uA;
    [ObservableProperty]
    private float uB;
    [ObservableProperty]
    private float uC;
    [ObservableProperty]
    private float uX;

    [ObservableProperty]
    private float iA;
    [ObservableProperty]
    private float iB;
    [ObservableProperty]
    private float iC;
    [ObservableProperty]
    private float iX;

    [ObservableProperty]
    private float faiUA;
    [ObservableProperty]
    private float faiUB;
    [ObservableProperty]
    private float faiUC;
    [ObservableProperty]
    private float faiUX;

    [ObservableProperty]
    private float faiIA;
    [ObservableProperty]
    private float faiIB;
    [ObservableProperty]
    private float faiIC;
    [ObservableProperty]
    private float faiIX;

    [ObservableProperty]
    private float tHDUA;
    [ObservableProperty]
    private float tHDUB;
    [ObservableProperty]
    private float tHDUC;
    [ObservableProperty]
    private float tHDUX;
    [ObservableProperty]
    private float tHDU;
    [ObservableProperty]
    private float tHDIA;
    [ObservableProperty]
    private float tHDIB;
    [ObservableProperty]
    private float tHDIC;
    [ObservableProperty]
    private float tHDIX;
    [ObservableProperty]
    private float tHDI;

    [ObservableProperty]
    private float pA;
    [ObservableProperty]
    private float pB;
    [ObservableProperty]
    private float pC;
    [ObservableProperty]
    private float pX;
    [ObservableProperty]
    private float p;

    [ObservableProperty]
    private float qA;
    [ObservableProperty]
    private float qB;
    [ObservableProperty]
    private float qC;
    [ObservableProperty]
    private float qX;
    [ObservableProperty]
    private float q;

    [ObservableProperty]
    private float sA;
    [ObservableProperty]
    private float sB;
    [ObservableProperty]
    private float sC;
    [ObservableProperty]
    private float sX;
    [ObservableProperty]
    private float s;

    [ObservableProperty]
    private float pFA;
    [ObservableProperty]
    private float pFB;
    [ObservableProperty]
    private float pFC;
    [ObservableProperty]
    private float pFX;
    [ObservableProperty]
    private float pF;

    [ObservableProperty]
    private float freq;
    [ObservableProperty]
    private float freqC;
    [ObservableProperty]
    private float freqX;
    [ObservableProperty]
    private FrequencySync frequencySync;
    [ObservableProperty]
    private WireMode[] itemsWireMode=( WireMode[] )Enum.GetValues(typeof(WireMode));
    [ObservableProperty]
    private CloseLoopMode[] itemsCloseLoopMode=(CloseLoopMode[] )Enum.GetValues(typeof(CloseLoopMode));
    [ObservableProperty]
    private HarmonicMode[] itemsHarmonicMode=(HarmonicMode[] )Enum.GetValues(typeof(HarmonicMode));
    [ObservableProperty]
    private QP_Mode[] itemsQPMode=(QP_Mode[] )Enum.GetValues(typeof(QP_Mode));
    [ObservableProperty]
    private RangeSwitchMode[] itemsRangeSwitchMode=(RangeSwitchMode[] )Enum.GetValues(typeof(RangeSwitchMode));
    #endregion 数据区》

    #region 状态栏Infobar绑定
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

    /// <summary>
    /// 丹迪克设备对象
    /// </summary>
    [ObservableProperty]
    private DKStandardSource? dKS;

    /// <summary>
    /// 当前选择的设备型号
    /// </summary>
    [ObservableProperty]
    private string? modelSelected;

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

    [ObservableProperty]
    private List<string> portNames = SerialPort. GetPortNames(). ToList();

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
    private void RefreshPortNames ()
    {
        PortNames. Clear();
        PortNames = SerialPort. GetPortNames(). ToList();
        PortNames. Sort();
    }
    #endregion

    [RelayCommand]
    private void ReadData_ACS ()
    {
        DKS?.ACS. ReadData();
        UA = DKS?.ACS. IA ?? 0;
        UB = DKS?.ACS. UB ?? 0;
        UC = DKS?.ACS. UC ?? 0;
        UX = DKS?.ACS. UX ?? 0;
        IA = DKS?.ACS. IA ?? 0;
        IB = DKS?.ACS. IB ?? 0;
        IC = DKS?.ACS. IC ?? 0;
        IX = DKS?.ACS. IX ?? 0;
        FaiUA = DKS?.ACS. FAI_UA ?? 0;
        FaiUB = DKS?.ACS. FAI_UB ?? 0;
        FaiUC = DKS?.ACS. FAI_UC ?? 0;
        FaiIA = DKS?.ACS. FAI_IA ?? 0;
        FaiIB = DKS?.ACS. FAI_IB ?? 0;
        FaiIC = DKS?.ACS. FAI_IC ?? 0;
        PA = DKS?.ACS. PA ?? 0;
        PB = DKS?.ACS. PB ?? 0;
        PC = DKS?.ACS. PC ?? 0;
        PX = DKS?.ACS. PX ?? 0;
        P = DKS?.ACS. P ?? 0;
        QA = DKS?.ACS. QA ?? 0;
        QB = DKS?.ACS. QB ?? 0;
        QC = DKS?.ACS. QC ?? 0;
        QX = DKS?.ACS. QX ?? 0;
        Q = DKS?.ACS. Q ?? 0;
        SA = DKS?.ACS. SA ?? 0;
        SB = DKS?.ACS. SB ?? 0;
        SC = DKS?.ACS. SC ?? 0;
        SX = DKS?.ACS. SX ?? 0;
        S = DKS?.ACS. S ?? 0;
        PFA = DKS?.ACS. PFA ?? 0;
        PFB = DKS?.ACS. PFB ?? 0;
        PFC = DKS?.ACS. PFC ?? 0;
        PFX = DKS?.ACS. PFX ?? 0;
        PF = DKS?.ACS. PF ?? 0;
        FreqC = DKS?.ACS. Freq_C ?? 0;
        FreqX = DKS?.ACS. Freq_X ?? 0;
        Freq = DKS?.ACS. Freq ?? 0;
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
                if ( ModelSelected == null || PortName == null )
                {
                    InfoBarSeverity = InfoBarSeverity. Warning;
                    IsInfobarShow = true;
                    InfobarTitle = "Warning";
                    InfobarMessage = "还没有选择任何设备型号";
                    IsOn_PortSwitch = false;
                    return;
                }

                //实例化对象
                DKS = new DKStandardSource(Enum.Parse<DKCommunicationNET.Models> (ModelSelected), PortName , BaudRate , ID);

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
                        InfobarMessage += $"【联机失败，功能已禁用：{res1. Message}】";
                        ProgressRingWhenSwitching = false;
                        IsOn_PortSwitch = false;
                        return;
                    }
                    //*****异步获取交流源档位
                    var res2 = await Task. Run(() =>
                      {
                          return DKS. Settings. IsEnabled_ACS ? DKS. ACS. GetRanges() : null;
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
                        return DKS. Settings. IsEnabled_DCS ? DKS. DCS. GetRanges() : null;
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
