using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompassPage : ContentPage
    {
        public CompassPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        CompassData _CompassData;
        SensorSpeed _speed = SensorSpeed.UI;

        public double CurrentHeading { get; set; }
        public double RotationAngle { get; set; }
        public string CurrentAspect { get; set; }

        protected override void OnAppearing()
        {
            Compass.ReadingChanged += Compass_ReadingChanged;
            ToggleCompass();
        }

        protected override void OnDisappearing()
        {
            Compass.ReadingChanged -= Compass_ReadingChanged;
            ToggleCompass();
        }

        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            _CompassData = e.Reading;
            CurrentHeading = _CompassData.HeadingMagneticNorth;
            RotationAngle = _CompassData.HeadingMagneticNorth;

            SetCurrentAspect();
        }

        private void ToggleCompass()
        {
            try
            {
                if (Compass.IsMonitoring)
                    Compass.Stop();
                else
                    Compass.Start(_speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Some other exception has occurred
            }
        }

        private void SetCurrentAspect()
        {
            if (CurrentHeading > 337.5 || CurrentHeading < 22.5)
            {
                CurrentAspect = "North";
            }
            else if (CurrentHeading > 22.5 && CurrentHeading < 67.5)
            {
                CurrentAspect = "Northeast";
            }
            else if (CurrentHeading > 67.5 && CurrentHeading < 112.5)
            {
                CurrentAspect = "East";
            }
            else if (CurrentHeading > 112.5 && CurrentHeading < 157.5)
            {
                CurrentAspect = "Southeast";
            }
            else if (CurrentHeading > 157.5 && CurrentHeading < 202.5)
            {
                CurrentAspect = "South";
            }
            else if (CurrentHeading > 202.5 && CurrentHeading < 247.5)
            {
                CurrentAspect = "Southwest";
            }
            else if (CurrentHeading > 247.5 && CurrentHeading < 292.5)
            {
                CurrentAspect = "West";
            }
            else if (CurrentHeading > 292.5 && CurrentHeading < 337.5)
            {
                CurrentAspect = "Northwest";
            }
        }
    }
}