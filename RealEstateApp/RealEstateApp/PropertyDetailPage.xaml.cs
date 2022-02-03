using System;
using System.Collections.Generic;
using System.IO;
using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Linq;
using System.Threading;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyDetailPage : ContentPage
    {
        private CancellationTokenSource _cts;

        public PropertyDetailPage(PropertyListItem propertyListItem)
        {
            InitializeComponent();

            Property = propertyListItem.Property;

            IRepository Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

            BindingContext = this;
        }

        public Agent Agent { get; set; }

        public Property Property { get; set; }

        private async void EditProperty_Clicked(object sender, System.EventArgs e)
        {
            Feedback(true);
            await Navigation.PushAsync(new AddEditPropertyPage(Property));
        }

        private async void btnPlay_OnClick(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            btnPlay.IsEnabled = false;
            btnStop.IsEnabled = !btnPlay.IsEnabled;
            await TextToSpeech.SpeakAsync(Property.Description, _cts.Token);
        }

        private async void btnStop_OnClick(object sender, EventArgs e)
        {
            btnPlay.IsEnabled = true;
            btnStop.IsEnabled = !btnPlay.IsEnabled;
            _cts.Cancel();
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

        private async void Image_OnTap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImageListPage(Property.ImageUrls));
        }

        private async void Email_OnTap(object sender, EventArgs e)
        {
            try
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var attachmentPath = Path.Combine(folder, "property.txt");
                File.WriteAllText(attachmentPath, $"{Property.Address}");
                EmailMessage message = new();
                message.To = new List<string> { Property.Vendor.Email };
                message.Subject = Property.Address;
                message.Body = Property.Description;
                message.Attachments.Add(new EmailAttachment(attachmentPath));
                await Email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oi!", ex.Message, "awwww...");
            }
        }

        private async void Phone_OnTap(object sender, EventArgs e)
        {
            string[] buttons = new[] { "Phone", "Text" };
            var ask = await DisplayActionSheet("Contact", "Cancel", null, FlowDirection.MatchParent, buttons);
            try
            {
                if (ask == "Phone") 
                    PhoneDialer.Open(Property.Vendor.Phone);
                if (ask == "Text")
                    await Sms.ComposeAsync(new SmsMessage($"Hello {Property.Vendor.FirstName}, about {Property.Address}",
                        Property.Vendor.Phone));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oi!", ex.Message, "awwww...");
            }
        }

        private void btnShowMap_OnClick(object sender, EventArgs e)
        {
            OpenMaps();
        }

        private void btnGetDirection_OnClick(object sender, EventArgs e)
        {
            OpenMaps(true);
        }

        private async void OpenMaps(bool navigation = false)
        {
            var options = navigation ? new MapLaunchOptions { NavigationMode = NavigationMode.Driving } : new MapLaunchOptions();
            Location location = new Location((double)Property.Latitude, (double)Property.Longitude);
            await Map.OpenAsync(location, options);
        }

        private void Description_OnTap(object sender, EventArgs e)
        {
            OpenBrowser();
        }
        
        private void btnExLink_OnClick(object sender, EventArgs e)
        {
            OpenBrowser(true);
        }

        private async void OpenBrowser(bool external = false)
        {
            var mode = external ? BrowserLaunchMode.External : BrowserLaunchMode.SystemPreferred;
            await Browser.OpenAsync(Property.NeighbourhoodUrl, mode);
        }

        private void btnPdf_OnClick(object sender, EventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var file = Path.Combine(folder, "contract.pdf");
            
        }
    }
}