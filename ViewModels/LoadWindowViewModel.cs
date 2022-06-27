using ChronoCounter.DBContexts;
using ChronoCounter.DBModels;
using ChronoCounter.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChronoCounter.ViewModels
{
    public partial class LoadWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Session> Sessions { get; set; }
        public bool Changed { get; set; }
        [ObservableProperty]
        private Session selected = new();
        private readonly LoadWindow loadWindow;
        private List<Session> sessionsToRemove = new();

        public LoadWindowViewModel(List<Session> sessions, Session selected, LoadWindow loadWindow)
        {
            this.Sessions = new ObservableCollection<Session>(sessions);
            this.Selected = selected;
            this.loadWindow = loadWindow;
        }

        public void UpdateListAndLoad(object? sender, CancelEventArgs e)
        {
            if (Changed && MessageBox.Show("Save changes ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SessionsDBdbContext context = new())
                {
                    context.RemoveRange(sessionsToRemove);
                    context.UpdateRange(Sessions);
                    context.SaveChanges();
                }
            }
            loadWindow.Selected = Selected;
        }

        [RelayCommand]
        private void RemoveSession()
        {
            sessionsToRemove.Add(Selected);
            Sessions.Remove(Selected);
            Changed = true;
        }
    }
}
