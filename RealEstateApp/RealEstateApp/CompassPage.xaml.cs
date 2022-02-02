using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompassPage : ContentPage
    {
        public double CurrentHeading { get; set; }
        public string CurrentAspect { get; set; }
        public double RotationAngle { get; set; }
        public CompassPage()
        {
            Compass.ReadingChanged += GetCompassDirection;
            Compass.Start(SensorSpeed.UI);
            InitializeComponent();
            BindingContext = this;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // TODO
        }

        protected override void OnDisappearing()
        {
            if (Compass.IsMonitoring) Compass.Stop();
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
            if (heading < 22.5-11.5 || heading > (360-11.5)) CurrentAspect = "North";
            if (heading > (22.5*1)-11.5 && heading < (22.5*2)-11.5) CurrentAspect = "North north-east";
            if (heading > (22.5*2)-11.5 && heading < (22.5*3)-11.5) CurrentAspect = "North east";
            if (heading > (22.5*3)-11.5 && heading < (22.5*4)-11.5) CurrentAspect = "East north-east";
            
            if (heading > (22.5*4)-11.5 && heading < (22.5*5)-11.5) CurrentAspect = "East";
            if (heading > (22.5*5)-11.5 && heading < (22.5*6)-11.5) CurrentAspect = "East south-east";
            if (heading > (22.5*6)-11.5 && heading < (22.5*7)-11.5) CurrentAspect = "South east";
            if (heading > (22.5*7)-11.5 && heading < (22.5*8)-11.5) CurrentAspect = "South south-east";
            
            if (heading > (22.5*8)-11.5 && heading < (22.5*9)-11.5) CurrentAspect = "South";
            if (heading > (22.5*9)-11.5 && heading < (22.5*10)-11.5) CurrentAspect = "South south-west";
            if (heading > (22.5*10)-11.5 && heading < (22.5*11)-11.5) CurrentAspect = "South West";
            if (heading > (22.5*11)-11.5 && heading < (22.5*12)-11.5) CurrentAspect = "West south-west";
            
            if (heading > (22.5*12)-11.5 && heading < (22.5*13)-11.5) CurrentAspect = "West";
            if (heading > (22.5*13)-11.5 && heading < (22.5*14)-11.5) CurrentAspect = "West north-west";
            if (heading > (22.5*14)-11.5 && heading < (22.5*15)-11.5) CurrentAspect = "North West";
            if (heading > (22.5*15)-11.5 && heading < (22.5*16)-11.5) CurrentAspect = "North north-west";
        }
    }
}