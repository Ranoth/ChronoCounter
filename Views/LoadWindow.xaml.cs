using ChronoCounter.DBModels;
using ChronoCounter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChronoCounter.Views
{
    /// <summary>
    /// Interaction logic for LoadWindow.xaml
    /// </summary>
    public partial class LoadWindow : Window
    {

        private LoadWindowViewModel vm;
        public List<Session> Sessions { get; }
        public Session Selected { get; set; }
        public LoadWindow(List<Session> sessions, Session selected)
        {
            InitializeComponent();
            var vm = new LoadWindowViewModel(sessions, selected, this);
            DataContext = vm;
            this.vm = vm;

            Sessions = sessions;

            Closing += vm.UpdateListAndLoad;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            vm.Changed = true;
        }
    }
}
