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
        private bool _sorting = false;
        private Location _location;
        IRepository Repository;
        public ObservableCollection<PropertyListItem> PropertiesCollection { get; } =
            new ObservableCollection<PropertyListItem>();

        public PropertyListPage()
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            BindingContext = this;

            GetLocationFromSystem();
            
            RefreshView.Command = new Command(OnRefresh);
        }

        private async void GetLocationFromSystem()
        {
            _location = null;
            try
            {
                _location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
            }
            catch (FeatureNotSupportedException ex)
            {
                await DisplayAlert("Location", ex.Message, "Ok");
            }
            catch (FeatureNotEnabledException ex)
            {
                await DisplayAlert("Location", "You need to enable location services to use this function", "Ok");
            }
            catch (PermissionException ex)
            {
                await DisplayAlert("Location", "You have denied this app to have permission to use location services",
                    "Ok");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadProperties();
        }

        private async void OnRefresh()
        {
            LoadProperties(_sorting);
            await Task.Delay(500);
            RefreshView.IsRefreshing = false;
        }

        private async void LoadProperties(bool sortDesc = false)
        {
            PropertiesCollection.Clear();
            var items = Repository.GetProperties();
            
            foreach (var item in items.Where(item => item.Latitude != null && item.Longitude != null))
            {
                if (_location == null) GetLocationFromSystem();
                if (item.Latitude != null && item.Longitude != null)
                {
                    item.Distance = _location.CalculateDistance((double)item.Latitude, (double)item.Longitude,
                        DistanceUnits.Kilometers);
                }
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
            LoadProperties(_sorting);
            FontImageSource icon = (FontImageSource)btnSort.IconImageSource;
            icon.Glyph = _sorting ? IconFont.SortAmountUp : IconFont.SortAmountDown;
            btnSort.IconImageSource = icon;
            _sorting = !_sorting;
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