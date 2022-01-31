using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FontAwesome;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyListPage : ContentPage
    {
        private bool sorting = false;
        IRepository Repository;

        public ObservableCollection<PropertyListItem> PropertiesCollection { get; } =
            new ObservableCollection<PropertyListItem>();

        public PropertyListPage()
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            LoadProperties();
            BindingContext = this;

            RefreshView.Command = new Command(OnRefresh);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadProperties();
        }

        private async void OnRefresh()
        {
            LoadProperties(sorting);
            await Task.Delay(500);
            RefreshView.IsRefreshing = false;
        }

        private async void LoadProperties(bool sortDesc = false)
        {
            PropertiesCollection.Clear();
            var items = Repository.GetProperties();
            Location location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
            foreach (var item in items.Where(item => item.Latitude != null && item.Longitude != null))
            {
                item.Distance = location.CalculateDistance((double)item.Latitude, (double)item.Longitude,
                    DistanceUnits.Kilometers);
            }

            if (!sortDesc)
            {
                foreach (var item in items.OrderBy(p => p.Distance))
                {
                    PropertiesCollection.Add(new PropertyListItem(item));
                }
            }
            else
            {
                foreach (var item in items.OrderByDescending(p => p.Distance))
                {
                    PropertiesCollection.Add(new PropertyListItem(item));
                }
            }
        }

        private void btnSort_OnClick(object sender, EventArgs e)
        {
            LoadProperties(sorting);
            FontImageSource icon = (FontImageSource)btnSort.IconImageSource;
            icon.Glyph = sorting ? IconFont.SortAmountUp : IconFont.SortAmountDown;
            sorting = !sorting;
        }

        private async void CollectionView_OnClick(object sender, SelectionChangedEventArgs e)
        {
            await Navigation.PushAsync(new PropertyDetailPage((PropertyListItem)e.CurrentSelection.FirstOrDefault()));
        }

        private async void AddProperty_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage());
        }
    }
}