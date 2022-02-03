using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.IO;
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
        CancellationTokenSource cts;
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
            await Navigation.PushAsync(new AddEditPropertyPage(Property));
        }

        private async void btnPlayText_Clicked(object sender, System.EventArgs e)
        {
            cts = new CancellationTokenSource();
            btnPlayText.IsEnabled = false;
            btnPauseText.IsEnabled = true;
            await TextToSpeech.SpeakAsync(Property.Description, cancelToken: cts.Token);
            btnPlayText.IsEnabled = true;
            btnPauseText.IsEnabled = false;
            cts.Cancel();
        }

        private void btnPauseText_Clicked(object sender, System.EventArgs e)
        {
            cts.Cancel();
            btnPlayText.IsEnabled = true;
            btnPauseText.IsEnabled = false;
        }

        private async void PhoneLabel_Tapped(object sender, System.EventArgs e)
        {
            try
            {
                string Choice = await DisplayActionSheet($"{Property.Vendor.FullName}", "Cancel", null, "Call", "SMS");
                if (Choice == "Call")
                {
                    PhoneDialer.Open(Property.Vendor.Phone);
                }
                else if (Choice == "SMS")
                {
                    SmsMessage message = new SmsMessage($"Hej, {Property.Vendor.FullName}, angående {Property.Address}", Property.Vendor.Phone);
                    await Sms.ComposeAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void MailLabel_Tapped(object sender, System.EventArgs e)
        {
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string attachmentFilePath = Path.Combine(folder, "property.txt");
                File.WriteAllText(attachmentFilePath, $"{Property.Address}");

                EmailMessage mail = new EmailMessage { Subject = "Real Estate" };
                mail.Attachments.Add(new EmailAttachment(attachmentFilePath));

                await Email.ComposeAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void btnMapDirections_Clicked(object sender, EventArgs e)
        {
            try
            {
                Location location = new Location((double)Property.Latitude, (double)Property.Longitude);
                MapLaunchOptions options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}