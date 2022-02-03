using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double G { get; set; }

        public AboutPage()
        {
            try
            {
                Accelerometer.Start(SensorSpeed.Default);
                Accelerometer.ReadingChanged += AccelerometerDataInput;
            }
            catch (Exception e)
            {
                DisplayAlert("Hey!", e.Message, "Ok");
            }
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (Accelerometer.IsMonitoring) Accelerometer.Stop();
        }

        private void AccelerometerDataInput(object sender, AccelerometerChangedEventArgs e)
        {
            var acc = e.Reading;
            X = acc.Acceleration.X;
            Y = acc.Acceleration.Y;
            Z = acc.Acceleration.Z;
            G = X+Y+Z;
        }
    }
}