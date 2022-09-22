using System.Drawing;
using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json.Linq;
using Windows.Globalization.NumberFormatting;
using Brush = Microsoft.UI.Xaml.Media.Brush;

namespace TestManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        portNames.Sort();
        portName = portNames[0];
        baudRate = baudRates[2];
        SetNumberBoxNumberFormatter_ACU();
        SetNumberBoxNumberFormatter_ACI();
        SetNumberBoxNumberFormatter_PQ();
        SetNumberBoxNumberFormatter_Freq();
        SetNumberBoxNumberFormatter_PF();
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
    private string[] dandickModels = Enum.GetNames(typeof(DKCommunicationNET.Models));

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
    partial void OnRanges_ACUChanged(float[]? value) => MaxValue_U = (float)((value != null && rangeIndex_Ua != 0XFF) ? value[rangeIndex_Ua] * 1.2 : 0);
    #endregion  交流电压档位集合》

    #region 《交流电流档位集合
    /// <summary>
    /// 交流电流档位集合
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SmallChange_I))]
    [NotifyPropertyChangedFor(nameof(LargeChange_I))]
    private float[]? ranges_ACI;
    partial void OnRanges_ACIChanged(float[]? value) => MaxValue_I = (float)((value != null && rangeIndex_Ia != 0XFF) ? value[rangeIndex_Ia] * 1.2 : 0);
    #endregion 交流电流档位集合》

    #region 《直流源档位集合
    [ObservableProperty]
    private float[]? ranges_DCU;
    [ObservableProperty]
    private float[]? ranges_DCI;
    #endregion 直流源档位集合》

    #region 《直流表档位集合
    [ObservableProperty]
    private float[]? ranges_DCMU;
    [ObservableProperty]
    private float[]? ranges_DCMI;
    #endregion 直流表档位集合》

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

    #region 《设置接线方式
    [ObservableProperty]
    private WireMode[] itemsWireMode = (WireMode[])Enum.GetValues(typeof(WireMode));
    [ObservableProperty]
    private byte wireModeIndex;

    partial void OnWireModeIndexChanging(byte value)
    {
        this.OnWireModeIndexChange(value);
    }

    private async void OnWireModeIndexChange(byte value)
    {
        var result = await Task.Run(() => DKS?.ACS.SetWireMode((WireMode)value));
        UpdateInfoBar("设置接线方式", result);
    }
    #endregion 设置接线方式》

    #region 《设置闭环模式
    [ObservableProperty]
    private CloseLoopMode[] itemsCloseLoopMode = (CloseLoopMode[])Enum.GetValues(typeof(CloseLoopMode));
    [ObservableProperty]
    private byte loopModeIndex;
    partial void OnLoopModeIndexChanging(byte value)
    {
        SetCloseLoopMode(value);
    }

    private async void SetCloseLoopMode(byte value)
    {
        var result = await Task.Run(() => DKS?.ACS.SetClosedLoop((CloseLoopMode)value, (HarmonicMode)HarmonicModeIndex));

        UpdateInfoBar("设置闭环模式", result);
    }
    #endregion 设置闭环模式》

    #region 《设置谐波模式
    [ObservableProperty]
    private HarmonicMode[] itemsHarmonicMode = (HarmonicMode[])Enum.GetValues(typeof(HarmonicMode));
    [ObservableProperty]
    private byte harmonicModeIndex;
    partial void OnHarmonicModeIndexChanging(byte value)
    {
        SetHarmonicMode(value);
    }
    partial void OnHarmonicModeIndexChanged(byte value)
    {
        // SetHarmonicMode(value);

    }

    private async void SetHarmonicMode(byte harmonicMode)
    {
        var result = await Task.Run(() => DKS?.ACS.SetHarmonicMode((HarmonicMode)harmonicMode, (CloseLoopMode)loopModeIndex));

        UpdateInfoBar("设置谐波模式", result);
    }
    #endregion 设置谐波模式》

    [ObservableProperty]
    private QP_Mode[] itemsQPMode = (QP_Mode[])Enum.GetValues(typeof(QP_Mode));
    [ObservableProperty]
    private QP_Mode qP_Mode;

    [ObservableProperty]
    private RangeSwitchMode[] itemsRangeSwitchMode = (RangeSwitchMode[])Enum.GetValues(typeof(RangeSwitchMode));
    [ObservableProperty]
    private RangeSwitchMode rangeSwitchMode = RangeSwitchMode.Manual;
    #endregion 数据区》

    #region 《操作区
    #region 《设置
    /// <summary>
    /// 指示交流源是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_ACS;
    partial void OnIsEnabled_ACSChanged(bool value)
    {
        switch (value)
        {
            case true: ModuleVisibility_ACS = Visibility.Visible; break;
            case false: ModuleVisibility_ACS = Visibility.Collapsed; break;
        }
    }
    [ObservableProperty]
    private Visibility moduleVisibility_ACS = Visibility.Collapsed;
    /// <summary>
    /// 指示直流源是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_DCS;
    [ObservableProperty]
    private Visibility moduleVisibility_DCS = Visibility.Collapsed;
    partial void OnIsEnabled_DCSChanged(bool value)
    {
        switch (value)
        {
            case true: ModuleVisibility_DCS = Visibility.Visible; break;
            case false: ModuleVisibility_DCS = Visibility.Collapsed; break;
        }
    }
    /// <summary>
    /// 指示交流标准表是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_ACM;
    [ObservableProperty]
    private Visibility moduleVisibility_ACM = Visibility.Collapsed;
    partial void OnIsEnabled_ACMChanged(bool value)
    {
        switch (value)
        {
            case true: ModuleVisibility_ACM = Visibility.Visible; break;
            case false: ModuleVisibility_ACM = Visibility.Collapsed; break;
        }
    }

    /// <summary>
    /// 指示直流表功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_DCM;
    [ObservableProperty]
    private Visibility moduleVisibility_DCM = Visibility.Collapsed;
    partial void OnIsEnabled_DCMChanged(bool value)
    {
        switch (value)
        {
            case true: ModuleVisibility_DCM = Visibility.Visible; break;
            case false: ModuleVisibility_DCM = Visibility.Collapsed; break;
        }
    }
    /// <summary>
    /// 指示电能校验功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_EQP;
    [ObservableProperty]
    private Visibility moduleVisibility_EPQ = Visibility.Collapsed;
    partial void OnIsEnabled_EQPChanged(bool value)
    {
        switch (value)
        {
            case true: ModuleVisibility_EPQ = Visibility.Visible; break;
            case false: ModuleVisibility_EPQ = Visibility.Collapsed; break;
        }
    }

    /// <summary>
    /// 指示校准功能是否激活
    /// </summary>
    [ObservableProperty]
    private bool isEnabled_Calibrate;
    [ObservableProperty]
    private Visibility moduleVisibility_Calibrate = Visibility.Collapsed;
    partial void OnIsEnabled_CalibrateChanged(bool value)
    {
        switch (value)
        {
            case true: ModuleVisibility_Calibrate = Visibility.Visible; break;
            case false: ModuleVisibility_Calibrate = Visibility.Collapsed; break;
        }
    }
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

    [ObservableProperty]
    private byte settingID;
    [ObservableProperty]
    private string? settingSN;
    [ObservableProperty]
    private ushort settingBaudRate;
    [RelayCommand]
    private async void SetDeviceInfo()
    {
        var pwd = new char[] { '6', '3', '8', '3', '6' };
        var result = await Task.Run(() => DKS?.Settings.SetDeviceInfo(pwd, settingID, settingSN ?? string.Empty));
        UpdateInfoBar("设置装置信息", result);
    }
    [RelayCommand]
    private async void SetBaudRate()
    {
        var result = await Task.Run(() => DKS?.Settings.SetBaudRate(settingBaudRate));
        UpdateInfoBar("设置装置波特率", result);
    }
    #endregion 设置》

    #region 《交流源
    [ObservableProperty]
    private Channels[] channels = (Channels[])Enum.GetValues(typeof(Channels));
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
    private Visibility visibility_ACS = Visibility.Collapsed;

    /// <summary>
    /// 通道 1 的幅值设定方法
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute_1))]
    private void SetAmplitude_1()
    {
        Task.Run(() => DKS?.ACS.SetAmplitude(channel_1, setAmplitudeValue_1));
    }
    [RelayCommand(CanExecute = nameof(CanExecute_2))]
    private void SetAmplitude_2()
    {
        Task.Run(() => DKS?.ACS.SetAmplitude(channel_2, setAmplitudeValue_2));
    }
    [RelayCommand(CanExecute = nameof(CanExecute_Dual))]
    private void SetAmplitude_Dual()
    {
        SetAmplitude_1();
        SetAmplitude_2();
    }
    private bool CanExecute_1()
    {
        return channel_1 != 0;
    }
    private bool CanExecute_2()
    {
        return channel_2 != 0;
    }
    private bool CanExecute_Dual()
    {
        return channel_1 != 0 && channel_2 != 0;
    }
    #endregion 交流源》

    #region 《直流电压电流幅值设置
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetAmplitude_DCUCommand))]
    private byte rangeIndex_DCU;
    partial void OnRangeIndex_DCUChanged(byte value) => Task.Run(() => DKS?.DCS.SetRange_DCU(value));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetAmplitude_DCICommand))]
    private byte rangeIndex_DCI;
    partial void OnRangeIndex_DCIChanged(byte value) => Task.Run(() => DKS?.DCS.SetRange_DCI(value));

    [ObservableProperty]
    private float value_DCU;
    [RelayCommand(CanExecute = nameof(CanExecute_SetAmplitudeDCU))]
    private void SetAmplitude_DCU()
    {
        Task.Run(() => DKS?.DCS.SetAmplitude_DCU(value_DCU));    //TODO 将档位和幅值一起设置
    }

    private bool CanExecute_SetAmplitudeDCU()
    {
        return Ranges_DCU != null;
    }

    private bool CanExecute_SetAmplitudeDCI()
    {
        return ranges_DCI != null;
    }

    [ObservableProperty]
    private float value_DCI;
    [RelayCommand(CanExecute = nameof(CanExecute_SetAmplitudeDCI))]
    private void SetAmplitude_DCI()
    {
        Task.Run(() => DKS?.DCS.SetAmplitude_DCI(value_DCI));
    }
    [ObservableProperty]
    private Visibility visibility_DCS = Visibility.Collapsed;
    [ObservableProperty]
    private float dCU;
    [ObservableProperty]
    private float dCI;
    #endregion 直流电压电流幅值设置》

    #region 《直流表操作
    [ObservableProperty]
    private float dCMU;
    [ObservableProperty]
    private float dCMI;
    [ObservableProperty]
    private Visibility visibility_DCM = Visibility.Collapsed;
    #endregion 直流表操作》

    #region 《选择交流电压档位索引值
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MaxValue_U))]
    [NotifyPropertyChangedFor(nameof(SmallChange_U))]
    [NotifyPropertyChangedFor(nameof(LargeChange_U))]
    private byte rangeIndex_Ua;
    [ObservableProperty]
    private float maxValue_U;
    public float SmallChange_U => (float)((ranges_ACU != null && rangeIndex_Ua != 0XFF) ? ranges_ACU[rangeIndex_Ua] * 0.1 : 0);
    public float LargeChange_U => (float)((ranges_ACU != null && rangeIndex_Ua != 0XFF) ? ranges_ACU[rangeIndex_Ua] * 0.2 : 0);

    private async void UpdateRangesIndex()
    {
        await Task.Run(() => Thread.Sleep(1000));
        RangeIndex_Ua = DKS?.ACS.RangeIndex_Ua ?? 0;
        RangeIndex_Ia = DKS?.ACS.RangeIndex_Ia ?? 0;
    }

    partial void OnRangeIndex_UaChanged(byte value)
    {
        if (value == 0xff)
        {
            return;
        }
        SetRange_ACU(value);
        UpdateRangesIndex();
        MaxValue_U = (float)((ranges_ACU != null && rangeIndex_Ua != 0XFF) ? ranges_ACU[rangeIndex_Ua] * 1.2 : 0);
    }

    private async void SetRange_ACU(byte index)
    {
        var result = await Task.Run(() => DKS?.ACS.SetRanges(index, rangeIndex_Ia));
        UpdateInfoBar("设置交流电压档位", result);
    }
    #endregion 电压档位索引值》

    #region 《选择交流电流档位索引值
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MaxValue_I))]
    [NotifyPropertyChangedFor(nameof(SmallChange_I))]
    [NotifyPropertyChangedFor(nameof(LargeChange_I))]
    private byte rangeIndex_Ia;
    [ObservableProperty]
    private float maxValue_I;
    public float SmallChange_I => (float)((ranges_ACI != null && rangeIndex_Ia != 0XFF) ? ranges_ACI[rangeIndex_Ia] * 0.1 : 0);
    public float LargeChange_I => (float)((ranges_ACI != null && rangeIndex_Ia != 0XFF) ? ranges_ACI[rangeIndex_Ia] * 0.2 : 0);

    partial void OnRangeIndex_IaChanged(byte value)
    {
        if (value == 0xff)
        {
            return;
        }
        SetRange_ACI(value);
        UpdateRangesIndex();

        MaxValue_I = (float)((ranges_ACI != null && rangeIndex_Ia != 0XFF) ? ranges_ACI[rangeIndex_Ia] * 1.2 : 0);
    }
    private async void SetRange_ACI(byte index)
    {
        var result = await Task.Run(() => DKS?.ACS.SetRanges(rangeIndex_Ua, index));
        UpdateInfoBar("设置交流电流档位", result);
    }
    #endregion 电流档位索引值》

    #region 《功能模块开关操作
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

    [ObservableProperty]
    private Brush progressRingColor_ACS;
    [ObservableProperty]
    private Brush progressRingColor_DCS;
    [ObservableProperty]
    private Brush progressRingColor_DCM;
    [ObservableProperty]
    private Brush progressRingColor_EPQ;

    private readonly Brush red = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
    private readonly Brush green = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
    #region <- ACS
    partial void OnIsOpenACSChanging(bool value)
    {
        if (!value)
        {

        }
    }
    partial void OnIsOpenACSChanged(bool value)
    {
        if (value)
        {
            Visibility_ACS = Visibility.Visible;
            ReadDataACS();
        }
        else
        {
            Visibility_ACS = Visibility.Collapsed;
            StopReadACS();
        }
    }

    private async void ReadDataACS()
    {
        UpdateRangesIndex();       
        do
        {
            var result = await Task.Run(() => DKS?.ACS.ReadData());
            if (result?.IsSuccess ?? false)
            {
                UA = DKS?.ACS.UA ?? 0;
                UB = DKS?.ACS.UB ?? 0;
                UC = DKS?.ACS.UC ?? 0;
                UX = DKS?.ACS.UX ?? 0;
                IA = DKS?.ACS.IA ?? 0;
                IB = DKS?.ACS.IB ?? 0;
                IC = DKS?.ACS.IC ?? 0;
                IX = DKS?.ACS.IX ?? 0;
                FaiUA = DKS?.ACS.FAI_UA ?? 0;
                FaiUB = DKS?.ACS.FAI_UB ?? 0;
                FaiUC = DKS?.ACS.FAI_UC ?? 0;
                FaiIA = DKS?.ACS.FAI_IA ?? 0;
                FaiIB = DKS?.ACS.FAI_IB ?? 0;
                FaiIC = DKS?.ACS.FAI_IC ?? 0;
                PA = DKS?.ACS.PA ?? 0;
                PB = DKS?.ACS.PB ?? 0;
                PC = DKS?.ACS.PC ?? 0;
                PX = DKS?.ACS.PX ?? 0;
                P = DKS?.ACS.P ?? 0;
                QA = DKS?.ACS.QA ?? 0;
                QB = DKS?.ACS.QB ?? 0;
                QC = DKS?.ACS.QC ?? 0;
                QX = DKS?.ACS.QX ?? 0;
                Q = DKS?.ACS.Q ?? 0;
                SA = DKS?.ACS.SA ?? 0;
                SB = DKS?.ACS.SB ?? 0;
                SC = DKS?.ACS.SC ?? 0;
                SX = DKS?.ACS.SX ?? 0;
                S = DKS?.ACS.S ?? 0;
                PFA = DKS?.ACS.PFA ?? 0;
                PFB = DKS?.ACS.PFB ?? 0;
                PFC = DKS?.ACS.PFC ?? 0;
                PFX = DKS?.ACS.PFX ?? 0;
                PF = DKS?.ACS.PF ?? 0;
                FreqC = DKS?.ACS.Freq_C ?? 0;
                FreqX = DKS?.ACS.Freq_X ?? 0;
                Freq = DKS?.ACS.Freq ?? 0;
                ProgressRingColor_ACS = green;
            }
            else
            {
                ProgressRingColor_ACS = red;
            }
            UpdateInfoBar("Read ACS", result);
        } while (isOpenACS);
    }
    
    private async void StopReadACS()
    {
        await Task.Run(() => DKS?.ACS.Stop());
    }
    #endregion ACS ->

    partial void OnIsOpenDCSChanged(bool value)
    {
        if (value)
        {
            ReadDataDCS();
            Visibility_DCS = Visibility.Visible;
        }
        else
        {
            Task.Run(() =>
            {
                DKS?.DCS.Stop_DCI();
                DKS?.DCS.Stop_DCU();
                DKS?.DCS.Stop_DCR();
            });
            Visibility_DCS = Visibility.Collapsed;
        }
    }
    partial void OnIsOpenEQPChanged(bool value)
    {
        if (value)
        {
            ReadDataEQP();
          
        }
        else
        {
        }
    }
    partial void OnIsOpenDCMChanged(bool value)
    {
        if (value)
        {
            ReadDataDCM();
            Visibility_DCM = Visibility.Visible;
        }
        else
        {
            Visibility_DCM = Visibility.Collapsed;
        }
    }

    private async void ReadDataDCS()
    {
        while (IsOpenDCS)
        {
            var result = await Task.Run(() => DKS?.DCS.ReadData());
            if (result?.IsSuccess ?? false)
            {
                DCU = DKS?.DCS.DCU ?? 0;
                DCI = DKS?.DCS.DCI ?? 0;
                ProgressRingColor_DCS = green;
            }
            else
            {
                ProgressRingColor_DCS = red;
            }

            UpdateInfoBar("Read DCS", result);
        }
    }
    private async void ReadDataDCM()
    {
        while (IsOpenDCM)
        {
            var result = await Task.Run(() => DKS?.DCM.ReadData());
            if (result?.IsSuccess ?? false)
            {
                DCMU = DKS?.DCM.DCMU ?? 0;
                DCMI = DKS?.DCM.DCMI ?? 0;
                ProgressRingColor_DCM = green;
            }
            else
            {
                ProgressRingColor_DCM = red;
            }
            UpdateInfoBar("Read DCM", result);
        }
    }
    private async void ReadDataEQP()
    {
        while (IsOpenEQP)
        {
            var result = await Task.Run(() => DKS?.EPQ.ReadData());
            if (result?.IsSuccess ?? false)
            {
                ProgressRingColor_EPQ = green;
            }
            else
            {
                ProgressRingColor_EPQ = red;
            }

            UpdateInfoBar("Read EPQ", result);
        }
    }
    #endregion 功能模块开关操作》


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
    public InfoBarSeverity infoBarSeverity = InfoBarSeverity.Success;
    #endregion

    #region 串口开关绑定
    /// <summary>
    /// 总开关的状态属性
    /// </summary>
    [ObservableProperty]
    private bool isOn_PortSwitch;
    partial void OnIsOn_PortSwitchChanging(bool value)
    {
        IsOpenACS = false;
        IsOpenDCM = false;
        IsOpenDCS = false;
        IsOpenEQP = false;
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
    private List<string> portNames = SerialPort.GetPortNames().ToList();

    [ObservableProperty]
    private int[]? baudRates = new int[] { 9600, 19200, 115200 };

    [ObservableProperty]
    private Parity[] paritys = (Parity[])Enum.GetValues(typeof(Parity));

    [ObservableProperty]
    private StopBits[] stopBits = (StopBits[])Enum.GetValues(typeof(StopBits));

    [ObservableProperty]
    private Handshake[] handShakes = (Handshake[])Enum.GetValues(typeof(Handshake));

    [ObservableProperty]
    private int[] dataBits = new int[] { 5, 6, 7, 8 };

    /// <summary>
    /// 串口开关打开时，显示超时设置选项
    /// </summary>
    [ObservableProperty]
    private Visibility visibilityFollowToggleSwitch = Visibility.Collapsed;

    /// <summary>
    /// 指示打开串口后禁用的状态
    /// </summary>
    [ObservableProperty]
    private bool disableSerialPortEdit = true;

    /// <summary>
    /// 当点击开关时显示转圈圈
    /// </summary>
    [ObservableProperty]
    private bool progressRingWhenSwitching;

    [RelayCommand]
    private void RefreshPortNames()
    {
        PortNames.Clear();
        PortNames = SerialPort.GetPortNames().ToList();
        PortNames.Sort();
    }
    #endregion

    //[RelayCommand]
    //private void ReadData_ACS()
    //{
    //    DKS?.ACS.ReadData();
    //    UA = DKS?.ACS.UA ?? 0;
    //    UB = DKS?.ACS.UB ?? 0;
    //    UC = DKS?.ACS.UC ?? 0;
    //    UX = DKS?.ACS.UX ?? 0;
    //    IA = DKS?.ACS.IA ?? 0;
    //    IB = DKS?.ACS.IB ?? 0;
    //    IC = DKS?.ACS.IC ?? 0;
    //    IX = DKS?.ACS.IX ?? 0;
    //    FaiUA = DKS?.ACS.FAI_UA ?? 0;
    //    FaiUB = DKS?.ACS.FAI_UB ?? 0;
    //    FaiUC = DKS?.ACS.FAI_UC ?? 0;
    //    FaiIA = DKS?.ACS.FAI_IA ?? 0;
    //    FaiIB = DKS?.ACS.FAI_IB ?? 0;
    //    FaiIC = DKS?.ACS.FAI_IC ?? 0;
    //    PA = DKS?.ACS.PA ?? 0;
    //    PB = DKS?.ACS.PB ?? 0;
    //    PC = DKS?.ACS.PC ?? 0;
    //    PX = DKS?.ACS.PX ?? 0;
    //    P = DKS?.ACS.P ?? 0;
    //    QA = DKS?.ACS.QA ?? 0;
    //    QB = DKS?.ACS.QB ?? 0;
    //    QC = DKS?.ACS.QC ?? 0;
    //    QX = DKS?.ACS.QX ?? 0;
    //    Q = DKS?.ACS.Q ?? 0;
    //    SA = DKS?.ACS.SA ?? 0;
    //    SB = DKS?.ACS.SB ?? 0;
    //    SC = DKS?.ACS.SC ?? 0;
    //    SX = DKS?.ACS.SX ?? 0;
    //    S = DKS?.ACS.S ?? 0;
    //    PFA = DKS?.ACS.PFA ?? 0;
    //    PFB = DKS?.ACS.PFB ?? 0;
    //    PFC = DKS?.ACS.PFC ?? 0;
    //    PFX = DKS?.ACS.PFX ?? 0;
    //    PF = DKS?.ACS.PF ?? 0;
    //    FreqC = DKS?.ACS.Freq_C ?? 0;
    //    FreqX = DKS?.ACS.Freq_X ?? 0;
    //    Freq = DKS?.ACS.Freq ?? 0;
    //}

    /// <summary>
    /// 打开串口开关时，设备实例化
    /// </summary>
    /// <param name="isOn"></param>
    public async void ToggleSwitch_Toggled(bool isOn)
    {
        switch (isOn)
        {
            //正在打开串口
            case true:

                //判断是否选择型号和串口号
                if (ModelSelected == null || PortName == null)
                {
                    InfoBarSeverity = InfoBarSeverity.Warning;
                    IsInfobarShow = true;
                    InfobarTitle = "Warning";
                    InfobarMessage = "还没有选择任何设备型号";
                    IsOn_PortSwitch = false;
                    return;
                }

                //实例化对象
                DKS = new DKStandardSource(Enum.Parse<DKCommunicationNET.Models>(ModelSelected), PortName, BaudRate, ID);

                //打开串口并发送握手报文               
                var result = DKS.Open();

                if (result.IsSuccess)
                {
                    InfoBarSeverity = InfoBarSeverity.Success;
                    IsInfobarShow = true;
                    InfobarTitle = "Success";
                    InfobarMessage = "【打开串口成功】";
                    IsOn_PortSwitch = true;

                    //开始转圈圈
                    ProgressRingWhenSwitching = true;

                    //*****异步联机命令
                    var res1 = await Task.Run(() =>
                     {
                         return DKS.HandShake();
                     });
                    if (res1.IsSuccess)
                    {
                        InfobarMessage += "【获取设备信息成功】";
                        IsEnabled_ACS = DKS.Settings.IsEnabled_ACS;
                        IsEnabled_ACM = DKS.Settings.IsEnabled_ACM;
                        IsEnabled_DCS = DKS.Settings.IsEnabled_DCS;
                        IsEnabled_DCM = DKS.Settings.IsEnabled_DCM;
                        IsEnabled_EQP = DKS.Settings.IsEnabled_EPQ;
                        IsEnabled_ACM_Cap = DKS.Settings.IsEnabled_ACM_Cap;
                        IsEnabled_DCS_AUX = DKS.Settings.IsEnabled_DCS_AUX;
                        IsEnabled_DCM_RIP = DKS.Settings.IsEnabled_DCM_RIP;
                        IsEnabled_IO = DKS.Settings.IsEnabled_IO;
                        IsEnabled_DualFreqs = DKS.Settings.IsEnabled_DualFreqs;
                        IsEnabled_IProtect = DKS.Settings.IsEnabled_IProtect;
                        IsEnabled_PST = DKS.Settings.IsEnabled_PST;
                        IsEnabled_YX = DKS.Settings.IsEnabled_YX;
                        IsEnabled_HF = DKS.Settings.IsEnabled_HF;
                        IsEnabled_PWM = DKS.Settings.IsEnabled_PWM;
                        IsEnabled_PPS = DKS.Settings.IsEnabled_PPS;
                        IsEnabled_Calibrate = DKS.Settings.IsEnabled_Calibrate;
                        SN = DKS.Settings.SN;
                        Model = DKS.Settings.Model;
                        FirmWare = DKS.Settings.Firmware;
                    }
                    else
                    {
                        InfoBarSeverity = InfoBarSeverity.Error;
                        InfobarTitle = "Error";
                        InfobarMessage += $"【联机失败，功能已禁用：{res1.Message}】";
                        ProgressRingWhenSwitching = false;
                        IsOn_PortSwitch = false;
                        return;
                    }
                    //*****异步获取交流源档位
                    var res2 = await Task.Run(() =>
                      {
                          return DKS.ACS.GetRanges();
                      });

                    if (res2 != null && !res2.IsSuccess)
                    {
                        if (infoBarSeverity < InfoBarSeverity.Warning)
                        {
                            InfoBarSeverity = InfoBarSeverity.Warning;
                            InfobarTitle = "Warning";
                        }
                        InfobarMessage += "【获取交流源档位失败】";
                    }
                    else if (res2 != null && res2.IsSuccess)
                    {
                        InfobarMessage += "【获取交流源档位成功】";
                    }

                    //*****异步获取直流源档位
                    var res3 = await Task.Run(() =>
                    {
                        return DKS.DCS.GetRanges();
                    });

                    if (res3 != null && !res3.IsSuccess)
                    {
                        if (infoBarSeverity < InfoBarSeverity.Warning)
                        {
                            InfobarTitle = "Warning";
                            InfoBarSeverity = InfoBarSeverity.Warning;
                        }
                        InfobarMessage += "【获取直流源档位失败】";
                    }
                    else if (res3 != null && res3.IsSuccess)
                    {
                        InfobarMessage += "【获取直流源档位成功】";
                    }

                    //获取直流表档位
                    var res4 = await Task.Run(() => DKS.Settings.IsEnabled_DCM ? DKS.DCM.GetRanges() : null);

                    if (res4 != null && !res4.IsSuccess)
                    {
                        if (infoBarSeverity < InfoBarSeverity.Warning)
                        {
                            InfobarTitle = "Warning";
                            InfoBarSeverity = InfoBarSeverity.Warning;
                        }
                        InfobarMessage += "【获取直流表档位失败】";
                    }
                    else if (res4 != null && res4.IsSuccess)
                    {
                        InfobarMessage += "【获取直流表档位成功】";
                    }

                    //交流电压档位集合初始化

                    if ((DKS.ACS.Ranges_ACU != null) && (!Ranges_ACU?.SequenceEqual(DKS.ACS.Ranges_ACU) ?? true))
                    {
                        Ranges_ACU = DKS.ACS.Ranges_ACU;
                    }

                    //交流电流档位集合初始化
                    if ((DKS.ACS.Ranges_ACI != null) && (!Ranges_ACI?.SequenceEqual(DKS.ACS.Ranges_ACI) ?? true))
                    {
                        Ranges_ACI = DKS.ACS.Ranges_ACI;
                    }

                    //直流源量程
                    if ((DKS.DCS.Ranges_DCI != null) && (!Ranges_DCI?.SequenceEqual(DKS.DCS.Ranges_DCI) ?? true))
                    {
                        Ranges_DCI = DKS.DCS.Ranges_DCI;
                    }
                    if ((DKS.DCS.Ranges_DCU != null) && (!Ranges_DCU?.SequenceEqual(DKS.DCS.Ranges_DCU) ?? true))
                    {
                        Ranges_DCU = DKS.DCS.Ranges_DCU;
                    }

                    //直流表量程
                    if ((DKS.DCM.Ranges_DCMI != null) && (!Ranges_DCMI?.SequenceEqual(DKS.DCM.Ranges_DCMI) ?? true))
                    {
                        Ranges_DCMI = DKS.DCM.Ranges_DCMI;
                    }
                    if ((DKS.DCM.Ranges_DCMU != null) && (!Ranges_DCMU?.SequenceEqual(DKS.DCM.Ranges_DCMU) ?? true))
                    {
                        Ranges_DCMU = DKS.DCM.Ranges_DCMU;
                    }
                    //禁用参数设置控件
                    DisableSerialPortEdit = false;
                    //打开串口超时参数设置控件
                    VisibilityFollowToggleSwitch = Visibility.Visible;
                    //停止转圈圈
                    ProgressRingWhenSwitching = false;
                }
                else
                {
                    InfoBarSeverity = InfoBarSeverity.Error;
                    IsInfobarShow = true;
                    InfobarTitle = "Failed";
                    InfobarMessage = result.Message;
                    IsOn_PortSwitch = false;
                }
                break;

            //正在关闭串口
            case false:
                DisableSerialPortEdit = true;
                VisibilityFollowToggleSwitch = Visibility.Collapsed;
                IsOn_PortSwitch = false;
                DKS?.Close();
                break;
        }
    }


    #region 自动界面处理方法

    public void UpdateInfoBar(string title, OperateResult? result)
    {
        if (result?.IsSuccess ?? false)
        {
            //InfoBarSeverity = InfoBarSeverity. Success;
        }
        else if (!result?.IsSuccess ?? false)
        {
            IsInfobarShow = true;
            InfoBarSeverity = InfoBarSeverity.Error;
            InfobarMessage = title + "：" + result?.Message;
            InfobarTitle = DateTime.Now.ToString();
        }
        else if (result is null)
        {
            InfoBarSeverity = InfoBarSeverity.Informational;
        }
    }
    /// <summary>
    /// 交流电压显示格式
    /// </summary>
    [ObservableProperty]
    private DecimalFormatter? formatter_ACU;
    private void SetNumberBoxNumberFormatter_ACU()
    {
        var rounder = new IncrementNumberRounder
        {
            Increment = 0.001,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };
        formatter_ACU = new()
        {
            IntegerDigits = 1,
            FractionDigits = 3,
            NumberRounder = rounder
        };
    }
    /// <summary>
    /// 交流电流显示格式
    /// </summary>
    [ObservableProperty]
    private DecimalFormatter? formatter_ACI;
    private void SetNumberBoxNumberFormatter_ACI()
    {
        var rounder = new IncrementNumberRounder
        {
            Increment = 0.0001,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };
        formatter_ACI = new()
        {
            IntegerDigits = 1,
            FractionDigits = 4,
            NumberRounder = rounder
        };
    }

    [ObservableProperty]
    private DecimalFormatter? formatter_PQ;
    private void SetNumberBoxNumberFormatter_PQ()
    {
        var rounder = new IncrementNumberRounder
        {
            Increment = 0.1,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };
        formatter_PQ = new()
        {
            IntegerDigits = 1,
            FractionDigits = 1,
            NumberRounder = rounder
        };
    }

    [ObservableProperty]
    private DecimalFormatter? formatter_Freq;
    private void SetNumberBoxNumberFormatter_Freq()
    {
        var rounder = new IncrementNumberRounder
        {
            Increment = 0.0001,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };
        formatter_Freq = new()
        {
            IntegerDigits = 1,
            FractionDigits = 4,
            NumberRounder = rounder
        };
    }

    [ObservableProperty]
    private DecimalFormatter? formatter_PF;
    private void SetNumberBoxNumberFormatter_PF()
    {
        var rounder = new IncrementNumberRounder
        {
            Increment = 0.00001,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };
        formatter_PF = new()
        {
            IntegerDigits = 1,
            FractionDigits = 5,
            NumberRounder = rounder
        };
    }
    #endregion

}
