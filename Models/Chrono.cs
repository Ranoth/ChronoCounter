using ChronoCounter.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChronoCounter.Models
{
    public partial class Chrono : ViewModelBase
    {
        public static int Counter { get; set; }
        private TimeSpan time;
        public int Id { get; set; }
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

        public Chrono()
        {
            Id = Counter;
            Counter++;
        }

        private void SetDisplayTime(TimeSpan t)
        {
            DisplayTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds / 10);
        }
    }
}
