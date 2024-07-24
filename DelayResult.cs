using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Platform;
using Syncfusion.Maui.Gauges;
using Syncfusion.Maui.Popup;

namespace SfPopUpAnComboTest
{
    public partial class DelayResult : ObservableObject
    {
        IDispatcherTimer timer = App.Current.Dispatcher.CreateTimer();  
        [ObservableProperty]
        Label countDownLabel;
        private readonly double numberOfMillisecondsToCountDownFrom;
        private string returnValue = "TimeOut";
        private readonly TimeSpan countDownInterval = TimeSpan.FromSeconds(1);
        private TimeSpan countDownTimeSpan;
        private SfPopup ShowCountDownTimer;

        public DelayResult(double numberOfSecondsToCountDownFrom)
        {
            timer.Interval = countDownInterval;
            numberOfMillisecondsToCountDownFrom = TimeSpan.FromSeconds(numberOfSecondsToCountDownFrom).TotalMilliseconds;
            countDownTimeSpan = TimeSpan.FromSeconds(numberOfSecondsToCountDownFrom);

            ShowCountDownTimer = new()
            {
                ShowHeader = true,
                HeaderHeight = 50,
                ShowFooter = false,
                HeightRequest = 250,
                WidthRequest = 300,
                StaysOpen = true,
                ShowCloseButton = true,
                PopupStyle = new PopupStyle()
                {
                    CornerRadius = 7,
                    HasShadow = true
                },
                StartY = (int)(DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density / 5)

            }; 
            ShowCountDownTimer.PopupStyle.SetAppThemeColor(PopupStyle.PopupBackgroundProperty, Colors.WhiteSmoke, Color.FromArgb("#FF6E6E6E"));
            ShowCountDownTimer.Closed += ShowCountDownTimer_Closed;

            CountDownLabel = new()
            {
                FontSize = 40,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            CountDownLabel.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#FF003366"), Colors.WhiteSmoke);

            DataTemplate _headerTemplate = new(() =>
            {
                Label headerContent = new() 
                {
                    Text = $"Waiting for completion.",  
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    Padding = new Thickness(10, 5, 0, 5),
                };
                return headerContent;
            });
            ShowCountDownTimer.HeaderTemplate = _headerTemplate;

            SfRadialGauge sfRadialGauge = new()
            {
                HeightRequest = 200,
                BackgroundColor = Colors.Transparent
            };

            RadialAxis radialAxis = new()
            {
                StartAngle = 270,
                EndAngle = 270,
                IsInversed = true,
                Minimum = 0,
                Maximum = numberOfMillisecondsToCountDownFrom,
                ShowLabels = false,
                ShowTicks = false,
            };
            RadialRange radialRange = new()
            {
                StartValue = 0,
                EndValue = numberOfMillisecondsToCountDownFrom,
            };
            radialRange.SetAppThemeColor(RadialRange.FillProperty, Color.FromArgb("#FF003366"), Colors.WhiteSmoke);
            radialAxis.Ranges.Add(radialRange);
            sfRadialGauge.Axes.Add(radialAxis);

            RangePointer rangePointer = new()
            {
                AnimationDuration = numberOfMillisecondsToCountDownFrom,
                EnableAnimation = true,
                Value = numberOfMillisecondsToCountDownFrom,
                PointerWidth = 15,
                PointerOffset = -3
            };
            rangePointer.SetAppThemeColor(RangePointer.FillProperty, Colors.WhiteSmoke, Color.FromArgb("#FF6E6E6E"));
            radialAxis.Pointers.Add(rangePointer);

            ShowCountDownTimer.ContentTemplate = new DataTemplate(() =>
            {
                var _content = new AbsoluteLayout { sfRadialGauge, CountDownLabel };
                _content.SetLayoutBounds(sfRadialGauge, new Rect(0.5, 0.5, 1, 1));
                _content.SetLayoutFlags(sfRadialGauge, AbsoluteLayoutFlags.All);
                _content.SetLayoutBounds(CountDownLabel, new Rect(0.5, 0.5, 1, 1));
                _content.SetLayoutFlags(CountDownLabel, AbsoluteLayoutFlags.All);
                return _content;
            });
        }

        public async Task<string> GetResult()
        {
            CountDownLabel.Text = countDownTimeSpan.ToFormattedString("mm:ss");
            timer.Tick += CountDown;
            timer.Start();
            await ShowCountDownTimer.ShowAsync();
            return returnValue;  // returnvalue is modified by Receive through AppLink message.
        }

        private void CountDown(object? sender, EventArgs e) 
        {
            countDownTimeSpan = countDownTimeSpan.Subtract(countDownInterval);
            if (ShowCountDownTimer != null && ShowCountDownTimer.IsOpen == true)
            {
                CountDownLabel.Text = countDownTimeSpan.ToFormattedString("mm:ss");
                if (countDownTimeSpan.TotalSeconds == 0 && ShowCountDownTimer != null)
                    ShowCountDownTimer.IsOpen = false;
            }
        }

        //public void Receive(PaymentResultMessage message)
        //{
        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        returnValue = message.Value;
        //        if (ShowCountDownTimer != null && ShowCountDownTimer.IsOpen == true)
        //            ShowCountDownTimer.IsOpen = false;
        //    });
        //}

        private void ShowCountDownTimer_Closed(object? sender, EventArgs e)
        {
            //TODO If sender is closebutton, returnValue should be set to "Abort"
            timer.Stop();
            // Disposes of eventhandlers
            try
            {
                if (ShowCountDownTimer != null)
                {
                    ShowCountDownTimer.Closed -= ShowCountDownTimer_Closed;   //This line causes ANR. Proberbly not needed.
                    ShowCountDownTimer.Dismiss();
                }
                if (timer != null)
                    timer.Tick -= CountDown;
            }
            catch (Exception ex)
            {
                //TODO Add sentry
            }
        }
    }
}