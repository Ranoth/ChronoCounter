using ChronoCounter.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using WPFUtilsBox.GlobalKeyboardHooker;
using WPFUtilsBox.HotKeyer;
using ChronoCounter.Views;
using CommunityToolkit.Mvvm.Messaging;
using ChronoCounter.Messages;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace ChronoCounter.ViewModels
{
    public partial class ChronoCounterViewModel : ViewModelBase
    {
        private GlobalKeyboardHook globalKeyboardHook;
        private Stopwatch stopWatch = new();
        private Chrono chrono = new();
        private TimeSpan totalElapsedTime;
        private HotKey keyBindSplit = new();

        [NotifyCanExecuteChangedFor(nameof(PauseButtonCommand))]
        [ObservableProperty]
        private bool isTiming = false;
        [ObservableProperty]
        private bool isPause = false;
        [ObservableProperty]
        private string splitButtonImg = "/Images/plusButtonIcon.png";
        [ObservableProperty]
        private string splitButtonContent = "Start";
        [ObservableProperty]
        private string bindingSplitButtonContent = "Bind Start / Stop";
        [ObservableProperty]
        private string pauseButtonContent = "Pause";
        [ObservableProperty]
        private string pauseButtonImg = "/Images/pauseIcon.png";
        [ObservableProperty]
        private string totalTimeDisp = "00:00:00.00";
        [ObservableProperty]
        private string bindDefaultTextVisible = "Visible";

        public HotKey KeyBindSplit
        {
            get => keyBindSplit;
            set
            {
                SetProperty(ref keyBindSplit, value);
                SaveBindToXml(value);
            }
        }
        public BindingList<Chrono> Chronos { get; set; } = new();
        public DispatcherTimer Timer { get; set; }
        public bool KeyBindChanging { get; private set; }

        public ChronoCounterViewModel()
        {
            globalKeyboardHook = new();
            globalKeyboardHook.KeyboardPressed += OnKeyboardPressed;

            Chronos.ListChanged += (s, e) =>
            {
                ResetButtonCommand.NotifyCanExecuteChanged();
            };

            WeakReferenceMessenger.Default.Register<KeyToBindSplitMessage>(this, (r, m) =>
            {
                (r as ChronoCounterViewModel).KeyBindSplit = m.Value;
                if (m.Value.Key == Key.None) BindingSplitButtonContent = "Bind Start / Stop";
                else BindingSplitButtonContent = m.Value.ToString();
            });

            LoadKeyBindFromXml();
        }

        private void LoadKeyBindFromXml()
        {
            if (!File.Exists("KeyBinds.xml"))
            {
                var xElement = new XElement("KeyBind");
                new XDocument(xElement).Save("KeyBinds.xml");
            }
            else
            {
                var xDoc = XDocument.Load("KeyBinds.xml");

                var xQuery = from binds in xDoc.Elements("KeyBind")
                             select new HotKey
                             {
                                 Key = (Key)Enum.Parse(typeof(Key), binds.Attribute(nameof(KeyBindSplit.Key)).Value.ToString()),
                                 Modifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), binds.Attribute(nameof(KeyBindSplit.Modifiers)).Value.ToString())
                             };

                KeyBindSplit = xQuery.FirstOrDefault();
                if (KeyBindSplit.Key == Key.None) BindingSplitButtonContent = "Bind Start / Stop";
                else BindingSplitButtonContent = KeyBindSplit.ToString();
            }
        }

        private void SaveBindToXml(HotKey hotKey)
        {
            BindDefaultTextVisible = "Collapsed";

            var xDoc = XDocument.Load("KeyBinds.xml");

            xDoc.Element("KeyBind").SetAttributeValue(nameof(hotKey.Key), hotKey.Key.ToString());
            xDoc.Element("KeyBind").SetAttributeValue(nameof(hotKey.Modifiers), hotKey.Modifiers.ToString());

            xDoc.Save("KeyBinds.xml");
        }

        public void UnhookKeyboardGlobal(object? sender, EventArgs e)
        {
            globalKeyboardHook.Dispose();
        }
        private void OnKeyboardPressed(object? sender, GlobalKeyboardHookEventArgs e)
        {
            var pressedKey = new HotKey(KeyInterop.KeyFromVirtualKey((int)e.KeyboardData.Key), Keyboard.Modifiers);
            if (KeyBindSplit.Key == pressedKey.Key && KeyBindSplit.Modifiers == pressedKey.Modifiers && !e.KeyboardState.HasFlag(GlobalKeyboardHook.KeyboardState.KeyUp))
            {
                IsTiming = !IsTiming;
                ActuateChrono();
            }
        }

        private void ActuateChrono()
        {
            IsPause = false;
            if (IsTiming)
            {
                stopWatch.Reset();
                Timer.Start();
                stopWatch.Start();
                chrono = new Chrono();
                chrono.Name = "Split " + chrono.Id;
                Chronos.Add(chrono);
                SplitButtonContent = "Stop";
                SplitButtonImg = "/Images/stopPlaybackIcon.png";
            }
            else if (!IsTiming)
            {
                stopWatch.Stop();
                Timer.Stop();

                var tempTotTime = TimeSpan.Zero;
                foreach (var item in Chronos)
                    tempTotTime = tempTotTime.Add(item.Time);

                totalElapsedTime = tempTotTime;
                SetTotalTimeDisp();
                SplitButtonContent = "Start";
                SplitButtonImg = "/Images/plusButtonIcon.png";
            }
        }
        public void Timer_Tick(object sender, EventArgs e)
        {
            var elapsedTime = stopWatch.Elapsed;
            chrono.Time = elapsedTime;
        }

        [RelayCommand]
        private void RemoveChrono(int id)
        {
            var chronoToRemove = Chronos.FirstOrDefault(x => x.Id == id);
            totalElapsedTime = totalElapsedTime.Subtract(chronoToRemove.Time);
            SetTotalTimeDisp();
            Chronos.Remove(chronoToRemove);
        }

        [RelayCommand]
        private void SplitButton()
        {
            ActuateChrono();
        }

        [RelayCommand]
        private void BindingSplitButton()
        {
            KeyBindChanging = true;
            var keyBindDialog = new KeyBinderWindow(KeyBindSplit);
            keyBindDialog.ShowDialog();
            KeyBindChanging = false;
        }

        [RelayCommand(CanExecute = nameof(CanPause))]
        private void PauseButton()
        {
            if (IsPause && IsTiming)
            {
                stopWatch.Stop();
                Timer.Stop();
                PauseButtonImg = "/Images/playIcon.png";
                PauseButtonContent = "Play";
            }
            else if (!IsPause && IsTiming)
            {
                stopWatch.Start();
                Timer.Start();
                PauseButtonImg = "/Images/pauseIcon.png";
                PauseButtonContent = "Pause";
            }
        }
        private bool CanPause()
        {
            return IsTiming;
        }

        [RelayCommand(CanExecute = nameof(CanReset))]
        private void ResetButton()
        {
            if (Timer != null && stopWatch != null)
            {
                stopWatch.Stop();
                Timer.Stop();


                totalElapsedTime = TimeSpan.Zero;
                SetTotalTimeDisp();

                IsTiming = false;
                IsPause = false;

                SplitButtonContent = "Start";
                SplitButtonImg = "/Images/plusButtonIcon.png";

                PauseButtonImg = "/Images/pauseIcon.png";
                PauseButtonContent = "Pause";
            }
            Chronos.Clear();

            Chrono.Counter = 1;
            ResetButtonCommand.NotifyCanExecuteChanged();
        }

        private bool CanReset()
        {
            return Chrono.Counter > 1;
        }

        private void SetTotalTimeDisp()
        {
            TotalTimeDisp = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", totalElapsedTime.Hours, totalElapsedTime.Minutes, totalElapsedTime.Seconds, totalElapsedTime.Milliseconds / 10);
        }
    }
}
