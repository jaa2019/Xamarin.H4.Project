using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstateApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompassPage : ContentPage
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public double CurrentHeading { get; set; }
        public string CurrentAspect { get; set; }
        public double RotationAngle { get; set; }
        private Property _property;

        public CompassPage(Property property)
        {
            _property = property;
            Compass.Start(SensorSpeed.UI);
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            if (!Compass.IsMonitoring) Compass.Start(SensorSpeed.UI);
            Compass.ReadingChanged += GetCompassDirection;
        }

        protected override void OnDisappearing()
        {
            if (Compass.IsMonitoring) Compass.Stop();
            Compass.ReadingChanged -= GetCompassDirection;
        }

        private void GetCompassDirection(object sender, CompassChangedEventArgs e)
        {
            var compassData = e.Reading;
            RotationAngle = compassData.HeadingMagneticNorth * -1;
            CurrentHeading = compassData.HeadingMagneticNorth;
            CalculateHeading(compassData.HeadingMagneticNorth);
        }

        private void CalculateHeading(double heading)
        {
            CurrentAspect = heading switch
            {
                < 11D or > 348.5D => "North",
                > 11D and < 33.5D => "North north-east",
                > 33.5D and < 56D => "North east",
                > 56D and < 78.5D => "East north-east",
                > 78.5D and < 101D => "East",
                > 101D and < 123.5D => "East south-east",
                > 123.5D and < 146D => "South east",
                > 146D and < 168.5D => "South south-east",
                > 168.5D and < 191D => "South",
                > 191D and < 213.5D => "South south-west",
                > 213.5D and < 236D => "South West",
                > 236D and < 258.5D => "West south-west",
                > 258.5D and < 281D => "West",
                > 281D and < 303.5D => "West north-west",
                > 303.5D and < 326D => "North West",
                > 326D and < 348.5D => "North north-west",
                _ => CurrentAspect
            };
        }

        private async void btnBack_OnClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void btnSave_OnClick(object sender, EventArgs e)
        {
            _property.Aspect = CurrentAspect;
            await Navigation.PopModalAsync();
        }
    }
}