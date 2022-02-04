using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public IEnumerable<Locale> locales { get; set; }
        public double Volume { get; set; }
        public double Pitch { get; set; }
        public Locale Locale { get; set; }
        private string loc { get; set; }


        public AboutPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetLocale();
            Volume = Preferences.Get("TTS_Volume", .5);
            Pitch = Preferences.Get("TTS_Pitch", 1);
            loc = Preferences.Get("TTS_Lang", "en-US");
            Locale = locales.FirstOrDefault(l => l.Language == loc);
            LangPick.SelectedIndex = locales.IndexOf(l => l.Language == loc);
        }

        private async void GetLocale()
        {
            var locs = await TextToSpeech.GetLocalesAsync();
            locales = locs.ToList();
        }

        private async void BtnSave_OnClick(object sender, EventArgs e)
        {
            Preferences.Set("TTS_Volume", Volume);
            Preferences.Set("TTS_Pitch", Pitch);
            Preferences.Set("TTS_Lang", Locale.Language);
            SavingActivityIndicator.IsVisible = true;
            SavingActivityIndicator.IsEnabled = true;
            SavingActivityIndicator.IsRunning = true;
            await Task.Delay(1000);
            SavingActivityIndicator.IsVisible = false;
            SavingActivityIndicator.IsEnabled = false;
            SavingActivityIndicator.IsRunning = false;
            await DisplayAlert("Hey!", "Settings are saved", "Ok");
        }

        private void LangPick_OnSelect(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int index = picker.SelectedIndex;
            if (index != -1)
            {
                Locale = locales.ToList()[index];
            }
        }

        private void BtnClear_OnClick(object sender, EventArgs e)
        {
            Preferences.Clear();
        }
    }
}