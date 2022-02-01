using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
        CancellationTokenSource cts;

        #region PROPERTIES
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

        public string StatusMessage { get; set; }

        public Color StatusColor { get; set; } = Color.White;
        #endregion

        public AddEditPropertyPage(Property property = null)
        {
            InitializeComponent();

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

            var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                DisplayAlert("Warning!", "No internet access!", "OK");
                btnLocateMe.IsVisible = false;
                btnShowCords.IsVisible = false;
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

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Property.Address)
                || Property.Beds == null
                || Property.Price == null
                || Property.AgentId == null)
                return false;

            return true;
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void btnLocateMe_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                Location location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location);
                    Placemark placemark = placemarks.FirstOrDefault();

                    if (placemark != null)
                    {
                        string geocodeAddress = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}, " +
                            $"{placemark.Locality} {placemark.PostalCode}, " +
                            $"{placemark.CountryName}";

                        Property.Address = geocodeAddress;
                    }

                    LatitudeLabel.Text = location.Latitude.ToString();
                    LongitudeLabel.Text = location.Longitude.ToString();

                    Property.Latitude = location.Latitude;
                    Property.Longitude = location.Longitude;
                    
                }
            }
            catch (Exception)
            {
                cts.Cancel();
            }
            
        }

        private async void btnShowCords_Clicked(object sender, EventArgs e)
        {
            if (Property.Address != null)
            {
                var locations = await Geocoding.GetLocationsAsync(Property.Address);
                Location location = locations.FirstOrDefault();

                if (location != null)
                {
                    LatitudeLabel.Text = location.Latitude.ToString();
                    LongitudeLabel.Text = location.Longitude.ToString();
                }
            }
            else
            {
                await DisplayAlert("Warning!", "Please add a address!", "ADD NOW");
                AddressEntryField.Focus();
            }
        }
    }
}