using ChronoCounter.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFUtilsBox.Styling;

namespace ChronoCounter.Views
{
    /// <summary>
    /// Interaction logic for ChronoCounter.xaml
    /// </summary>
    public partial class ChronoCounter : UserControl
    {
        public ChronoCounter()
        {
            InitializeComponent();
            //toolbar.Loaded += NoOverflowToolbar.ToolbarRemoveOverflowBorder;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ChronoCounterViewModel;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += vm.Timer_Tick;
            vm.Timer = timer;
            Unloaded += vm.UnhookKeyboardGlobal;
        }
    }
}
