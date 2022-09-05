using DKCommunicationNET;
using Microsoft. UI. Xaml. Controls;

using TestManager. ViewModels;
using Windows. UI. Popups;

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
        Array. Sort(ViewModel. DsModels);
        ViewModel. SS_Model = "Hex81";
    }
}
