using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.ObjectModel;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyListPage : ContentPage
    {
        IRepository Repository;
        public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new ObservableCollection<PropertyListItem>(); 

        public PropertyListPage()
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            LoadProperties();
            BindingContext = this; 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            LoadProperties();
        }

        void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            LoadProperties();
            list.IsRefreshing = false;
        }

        async void LoadProperties()
        {
            PropertiesCollection.Clear();
            var items = Repository.GetProperties();
            Location location = await Geolocation.GetLocationAsync();
            foreach (Property item in items)
            {
                if (item.Latitude != null && item.Longitude != null)
                {
                    item.Distance = location.CalculateDistance((double)item.Latitude, (double)item.Longitude, DistanceUnits.Kilometers);
                }

                PropertiesCollection.Add(new PropertyListItem(item));
            }
        }

        private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new PropertyDetailPage(e.Item as PropertyListItem));
        }

        private async void AddProperty_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage());
        }

        private void btnSort_OnClick(object sender, EventArgs e)
        {
            // PropertiesCollection.
            throw new NotImplementedException();
        }
    }
}