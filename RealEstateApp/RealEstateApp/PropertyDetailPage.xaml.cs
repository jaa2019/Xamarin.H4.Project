using System;
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
            btnStop.IsEnabled = !btnPlay.IsVisible;
            await TextToSpeech.SpeakAsync(Property.Description, _cts.Token);
        }

        private async void btnStop_OnClick(object sender, EventArgs e)
        {
            btnPlay.IsEnabled = true;
            btnStop.IsEnabled = !btnPlay.IsVisible;
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
            { }
        }

        private async void Image_OnTap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImageListPage(Property.ImageUrls));
        }
    }
}