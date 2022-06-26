using CommunityToolkit.Mvvm.ComponentModel;

namespace ChronoCounter.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase? currentViewModel;
        public MainWindowViewModel()
        {
            CurrentViewModel = new ChronoCounterViewModel();
        }
    }
}
