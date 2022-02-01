using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
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
    public partial class PropertyListPage : ContentPage
    {
        IRepository Repository;
        CancellationTokenSource cts;

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
            Location PhoneLocation = await Geolocation.GetLastKnownLocationAsync();
            if (PhoneLocation == null)
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    cts = new CancellationTokenSource();
                    PhoneLocation = await Geolocation.GetLocationAsync(request, cts.Token);
                }
                catch (Exception)
                {
                    cts.Cancel();
                }
            }

            PropertiesCollection.Clear();
            var items = Repository.GetProperties();

            foreach (Property item in items)
            {
                Location PropertyLocation = new Location((double)item.Latitude, (double)item.Longitude);

                PropertyListItem propertyListItem = new PropertyListItem(item);
                propertyListItem.Distance = Location.CalculateDistance(PhoneLocation, PropertyLocation, DistanceUnits.Kilometers);
                PropertiesCollection.Add(propertyListItem);
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

        private void btnSortDistance_Clicked(object sender, EventArgs e)
        {
            List<PropertyListItem> propertyListItems = PropertiesCollection.OrderBy(p => p.Distance).ToList();

            PropertiesCollection.Clear();
            foreach (PropertyListItem item in propertyListItems)
            {
                PropertiesCollection.Add(item);
            }
        }
    }
}