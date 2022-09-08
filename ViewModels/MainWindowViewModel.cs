using ChronoCounter.DBContexts;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace ChronoCounter.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase? currentViewModel;
        public MainWindowViewModel()
        {
            HandshakeDBAsync();
            CurrentViewModel = new ChronoCounterViewModel();
        }
        private async Task HandshakeDBAsync() //Workaround because DB lags at first connection
        {
            await Task.Run(() =>
            {
                using (SessionsDBdbContext context = new())
                {
                    var _ = context.Session.Select(x => x.Id).FirstOrDefaultAsync();
                }
            });
        }
    }
}
