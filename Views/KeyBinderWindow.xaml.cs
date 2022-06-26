using ChronoCounter.ViewModels;
using System.Windows;
using WPFUtilsBox.HotKeyer;

namespace ChronoCounter.Views
{
    /// <summary>
    /// Interaction logic for KeyBinderWindow.xaml
    /// </summary>
    public partial class KeyBinderWindow : Window
    {
        public KeyBinderWindow(HotKey hotKey)
        {
            InitializeComponent();
            var viewModel = new KeyBinderWindowViewModel(this, hotKey);
            DataContext = viewModel;
            KeyUp += viewModel.OnKeyUp;
        }
    }
}
