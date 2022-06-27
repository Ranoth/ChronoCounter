using ChronoCounter.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChronoCounter.Models
{
    public partial class FunctionChrono : ViewModelBase
    {
        public static int Counter { get; set; }
        public static bool NoCounter { get; set; }
        private TimeSpan time;
        public int Number { get; set; }
        public string Name { get; set; } = String.Empty;
        [ObservableProperty]
        private string displayTime = String.Empty;


        public TimeSpan Time
        {
            get => time;
            set
            {
                time = value;
                SetDisplayTime(value);
            }
        }

        public FunctionChrono()
        {
            if (!NoCounter)
            {
                Number = Counter;
                Counter++;
            }
        }

        private void SetDisplayTime(TimeSpan t)
        {
            DisplayTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds / 10);
        }
    }
}
