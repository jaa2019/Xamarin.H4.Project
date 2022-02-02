using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RealEstateApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeightCalculatorPage : ContentPage
    {
        public double CurrentAltitude { get; set; }
        public BarometerMeasurement BarometerMeasurement { get; set; } = new BarometerMeasurement();
        public ObservableCollection<BarometerMeasurement> BarometerMeasurements { get; set; } = new ObservableCollection<BarometerMeasurement>();
        
        
        public HeightCalculatorPage()
        {
            Barometer.ReadingChanged += BarometerChanged;
            Barometer.Start(SensorSpeed.UI);
            
            InitializeComponent();
            BindingContext = this;
        }
        
        protected override void OnDisappearing()
        {
            if (Barometer.IsMonitoring) Barometer.Stop();
            Barometer.ReadingChanged -= BarometerChanged;
        }

        private void BarometerChanged(object sender, BarometerChangedEventArgs e)
        {
            var barometerData = e.Reading;
            CurrentAltitude = 44307.694 * (1 - Math.Pow(barometerData.PressureInHectopascals / 1010, 0.190284));

            BarometerMeasurement.Altitude = CurrentAltitude;
            BarometerMeasurement.Pressure = barometerData.PressureInHectopascals;
        }

        private void btnSave_OnClick(object sender, EventArgs e)
        {
            double prevHeight = 0;
            if (BarometerMeasurements.Count > 0)
            {
                prevHeight = BarometerMeasurements.LastOrDefault().Altitude;
            }
            BarometerMeasurements.Add(new BarometerMeasurement(
                BarometerMeasurement.Pressure, 
                BarometerMeasurement.Altitude, 
                BarometerMeasurement.Label,
                (BarometerMeasurement.Altitude - prevHeight)));
            BarometerMeasurement = new BarometerMeasurement();
        }
    }
}