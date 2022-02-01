using System;
using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPropertyPage : ContentPage
    {
        private IRepository Repository;

        #region PROPERTIES
        public ObservableCollection<Agent> Agents { get; }
        private Location _location;
        private Property _property;
        public Property Property
        {
            get => _property;
            set
            {
                _property = value;
                if (_property.AgentId != null)
                {
                    SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
                }
               
            }
        }
    
        private Agent _selectedAgent;

        public Agent SelectedAgent
        {
            get => _selectedAgent;
            set
            {
                if (Property != null)
                {
                    _selectedAgent = value;
                    Property.AgentId = _selectedAgent?.Id;
                }                 
            }
        }

        public string StatusMessage { get; set; }

        public Color StatusColor { get; set; } = Color.White;
        #endregion

        public AddEditPropertyPage(Property property = null)
        {
            InitializeComponent();
            btnAdress.IsEnabled = false || Connectivity.NetworkAccess == NetworkAccess.Internet;
            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agents = new ObservableCollection<Agent>(Repository.GetAgents());

            if (property == null)
            {
                Title = "Add Property";
                Property = new Property();
            }
            else
            {
                Title = "Edit Property";
                Property = property;
            }
         
            BindingContext = this;
        }

        private async void SaveProperty_Clicked(object sender, System.EventArgs e)
        {
            if (IsValid() == false)
            {
                StatusMessage = "Please fill in all required fields";
                StatusColor = Color.Red;
            }
            else
            {
                Repository.SaveProperty(Property);
                await Navigation.PopToRootAsync();
            }   
        }

        private bool IsValid()
        {
            return !string.IsNullOrEmpty(Property.Address) && Property.Beds != null && Property.Price != null && Property.AgentId != null;
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void btnLocation_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (_location == null) GetLocationFromSystem();
                lblLat.Text = _location?.Latitude.ToString(CultureInfo.InvariantCulture);
                lblLong.Text = _location?.Longitude.ToString(CultureInfo.CurrentCulture);
                var placemarks = await Geocoding.GetPlacemarksAsync(_location);
                var p = placemarks.FirstOrDefault();
                if (p != null)
                {
                    Property.Address =
                        $"{p.SubThoroughfare}, {p.Thoroughfare}, {p.Locality}, {p.PostalCode}, {p.CountryName}";
                }
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
                await DisplayAlert("Location", "You have denied this app to have permission to use location services", "Ok");
            }
        }

        private async void btnAdress_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Property.Address))
            {
                await DisplayAlert("Hey!", "Address field cannot be blank", "Ok");
            }
            else
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await DisplayAlert("Hmm...",
                        "It looks like you no longer have internet connectivity. Please try again", "Ok");
                    return;
                }
                try
                {
                    var location = await Geocoding.GetLocationsAsync(Property.Address);
                    Property.Longitude = location.FirstOrDefault().Longitude;
                    Property.Latitude = location.FirstOrDefault().Latitude;
                }
                catch (Exception exception)
                {
                    await DisplayAlert("Oi! This is weird ...", exception.Message, "Ok");
                }
            }
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
    }
}