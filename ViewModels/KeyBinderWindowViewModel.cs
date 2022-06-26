using ChronoCounter.Messages;
using ChronoCounter.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Input;
using WPFUtilsBox.HotKeyer;

namespace ChronoCounter.ViewModels
{
    public partial class KeyBinderWindowViewModel : ViewModelBase
    {
        private readonly KeyBinderWindow view;
        private HotKey? keyToBind;

        public HotKey? KeyToBind
        {
            get => keyToBind;
            set
            {
                SetProperty(ref keyToBind, value);
                WeakReferenceMessenger.Default.Send(new KeyToBindSplitMessage(keyToBind));
            }
        }
        public KeyBinderWindowViewModel(KeyBinderWindow view, HotKey keyToBind)
        {
            this.view = view;
            this.keyToBind = keyToBind;
        }

        public void OnKeyUp(object? sender, KeyEventArgs e)
        {
            var key = KeyBinder.GatherHotKey(e);
            if (key != null) KeyToBind = key;
            view.Close();
        }

        [RelayCommand]
        private void ClearButton()
        {
            if (KeyToBind.Key != Key.None)
            {
                if (MessageBox.Show($"Do you truly wish to clear the bound key ?",
                    "Clear Keybind",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    KeyToBind = new HotKey(Key.None, ModifierKeys.None);
                    view.Close();
                }
            }
        }
        [RelayCommand]
        private void CancelButton()
        {
            view.Close();
        }
    }
}
