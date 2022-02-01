using System;
using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Linq;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyDetailPage : ContentPage
    {
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
    }
}