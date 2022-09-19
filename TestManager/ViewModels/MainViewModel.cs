using System. IO. Ports;
using CommunityToolkit. Mvvm. ComponentModel;
using CommunityToolkit. Mvvm. Input;
using Microsoft. UI. Xaml;
using Microsoft. UI. Xaml. Controls;
using Windows. Globalization. NumberFormatting;
using Windows. UI. WebUI;

namespace TestManager. ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel ()
    {
        portNames. Sort();
        portName = portNames[0];
        baudRate = baudRates[2];
        Formatter_ACU = new();
        rounder_ACU = new();
        SetNumberBoxNumberFormatter_ACU();
        SetNumberBoxNumberFormatter_ACI();
        SetNumberBoxNumberFormatter_PQ();
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
    [ObservableProperty]
    private string? sN;
    [ObservableProperty]
    private string? firmWare;

    #region 《交流电压档位集合
    /// <summary>
    /// 下位机回复的交流电压档位集合
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SmallChange_U))]
    [NotifyPropertyChangedFor(nameof(LargeChange_U))]
    private float[]? ranges_ACU;
    partial void OnRanges_ACUChanged (float[]? value) => MaxValue_U = ( float )( value != null ? value[rangeIndex_Ua] * 1.2 : 1000 );
    #endregion  交流电压档位集合》

    #region 《交流电流档位集合
    /// <summary>
    /// 交流电流档位集合
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SmallChange_I))]
    [NotifyPropertyChangedFor(nameof(LargeChange_I))]
    private float[]? ranges_ACI;
    partial void OnRanges_ACIChanged (float[]? value) => MaxValue_I = ( float )( value != null ? value[rangeIndex_Ia] * 1.2 : 100 );
    #endregion 交流电流档位集合》

    #region UA
    [ObservableProperty]
    private float uA;
    #endregion

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

    #region 《设置接线方式
    [ObservableProperty]
    private WireMode[] itemsWireMode = ( WireMode[] )Enum. GetValues(typeof(WireMode));
    [ObservableProperty]
    private byte wireModeIndex;
    private byte wireModeIndex_Temp;
    partial void OnWireModeIndexChanging (byte value)
    {
        wireModeIndex_Temp = WireModeIndex;
        this. OnWireModeIndexChange(value);
    }
    partial void OnWireModeIndexChanged (byte value)
    {
        wireModeIndex = wireModeIndex_Temp;
    }
    private async void OnWireModeIndexChange (byte value)
    {
        var result = await Task. Run(() =>
        {
            var result = DKS?.ACS. SetWireMode(( WireMode )value);
            if ( result?.IsSuccess ?? false )
            {
                wireModeIndex_Temp = value;
            }
            return result;
        });
        UpdateInfoBar("设置接线方式" , result);
    }
    #endregion 设置接线方式》

    #region 《设置闭环模式
    [ObservableProperty]
    private CloseLoopMode[] itemsCloseLoopMode = ( CloseLoopMode[] )Enum. GetValues(typeof(CloseLoopMode));
    [ObservableProperty]
    private byte loopModeIndex;
    private byte loopModeIndex_Temp;
    partial void OnLoopModeIndexChanging (byte value)
    {
        loopModeIndex_Temp = loopModeIndex;
        SetCloseLoopMode(value);
    }
    partial void OnLoopModeIndexChanged (byte value)
    {
        loopModeIndex = loopModeIndex_Temp;
    }
    private async void SetCloseLoopMode (byte value)
    {
        var result = await Task. Run(() => DKS?.ACS. SetClosedLoop(( CloseLoopMode )value , ( HarmonicMode )HarmonicModeIndex));
        if ( result?.IsSuccess ?? false )
        {

            loopModeIndex_Temp ^= value;
        }
        UpdateInfoBar("设置闭环模式" , result);
    }
    #endregion 设置闭环模式》

    #region 《设置谐波模式
    [ObservableProperty]
    private HarmonicMode[] itemsHarmonicMode = ( HarmonicMode[] )Enum. GetValues(typeof(HarmonicMode));
    [ObservableProperty]
    private byte harmonicModeIndex;
    private byte harmonicModeIndex_Temp;
    partial void OnHarmonicModeIndexChanging (byte value)
    {
        harmonicModeIndex_Temp = harmonicModeIndex;
        SetHarmonicMode(value);
    }
    partial void OnHarmonicModeIndexChanged (byte value)
    {
        harmonicModeIndex = harmonicModeIndex_Temp;
    }

    private async void SetHarmonicMode (byte harmonicMode)
    {
        var result = await Task. Run(() => DKS?.ACS. SetHarmonicMode(( HarmonicMode )harmonicMode , ( CloseLoopMode )loopModeIndex));
        if ( result?.IsSuccess ?? false )
        {
            harmonicModeIndex_Temp = harmonicMode;
        }
        UpdateInfoBar("设置谐波模式" , result);
    }
    #endregion 设置谐波模式》

    [ObservableProperty]
    private QP_Mode[] itemsQPMode = ( QP_Mode[] )Enum. GetValues(typeof(QP_Mode));
    [ObservableProperty]
    private QP_Mode qP_Mode;

    [ObservableProperty]
    private RangeSwitchMode[] itemsRangeSwitchMode = ( RangeSwitchMode[] )Enum. GetValues(typeof(RangeSwitchMode));
    [ObservableProperty]
    private RangeSwitchMode rangeSwitchMode=RangeSwitchMode.Manual;
    #endregion 数据区》

    #region 《操作区

    #region 《电压电流幅值设置
    [ObservableProperty]
    private Channels[] channels = ( Channels[] )Enum. GetValues(typeof(Channels));
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetAmplitude_1Command))]
    [NotifyCanExecuteChangedFor(nameof(SetAmplitude_DualCommand))]
    private Channels channel_1;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetAmplitude_DualCommand))]
    [NotifyCanExecuteChangedFor(nameof(SetAmplitude_2Command))]
    private Channels channel_2;
    [ObservableProperty]
    private float setAmplitudeValue_1;
    [ObservableProperty]
    private float setAmplitudeValue_2;

    /// <summary>
    /// 当打开交流源开关则显示相关操作菜单
    /// </summary>
    [ObservableProperty]
    private Visibility visibility_ACS = Visibility. Collapsed;

    /// <summary>
    /// 通道 1 的幅值设定方法
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute_1))]
    private void SetAmplitude_1 ()
    {
        Task. Run(() => DKS?.ACS. SetAmplitude(channel_1 , setAmplitudeValue_1));
    }
    [RelayCommand(CanExecute = nameof(CanExecute_2))]
    private void SetAmplitude_2 ()
    {
        Task. Run(() => DKS?.ACS. SetAmplitude(channel_2 , setAmplitudeValue_2));
    }
    [RelayCommand(CanExecute = nameof(CanExecute_Dual))]
    private void SetAmplitude_Dual ()
    {
        SetAmplitude_1();
        SetAmplitude_2();
    }
    private bool CanExecute_1 ()
    {
        return channel_1 == 0 ? false : true;
    }
    private bool CanExecute_2 ()
    {
        return channel_2 == 0 ? false : true;
    }
    private bool CanExecute_Dual ()
    {
        return channel_1 == 0 || channel_2 == 0 ? false : true;
    }
    #endregion 电压电流幅值设置》

    [ObservableProperty]
    private bool isEnabled_ACS;
    [ObservableProperty]
    private bool isEnabled_DCS;
    [ObservableProperty]
    private bool isEnabled_ACM;
    /// <summary>
    /// 指示直流表功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_DCM;
    /// <summary>
    /// 指示电能校验功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_EQP;
    /// <summary>
    /// 指示开关量功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_IO;
    /// <summary>
    /// 指示标准表钳表功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_ACM_Cap;
    /// <summary>
    /// 辅助直流源是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_DCS_AUX;
    /// <summary>
    /// 指示直流纹波表是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_DCM_RIP;
    /// <summary>
    /// 指示双频输出功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_DualFreqs;

    /// <summary>
    /// 指示保护电流功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_IProtect;

    /// <summary>
    /// 指示闪变输出功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_PST;

    /// <summary>
    /// 指示遥信功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_YX;

    /// <summary>
    /// 指示高频输出功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_HF;

    /// <summary>
    /// 指示电机控制功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_PWM;

    /// <summary>
    /// 指示对时功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_PPS;

    /// <summary>
    /// 指示ACS开关状态.
    /// </summary>
    [ObservableProperty]
    private bool isOpenACS;

    [ObservableProperty]
    private bool isOpenDCS;

    [ObservableProperty]
    private bool isOpenEQP;

    [ObservableProperty]
    private bool isOpenDCM;

    #region 《电压档位索引值
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MaxValue_U))]
    [NotifyPropertyChangedFor(nameof(SmallChange_U))]
    [NotifyPropertyChangedFor(nameof(LargeChange_U))]
    private byte rangeIndex_Ua;
    [ObservableProperty]
    private float maxValue_U;
    public float SmallChange_U => ( float )( ranges_ACU != null ? ranges_ACU[rangeIndex_Ua] * 0.1 : 0 );
    public float LargeChange_U => ( float )( ranges_ACU != null ? ranges_ACU[rangeIndex_Ua] * 0.2 : 0 );

    /// <summary>
    /// 【备份】用于设置失败时恢复设置前的值；
    /// </summary>
    private byte rangeIndex_Ua_Backup;
    partial void OnRangeIndex_UaChanging (byte value)
    {
        rangeIndex_Ua_Backup = rangeIndex_Ua;
        SetRange_ACU(value);
    }
    partial void OnRangeIndex_UaChanged (byte value)
    {
        rangeIndex_Ua = rangeIndex_Ua_Backup;
    }

    private async void SetRange_ACU (byte index)
    {
        var result = await Task. Run(() => DKS?.ACS. SetRanges(index , rangeIndex_Ia));
        if ( result?.IsSuccess ?? false )
        {
            rangeIndex_Ua_Backup = index;
        }
        UpdateInfoBar("设置交流电压档位" , result);
    }
    #endregion 电压档位索引值》

    #region 《电流档位索引值
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MaxValue_I))]
    [NotifyPropertyChangedFor(nameof(SmallChange_I))]
    [NotifyPropertyChangedFor(nameof(LargeChange_I))]
    private byte rangeIndex_Ia;
    [ObservableProperty]
    private float maxValue_I;
    public float SmallChange_I => ( float )( ranges_ACI != null ? ranges_ACI[rangeIndex_Ia] * 0.1 : 0 );
    public float LargeChange_I => ( float )( ranges_ACI != null ? ranges_ACI[rangeIndex_Ia] * 0.2 : 0 );
    /// <summary>
    /// 【备份】用于设置失败时恢复设置前的值；
    /// </summary>
    private byte rangeIndex_Ia_Temp;
    partial void OnRangeIndex_IaChanging (byte value)
    {
        rangeIndex_Ia_Temp = rangeIndex_Ia;
        SetRange_ACI(value);
    }
    partial void OnRangeIndex_IaChanged (byte value)
    {
        rangeIndex_Ia = rangeIndex_Ia_Temp;
        // MaxValue_I = ( float )( ranges_ACI != null ? ranges_ACI[rangeIndex_Ia] * 1.2 : 0 );
    }
    private async void SetRange_ACI (byte index)
    {
        var result = await Task. Run(() => DKS?.ACS. SetRanges(rangeIndex_Ua , index));
        if ( result?.IsSuccess ?? false )
        {
            rangeIndex_Ia_Temp = index;
        }
        UpdateInfoBar("设置交流电流档位" , result);
    }
    #endregion 电流档位索引值》


    partial void OnIsOpenACSChanged (bool value)
    {
        if ( value )
        {
            Task. Run(new Action(OpenACS));
            Visibility_ACS = Visibility. Visible;
        }
        else
        {
            Task. Run(() => DKS?.ACS. Stop());
            Visibility_ACS = Visibility. Collapsed;
        }
    }


    partial void OnIsOpenDCSChanged (bool value)
    {
        if ( value )
        {
            Task. Run(new Action(OpenDCS));
        }
        else
        {
            Task. Run(() =>
            {
                DKS?.DCS. Stop_DCI();
                DKS?.DCS. Stop_DCU();
                DKS?.DCS. Stop_DCR();
            });
        }
    }
    partial void OnIsOpenEQPChanged (bool value)
    {
        if ( value )
        {
            Task. Run(new Action(OpenEQP));
        }
    }
    partial void OnIsOpenDCMChanged (bool value)
    {
        if ( value )
        {
            Task. Run(new Action(OpenDCM));
        }
    }

    private void OpenACS ()
    {
        DKS?.ACS. Open();
        while ( IsOpenACS )
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
            RangeIndex_Ia = DKS?.ACS. RangeIndex_Ia ?? 0;
            RangeIndex_Ua = DKS?.ACS. RangeIndex_Ua ?? 0;
        }
    }
    private void OpenDCS ()
    {
        while ( IsOpenDCS )
        {
            var result = DKS?.DCS. ReadData();
            if ( result?.IsSuccess ?? false )
            {

            }
        }
    }
    private void OpenDCM ()
    {
        while ( IsOpenDCM )
        {
            var result = DKS?.DCM. ReadData();
            if ( result?.IsSuccess ?? false )
            {

            }
        }
    }
    private void OpenEQP ()
    {
        while ( IsOpenEQP )
        {
            var result = DKS?.EPQ. ReadData();
            if ( result?.IsSuccess ?? false )
            {

            }
        }
    }
    #endregion 操作区》

    #region 状态栏Infobar绑定
    /// <summary>
    /// 串口打开进度
    /// </summary>
    [ObservableProperty]
    public bool isInfobarShow;

    [ObservableProperty]
    public string? infobarTitle;

    [ObservableProperty]
    public string? infobarMessage;

    [ObservableProperty]
    public InfoBarSeverity infoBarSeverity = InfoBarSeverity. Success;

    #endregion

    #region 串口开关绑定
    /// <summary>
    /// 总开关的状态属性
    /// </summary>
    [ObservableProperty]
    private bool isOn_PortSwitch;
    partial void OnIsOn_PortSwitchChanging (bool value)
    {
        if ( !value )
        {
            IsOpenACS = false;
            IsOpenDCM = false;
            IsOpenEQP = false;
            IsOpenDCS = false;
        }
    }


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
    private Parity[] paritys = ( Parity[] )Enum. GetValues(typeof(Parity));

    [ObservableProperty]
    private StopBits[] stopBits = ( StopBits[] )Enum. GetValues(typeof(StopBits));

    [ObservableProperty]
    private Handshake[] handShakes = ( Handshake[] )Enum. GetValues(typeof(Handshake));

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
                DKS = new DKStandardSource(Enum. Parse<DKCommunicationNET. Models>(ModelSelected) , PortName , BaudRate , ID);

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
                        IsEnabled_ACS = DKS. Settings. IsEnabled_ACS;
                        IsEnabled_ACM = DKS. Settings. IsEnabled_ACM;
                        IsEnabled_DCS = DKS. Settings. IsEnabled_DCS;
                        IsEnabled_DCM = DKS. Settings. IsEnabled_DCM;
                        IsEnabled_EQP = DKS. Settings. IsEnabled_EPQ;
                        IsEnabled_ACM_Cap = DKS. Settings. IsEnabled_ACM_Cap;
                        IsEnabled_DCS_AUX = DKS. Settings. IsEnabled_DCS_AUX;
                        IsEnabled_DCM_RIP = DKS. Settings. IsEnabled_DCM_RIP;
                        IsEnabled_IO = DKS. Settings. IsEnabled_IO;
                        IsEnabled_DualFreqs = DKS. Settings. IsEnabled_DualFreqs;
                        IsEnabled_IProtect = DKS. Settings. IsEnabled_IProtect;
                        IsEnabled_PST = DKS. Settings. IsEnabled_PST;
                        IsEnabled_YX = DKS. Settings. IsEnabled_YX;
                        IsEnabled_HF = DKS. Settings. IsEnabled_HF;
                        IsEnabled_PWM = DKS. Settings. IsEnabled_PWM;
                        IsEnabled_PPS = DKS. Settings. IsEnabled_PPS;
                        SN = DKS. Settings. SN;
                        Model = DKS. Settings. Model;
                        FirmWare = DKS. Settings. Firmware;
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

                    //获取直流表档位
                    var res4 = await Task. Run(() => DKS. Settings. IsEnabled_DCM ? DKS. DCM. GetRanges() : null);

                    if ( res4 != null && !res4. IsSuccess )
                    {
                        if ( infoBarSeverity < InfoBarSeverity. Warning )
                        {
                            InfobarTitle = "Warning";
                            InfoBarSeverity = InfoBarSeverity. Warning;
                        }
                        InfobarMessage += "【获取直流表档位失败】";
                    }
                    else if ( res4 != null && res4. IsSuccess )
                    {
                        InfobarMessage += "【获取直流表档位成功】";
                    }

                    //交流电压档位集合初始化
                    Ranges_ACU = DKS. ACS. Ranges_ACU;
                    //交流电流档位集合初始化
                    Ranges_ACI = DKS. ACS. Ranges_ACI;
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
    public void UpdateInfoBar (string title , OperateResult? result)
    {
        if ( result?.IsSuccess ?? false )
        {
            InfoBarSeverity = InfoBarSeverity. Success;
        }
        else if ( !result?.IsSuccess ?? false )
        {
            InfoBarSeverity = InfoBarSeverity. Error;
        }
        else if ( result is null )
        {
            InfoBarSeverity = InfoBarSeverity. Informational;
        }
        InfobarTitle = title;
        InfobarMessage = result?.Message;
    }
    /// <summary>
    /// 交流电压显示格式
    /// </summary>

    public DecimalFormatter Formatter_ACU
    {
        get; set;
    }

    private readonly IncrementNumberRounder rounder_ACU;

    private void SetNumberBoxNumberFormatter_ACU ()
    {
        rounder_ACU. Increment = 0.001;
        rounder_ACU. RoundingAlgorithm = RoundingAlgorithm. RoundHalfUp;
        Formatter_ACU. IntegerDigits = 1;
        Formatter_ACU. FractionDigits = 3;
        Formatter_ACU. NumberRounder = rounder_ACU;
    }
    /// <summary>
    /// 交流电流显示格式
    /// </summary>
    [ObservableProperty]
    private DecimalFormatter formatter_ACI;
    private void SetNumberBoxNumberFormatter_ACI ()
    {
        IncrementNumberRounder rounder = new IncrementNumberRounder();
        rounder. Increment = 0.00001;
        rounder. RoundingAlgorithm = RoundingAlgorithm. RoundHalfUp;
        formatter_ACI = new();
        formatter_ACI. IntegerDigits = 1;
        formatter_ACI. FractionDigits = 4;
        formatter_ACI. NumberRounder = rounder;
    }

    [ObservableProperty]
    private DecimalFormatter formatter_PQ;
    private void SetNumberBoxNumberFormatter_PQ ()
    {
        IncrementNumberRounder rounder = new IncrementNumberRounder();
        rounder. Increment = 0.001;
        rounder. RoundingAlgorithm = RoundingAlgorithm. RoundHalfUp;
        formatter_PQ = new();
        formatter_PQ. IntegerDigits = 1;
        formatter_PQ. FractionDigits = 3;
        formatter_PQ. NumberRounder = rounder;
    }
    #endregion

}
