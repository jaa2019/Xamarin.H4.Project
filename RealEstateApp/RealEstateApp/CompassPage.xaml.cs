using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompassPage : ContentPage
    {
        public string CurrentHeading { get; set; }
        public string CurrentAspect { get; set; }
        public string RotationAngle { get; set; }
        public CompassPage()
        {
            
            InitializeComponent();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // DO SOMETHING
        }
    }
}