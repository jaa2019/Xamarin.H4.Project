using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstateApp.Models;
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
        }
    }
}