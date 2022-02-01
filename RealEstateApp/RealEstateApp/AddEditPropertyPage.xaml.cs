using System;
using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
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

        private bool _flashOn;
        private Location _location;
        private bool _onScreen = false;
        public string StatusMessage { get; set; }
        public Color StatusFont { get; set; }
        public Color StatusColor { get; set; } = Color.White;
        public ObservableCollection<Agent> Agents { get; }

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
                SetStatus("Please fill in all the required fields", Color.Red);
            }
            else
            {
                Feedback();
                Repository.SaveProperty(Property);
                await Navigation.PopToRootAsync();
            }
        }

        private bool IsValid()
        {
            return !string.IsNullOrEmpty(Property.Address) && Property.Beds != null && Property.Price != null &&
                   Property.AgentId != null;
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            Feedback();
            await Navigation.PopToRootAsync();
        }

        private async void btnLocation_OnClick(object sender, EventArgs e)
        {
            try
            {
                Feedback();
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
                await DisplayAlert("Location", "You have denied this app to have permission to use location services",
                    "Ok");
            }
        }

        private async void btnAdress_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Property.Address))
            {
                Feedback(true);
                await DisplayAlert("Hey!", "Address field cannot be blank", "Ok");
            }
            else
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    Feedback(true);
                    await DisplayAlert("Hmm...",
                        "It looks like you no longer have internet connectivity. Please try again", "Ok");
                    return;
                }

                try
                {
                    Feedback();
                    var location = await Geocoding.GetLocationsAsync(Property.Address);
                    Property.Longitude = location.FirstOrDefault().Longitude;
                    Property.Latitude = location.FirstOrDefault().Latitude;
                }
                catch (Exception ex)
                {
                    if (ex.GetType().Name == "NSErrorException")
                    {
                        await DisplayAlert("Hey...", "It looks like we couldn't find the address. Please try again",
                            "Ok");
                    }
                    else
                    {
                        await DisplayAlert("Oi! This is weird ...", ex.Message, "Ok");
                    }
                }
            }
        }

        private async void GetLocationFromSystem()
        {
            _location = null;
            try
            {
                _location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
                SetStatus("Trying to locate location", Color.Aqua);
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

        private void Feedback(bool longPress = false)
        {
            try
            {
                if (longPress)
                {
                    HapticFeedback.Perform(HapticFeedbackType.LongPress);
                }
                else
                {
                    HapticFeedback.Perform();
                }
            }
            catch (Exception)
            {
            }
        }

        private void Flashlight_OnClick(object sender, EventArgs e)
        {
            if (Battery.ChargeLevel < 0.2) // Doesn't allow user to turn on flashlight if the user is below 20%
            {
                SetStatus("Battery level is below 20%, and not charging", Color.Red, Color.WhiteSmoke);
            }
            else if (Battery.ChargeLevel < 0.2 && Battery.State == BatteryState.Charging)
            {
                SetStatus($"Battery level is below 20%, but charging - flashlight {_flashOn.ToString()}", Color.Gold);
            }
            else
            {
                if (!_flashOn)
                {
                    SetStatus("Flashlight on", Color.Gold);
                    Flashlight.TurnOnAsync();
                }
                else
                {
                    SetStatus("Flashlight off", Color.Blue, Color.WhiteSmoke);
                    Flashlight.TurnOffAsync();
                }
            }

            _flashOn = !_flashOn;
        }

        private async void SetStatus(string message, Color colour, Color fontColour = default)
        {
            if (_onScreen) return; // Discards the message if there's a message on screen
            _onScreen = true;
            StatusColor = colour;
            StatusFont = fontColour;
            StatusMessage = message;
            Vibration.Vibrate(1000);
            await Task.Delay(2000);
            _onScreen = false;
            StatusMessage = "";
        }
    }
}