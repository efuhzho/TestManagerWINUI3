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

    private async void UA_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(( float )args. NewValue , ViewModel. UB , ViewModel. UC , ViewModel. IA , ViewModel. IB , ViewModel. IC));
    }

    private async void UB_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(ViewModel. UA , ( float )args. NewValue , ViewModel. UC , ViewModel. IA , ViewModel. IB , ViewModel. IC));
    }
}
