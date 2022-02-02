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
    public partial class ImageListPage : ContentPage
    {
        public List<String> images { get; set; }
        public ImageListPage(List<string> imgList)
        {
            images = imgList;
            InitializeComponent();
            BindingContext = this;
            Accelerometer.Start(SensorSpeed.UI);
            Accelerometer.ShakeDetected += NextPicture;
        }
        
        protected override void OnDisappearing()
        {
            if (Accelerometer.IsMonitoring) Accelerometer.Stop();
            Accelerometer.ShakeDetected -= NextPicture;
        }

        private void NextPicture(object sender, EventArgs e)
        {
            if (View.Position < images.Count-1)
                View.Position = View.Position++;
            else
                View.Position = 0;
        }
    }
}