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
        Array. Sort(ViewModel. DandickModels);       
    }

    /// <summary>
    /// 标准源开关事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    private async void CloseLoopMode_SelectionChanged (object sender , SelectionChangedEventArgs e)
    {
        if ( ViewModel. DKS != null )
        {
            var result = await Task. Run(() => ViewModel. DKS. ACS. SetClosedLoop(ViewModel. CloseLoop));
            if ( result. IsSuccess )
            {
                ViewModel. InfobarTitle = $"设置闭环模式成功";
                return;
            }
            else
            {
                ViewModel. InfobarTitle = $"设置闭环模式失败";
            }
        }
    }

    /// <summary>
    /// 谐波模式设置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ComboBox_SelectionChanged (object sender , SelectionChangedEventArgs e)
    {
        if ( ViewModel. DKS != null )
        {
            var result = await Task. Run(() => ViewModel. DKS. ACS. SetHarmonicMode(ViewModel. HarmonicMode));
            if ( result. IsSuccess )
            {
                ViewModel. InfobarTitle = $"设置谐波模式成功";
                return;
            }
            else
            {
                ViewModel. InfobarTitle = $"设置谐波模式失败";
            }
        }
    }
    /// <summary>
    /// 无功计算方法设置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ComboBox_SelectionChanged_1 (object sender , SelectionChangedEventArgs e)
    {

    }

    private void ComboBox_SelectionChanged_2 (object sender , SelectionChangedEventArgs e)
    {

    }

    private async void CbxWireMode_SelectionChanged (object sender , SelectionChangedEventArgs e)
    {
        if (ViewModel. DKS != null )
        {
            var result = await Task. Run(() =>
            {
                return ViewModel. DKS. ACS. SetWireMode(ViewModel.WireMode);
            });
            if ( result. IsSuccess )
            {
                ViewModel. InfobarTitle = $"设置接线方式成功";
                return;
            }
            else
            {
                ViewModel. InfobarTitle = $"设置接线方式失败";
            }
        }
    }
}
