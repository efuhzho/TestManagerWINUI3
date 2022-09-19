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
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Ua , ( float )args. NewValue));
    }

    private async void UB_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Ub , ( float )args. NewValue));
    }

    private async void UC_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Uc , ( float )args. NewValue));
    }

    private async void UX_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Ux , ( float )args. NewValue));
    }

    private async void IA_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Ia , ( float )args. NewValue));
    }

    private async void IB_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Ib , ( float )args. NewValue));
    }

    private async void IC_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Ic , ( float )args. NewValue));
    }

    private async void IX_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetAmplitude(Channels. Ix , ( float )args. NewValue));
    }

   

    private async void FaiUB_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetPhase(Channels. Ub , ( float )args. NewValue));
    }

    private async void FaiUC_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetPhase(Channels. Uc , ( float )args. NewValue));
    }

    private async void FaiIA_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetPhase(Channels. Ia , ( float )args. NewValue));
    }

    private async void FaiIB_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetPhase(Channels. Ib , ( float )args. NewValue));
    }

    private async void FaiIC_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetPhase(Channels. Ic , ( float )args. NewValue));

    }

    private async void FaiIX_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetPhase(Channels. Ix , ( float )args. NewValue));
    }

    private async void Freq_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?. ACS. SetFrequency(( float )args. NewValue));
    }

    private async void FreqX_ValueChanged (NumberBox sender , NumberBoxValueChangedEventArgs args)
    {
        await Task. Run(() => ViewModel. DKS?.ACS. SetFrequency(ViewModel.Freq,( float )args.NewValue));
    }
}
